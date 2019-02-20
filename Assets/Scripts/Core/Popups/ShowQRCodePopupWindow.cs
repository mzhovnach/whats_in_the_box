using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This popup shows on patient side, doctor should scan this QR code
// Popup closes by receiving OnQRCodeScannedServerEvent (or if user close it earlier)

public class ShowQRCodePopupWindow : PopupWindow
{       
    public Image QRImage;
    public GameObject SuccessScanPanel;
    public GameObject ErrorScanPanel;    
    private string _encodedQR;

    public static void ShowPopup(string qrcode)
    {
        EventData e = new EventData("OnShowPopupWindowEvent");
        e.Data["Type"] = typeof(ShowQRCodePopupWindow);        
        e.Data["QRCode"] = qrcode;
        AppManager.Instance.EventManager.CallOnShowPopupWindowEvent(e);
    }

    public void OnClosePopupButtonPressed()
    {
        ClosePopup();
    }    

    private void ClosePopup()
    {        
        EventData e = new EventData("OnHidePopupWindowEvent");
        e.Data["Type"] = typeof(ShowQRCodePopupWindow);
		AppManager.Instance.EventManager.CallOnHidePopupWindowEvent(e);
    }

    public override void Show(Dictionary<string, object> data)
    {
        base.Show(data);
        SuccessScanPanel.SetActive(false);
        ErrorScanPanel.SetActive(false);        
        _encodedQR = data["QRCode"] as string;
        QRImage.sprite = ImageUtils.CreateSpriteFromBytes(System.Convert.FromBase64String(_encodedQR));
    }

    //public void HidePopupOnServerEvent(OnQRCodeScannedServerEvent qrevent, OnQRCodeScannedServerEvent.OnQRCodeScannedServerEventResponse data)
    //{
    //    StartCoroutine(HidePopupOnServerEventCo(qrevent, data.success));
    //}

    //private IEnumerator HidePopupOnServerEventCo(OnQRCodeScannedServerEvent qrevent, bool result)
    //{
    //    // show good or fail icon (better animated)
    //    SuccessScanPanel.SetActive(result);
    //    ErrorScanPanel.SetActive(!result);
    //    // wait 2 second
    //    yield return new WaitForSeconds(2.0f);
    //    // close popup
    //    ClosePopup();
    //    qrevent.SetProcessed();

    //    Dictionary<string, object> screenData = new Dictionary<string, object>();
    //    screenData["visitId"] = _visitID;
    //    WindowsController.TransitToScreen(typeof(PatientVisitInProgressScreen), screenData);
    //}
}
