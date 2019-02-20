using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TakePicturePopupWindow : PopupWindow
{
    public Button TakePictureButton;
    public Button SwitchCameraButton;
    public Image NotAvailableImage;
    public RawImage PreviewImage; 
    [HideInInspector]
    public Texture2D Picture;
    public Texture2D TestPicture;
    public DeviceCameraController CameraController;
    private int _currentDevice;
    public Action<Texture2D> OnPictureTakenCallback;
    private bool _available;


    public static void ShowPopup(Action<Texture2D> callback)
    {
        EventData e = new EventData("OnShowPopupWindowEvent");
        e.Data["Type"] = typeof(TakePicturePopupWindow);
        e.Data["Callback"] = callback;
		AppManager.Instance.EventManager.CallOnShowPopupWindowEvent(e);
    }

    void Start()
    {   
        _available = WebCamTexture.devices.Length > 0;
        NotAvailableImage.gameObject.SetActive(!_available);
        PreviewImage.gameObject.SetActive(_available);
        TakePictureButton.interactable = _available;
        SwitchCameraButton.interactable = _available;
//#if UNITY_EDITOR
//        TakePictureButton.interactable = true;
//#endif
    }   

    public void OnTakePictureButtonPressed()
    {
//#if UNITY_EDITOR
//        Picture = TestPicture;
//#else
        WebCamTexture wct = CameraController.GetActiveTexture();
        Picture = new Texture2D(wct.width, wct.height);
        Picture.SetPixels(wct.GetPixels());

        if (wct.videoRotationAngle == 90)
        {
            TextureRotate.RotateTexture(Picture, TextureRotate.Rotate.Right);
        }
        //for rect image could be used crop
        // like Picture.ReadPixels(new Rect(0,0,512,512),0,0);
        Picture.Apply();        
//#endif
        if (OnPictureTakenCallback != null)
        {
            OnPictureTakenCallback(Picture);
        }
        ClosePopup();
    }

    public void OnClosePopupButtonPressed()
    {
        if (OnPictureTakenCallback != null)
        {
            OnPictureTakenCallback(null);
        }
        ClosePopup();
    }    

    private void ClosePopup()
    {
        CameraController.DisableWebCamera();
        OnPictureTakenCallback = null;
        EventData e = new EventData("OnHidePopupWindowEvent");
        e.Data["Type"] = typeof(TakePicturePopupWindow);
		AppManager.Instance.EventManager.CallOnHidePopupWindowEvent(e);
    }

    public override void Show(Dictionary<string, object> data)
    {
        base.Show(data);
        OnPictureTakenCallback = data["Callback"] as Action<Texture2D>;
        CameraController.EnableWebCamera();
    }
}
