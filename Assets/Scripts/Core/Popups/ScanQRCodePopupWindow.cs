using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZXing;

public class ScanQRCodePopupWindow : PopupWindow
{   
    public Image NotAvailableImage;
    public RawImage PreviewImage;   
    public Action<string> OnQRCodeScannedCallback;
    public dZine4D.Misc.QR.QRReader QRReader;

    public static void ShowPopup(Action<string> callback)
    {
        EventData e = new EventData("OnShowPopupWindowEvent");
        e.Data["Type"] = typeof(ScanQRCodePopupWindow);
        e.Data["Callback"] = callback;
		AppManager.Instance.EventManager.CallOnShowPopupWindowEvent(e);
    }      

    public void OnQRCodeDetected(Result resultObj)
    {        
        ClosePopup();
        OnQRCodeScannedCallback(resultObj.Text);
    }

    public void OnClosePopupButtonPressed()
    {
        ClosePopup();
    }    

    private void ClosePopup()
    {
        QRReader.DisableReader();        
        EventData e = new EventData("OnHidePopupWindowEvent");
        e.Data["Type"] = typeof(ScanQRCodePopupWindow);
		AppManager.Instance.EventManager.CallOnHidePopupWindowEvent(e);
    }

    public override void Show(Dictionary<string, object> data)
    {
        base.Show(data);
        OnQRCodeScannedCallback = data["Callback"] as Action<string>;
        QRReader.EnableReader();
    }
}
