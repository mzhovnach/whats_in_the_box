using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using ZXing;

namespace dZine4D.Misc.QR
{
    /// <summary>
    /// Detects qr codes using the webcam/phone camera
    /// </summary>
    public class QRReader : MonoBehaviour
    {
        // .. ATTRIBUTES
        public string LastResult { get; private set; }
        public Texture2D TestTexture;

        [SerializeField]
        [Tooltip("An optional renderer component to display the camera feed.")]
        private Renderer OutputRenderer = null;
        [SerializeField]
        [Tooltip("An optional RawImage component to display the camera feed.")]
        public RawImage OutputImage = null;
        [SerializeField]
        [Tooltip("An optional text component to display the last qr decoding result.")]
        private Text OutputText = null;
        [SerializeField]
        [Tooltip("Should we start decoding on awake?")]
        private bool EnableOnAwake = true;

        private WebCamTexture camTexture;
        //private Thread qrThread;

        private int W = 512;
        private int H = 512;

        private Color32[] cameraFeedGrab;
        private bool isQuit;
        private bool isReaderEnabled;

        private string prevResult;
        private Result _resultObj;
        private bool _cameraAvailable;

        // .. EVENTS

        [Serializable]
        public class QrCodeDetectedEvent : UnityEvent<Result> { }
        public QrCodeDetectedEvent OnQrCodeDetected;


        // .. INITIALIZATION

        void Awake()
        {
            isReaderEnabled = false;
            _cameraAvailable = WebCamTexture.devices.Length > 0;
            LastResult = string.Empty;
            Init();
            if (EnableOnAwake)
                EnableReader();

            //qrThread = new Thread(DecodeQR);
            //qrThread.Start();
        }

        private void Init()
        {
            camTexture = new WebCamTexture(W, H);
            if (OutputRenderer != null)
                OutputRenderer.material.mainTexture = camTexture;
            if (OutputImage != null)
            {
                if (_cameraAvailable)
                {
                    OutputImage.texture = camTexture;
                }
                else
                {
                    OutputImage.texture = TestTexture;
                }
            }
        }
        // .. OPERATIONS

        public void EnableReader()
        {
            Init();            
            StopCoroutine("EnableReaderRoutine");
            StartCoroutine("EnableReaderRoutine");
            StopCoroutine("DecodeQR");
            StartCoroutine("DecodeQR");
        }

        public void DisableReader()
        {
            if (!isReaderEnabled)
                return;
            isReaderEnabled = false;

            LastResult = string.Empty;
            prevResult = string.Empty;
            cameraFeedGrab = null;            
            camTexture.Stop();
            camTexture = null;  
            StopCoroutine("DecodeQR");
        }       

        void Update()
        {
            if (!isReaderEnabled)
                return;

            if (cameraFeedGrab == null)
            {
                if (_cameraAvailable)
                {
                    cameraFeedGrab = camTexture.GetPixels32();
                } else
                {
                    cameraFeedGrab = TestTexture.GetPixels32();
                }
            }

            if (!string.IsNullOrEmpty(LastResult) && LastResult != prevResult)
            {
                if (OnQrCodeDetected != null)
                    OnQrCodeDetected.Invoke(_resultObj);

                if (OutputText != null)
                    OutputText.text = LastResult;
                //DisableReader();
                LastResult = "";
                prevResult = LastResult; 
            }
        }

        void OnDestroy()
        {
            StopCoroutine("DecodeQR");
            if (camTexture != null)
            {
                camTexture.Stop();
            }
        }

        // It's better to stop the thread by itself rather than abort it.
        void OnApplicationQuit()
        {
            isQuit = true;
        }

        IEnumerator DecodeQR()
        {
			// create a reader with a custom luminance source
			var barcodeReader = new BarcodeReader();

            while (true)
            {

                if (!isReaderEnabled)
                {
                    yield return new WaitForSeconds(1.0f);
                    continue;
                }

				// decode the current fram
				if (cameraFeedGrab != null)
				{
                    try
                    {
                        Result result = barcodeReader.Decode(cameraFeedGrab, W, H);
                        if (result != null)
                        {
                            _resultObj = result;
                            LastResult = result.Text;
                            print(result.Text);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error in barcodeReader.Decode + " + e.ToString());
                    };

                    // Sleep a little bit and set the signal to get the next frame
                    yield return new WaitForSeconds(0.25f);
				}
                cameraFeedGrab = null;
                yield return new WaitForEndOfFrame();
            }
        }

        // .. COROUTINES

        IEnumerator EnableReaderRoutine()
        {
            if (isReaderEnabled)
                yield break;

            LastResult = string.Empty;
            prevResult = string.Empty;
            cameraFeedGrab = null;

            if (_cameraAvailable)
            {
                camTexture.Play();
                //while (camTexture.width != W && camTexture.height != H)
                //{
                //    Debug.Log(camTexture.width);
                //    W = camTexture.width;
                //    H = camTexture.height;
                //    yield return new WaitForSeconds(1.0f);
                //}
                while (!camTexture.didUpdateThisFrame)
                {
                    yield return new WaitForEndOfFrame();
                }
                W = camTexture.width;
                H = camTexture.height;
                Debug.Log("W=" + W + " H=" + H);

                // Rotate image to show correct orientation 
                Vector3 rotationVector = Vector3.zero;
                rotationVector.z = -camTexture.videoRotationAngle;
                OutputImage.rectTransform.localEulerAngles = rotationVector;

                // Image uvRect
                Rect defaultRect = new Rect(0f, 0f, 1f, 1f);
                Rect fixedRect = new Rect(0f, 1f, 1f, -1f);

                Vector3 defaultScale = new Vector3(1f, 1f, 1f);
                Vector3 fixedScale = new Vector3(-1f, 1f, 1f);               

                // Unflip if vertically flipped
                OutputImage.uvRect =
                    camTexture.videoVerticallyMirrored ? fixedRect : defaultRect;
            } else
            {
                W = TestTexture.width;
                H = TestTexture.height;
            }

            // Set AspectRatioFitter's ratio
            float videoRatio = (float)W / (float)H;
            OutputImage.GetComponent<AspectRatioFitter>().aspectRatio = videoRatio;

            // Mirror front-facing camera's image horizontally to look more natural
            //OutputImage.transform.parent.localScale =
            //    WebCamTexture.devices[0].isFrontFacing ? fixedScale : defaultScale;

            isReaderEnabled = true;
        }
    }
}
