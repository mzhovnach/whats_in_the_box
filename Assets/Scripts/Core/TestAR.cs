using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class TestAR : MonoBehaviour {

    public Image QRImage;
    public Image Photo;
    public dZine4D.Misc.QR.QRReader QRReader;

    // Use this for initialization
    void Start () {

        //Texture2D boxImage = QRHelper.GenerateQRTexture("0003");
        //QRImage.sprite = Sprite.Create(boxImage, new Rect(0, 0, boxImage.width, boxImage.height), new Vector2(0.5f, 0.5f));

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TestGetMarkers()
    {
        var barcodeReader = new BarcodeReader();
        Color32[] colors = QRImage.sprite.texture.GetPixels32();
        Result result = barcodeReader.Decode(colors, 256, 256);

        Vector3 viewScale = QRImage.GetComponent<RectTransform>().sizeDelta / new Vector2(QRImage.sprite.texture.width, QRImage.sprite.texture.height);

        if (result.ResultPoints.Length == 3)
        {
            Vector3[] points = new Vector3[3];
            int i = 0;
            foreach (var point in result.ResultPoints)
            {
                points[i] = new Vector3(point.X, point.Y, 0);
                ++i;
            }

            float minDist = float.MaxValue;
            float maxDist = float.MinValue;
            Vector3 middlePoint = Vector3.zero;

            foreach(var pointA in points)
            {
                foreach(var pointB in points)
                {
                    if (pointA != pointB)
                    {
                        float dist = (pointA - pointB).magnitude;
                        minDist = Mathf.Min(dist, minDist);

                        if (dist > maxDist)
                        {
                            maxDist = dist;
                            middlePoint = (pointA + pointB) / 2.0f;
                            Debug.Log(pointA + "   " + pointB);
                        }
                    }
                }
            }

            middlePoint.y *= -1;
            Photo.GetComponent<RectTransform>().anchoredPosition3D = middlePoint * viewScale.x;
            Photo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxDist * viewScale.x);
            Photo.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, maxDist * viewScale.x);
        }
    }
}
