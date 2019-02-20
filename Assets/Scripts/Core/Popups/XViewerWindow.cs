using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class XViewerWindow : PopupWindow
{
    public Sprite EmptyBoxSprite;
    public Image NotAvailableImage;
    public RawImage PreviewImage;
    public dZine4D.Misc.QR.QRReader QRReader;
    public Image Photo;
    public Button UpdatePhotoButton;

    public Image ScannedInfoPhoto;
    public Text ScannedInfoNumber;

    private float _lastScannedTimer;
    private BoxList _boxList;
    private BoxData _activeBoxData;
    private Texture2D _activePhoto;

    public static void ShowPopup(BoxList boxList)
    {
        EventData e = new EventData("OnShowPopupWindowEvent");
        e.Data["Type"] = typeof(XViewerWindow);
        e.Data["BoxList"] = boxList;
		AppManager.Instance.EventManager.CallOnShowPopupWindowEvent(e);
    }      

    public void OnQRCodeDetected(Result resultObj)
    {
        _lastScannedTimer = 0.0f;
        Photo.gameObject.SetActive(true);
        UpdatePhotoButton.interactable = true;
        // search for the photo
        _activeBoxData = null;
        _activePhoto = null;
        int box_index = Convert.ToInt32(resultObj.Text);
        foreach(var box in _boxList.BoxItemsList)
        {
            if (box.GetData().box_index == box_index && box.PhotoImage.sprite != null)
            {
                Photo.sprite = box.PhotoImage.sprite;
                _activePhoto = box.PhotoImage.sprite.texture;
                _activeBoxData = box.GetData();
                break;
            }
        }

        // scanned box that are not in user database
        if (_activeBoxData == null)
        {
            Photo.sprite = EmptyBoxSprite;
            _activeBoxData = new BoxData();
            _activeBoxData.box_index = box_index;
        }

        ScannedInfoPhoto.sprite = Photo.sprite;
        ScannedInfoNumber.text = _activeBoxData.box_index.ToString("D4");

        Vector2 sourceSize = new Vector2(QRReader.OutputImage.texture.width, QRReader.OutputImage.texture.height);
        Vector2 viewSize = new Vector2(QRReader.OutputImage.rectTransform.rect.width, QRReader.OutputImage.rectTransform.rect.height);
        Debug.Log("OutputImage size=" + viewSize);
        Debug.Log("Source size=" + sourceSize);
        Vector3 viewScale = viewSize / sourceSize;
        Debug.Log("viewScale = " + viewScale);
        if (resultObj.ResultPoints.Length == 3)
        {
            Vector3[] points = new Vector3[3];
            int i = 0;
            foreach (var point in resultObj.ResultPoints)
            {
                points[i] = new Vector3(point.X, point.Y, 0);
                ++i;
            }

            float minDist = float.MaxValue;
            float maxDist = float.MinValue;
            Vector3 middlePoint = Vector3.zero;

            foreach (var pointA in points)
            {
                foreach (var pointB in points)
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

    private void Update()
    {
        if (Photo.gameObject.activeSelf)
        {
            _lastScannedTimer += Time.deltaTime;
            if (_lastScannedTimer > 1.5f)
            {
                Photo.gameObject.SetActive(false);
                //UpdatePhotoButton.interactable = false;
                _lastScannedTimer = 0.0f;
                //_activeBoxData = null;
                _activePhoto = null;
            }
        }
    }

    public void OnShowBoxDetailsButtonPressed()
    {
        QRReader.DisableReader();
        transform.SetAsFirstSibling();
        BoxDetailsPopupWindow.ShowPopup(_activeBoxData, ScannedInfoPhoto.sprite.texture, OnDetailCloseCallback);
    }

    private void OnDetailCloseCallback()
    {
        QRReader.EnableReader();
    }

    public void OnClosePopupButtonPressed()
    {
        QRReader.DisableReader();
        ClosePopup();
    }    

    private void ClosePopup()
    {
        QRReader.DisableReader();        
        EventData e = new EventData("OnHidePopupWindowEvent");
        e.Data["Type"] = typeof(XViewerWindow);
		AppManager.Instance.EventManager.CallOnHidePopupWindowEvent(e);
    }

    public override void Show(Dictionary<string, object> data)
    {
        base.Show(data);
        _lastScannedTimer = 0.0f;
        Photo.gameObject.SetActive(false);
        UpdatePhotoButton.interactable = false;
        _boxList = data["BoxList"] as BoxList;
//#if !UNITY_EDITOR
        QRReader.EnableReader();
//#else
        //OnQRCodeDetected("0005");
//#endif
    }
}
