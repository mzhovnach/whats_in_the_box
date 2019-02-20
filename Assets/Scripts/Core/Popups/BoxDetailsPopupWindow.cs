using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This popup shows on patient side, doctor should scan this QR code
// Popup closes by receiving OnQRCodeScannedServerEvent (or if user close it earlier)

public class BoxDetailsPopupWindow : PopupWindow
{       
    public Image PhotoImage;
    public Sprite EmptyBoxSprite;
    public Text BoxNumberLabel;
    private Action OnDetailsClosedCallback;

    private BoxData _boxData;

    public static void ShowPopup(BoxData data, Texture2D photo, Action onDetailsClosedCallback)
    {
        EventData e = new EventData("OnShowPopupWindowEvent");
        e.Data["Type"] = typeof(BoxDetailsPopupWindow);        
        e.Data["Data"] = data;
        e.Data["Photo"] = photo;
        e.Data["CloseCallback"] = onDetailsClosedCallback;
        AppManager.Instance.EventManager.CallOnShowPopupWindowEvent(e);
    }

    public void OnClosePopupButtonPressed()
    {
        ClosePopup();
    }    

    private void ClosePopup()
    {        
        if (OnDetailsClosedCallback != null)
        {
            OnDetailsClosedCallback();
        }
        EventData e = new EventData("OnHidePopupWindowEvent");
        e.Data["Type"] = typeof(BoxDetailsPopupWindow);
		AppManager.Instance.EventManager.CallOnHidePopupWindowEvent(e);
    }

    public override void Show(Dictionary<string, object> data)
    {
        base.Show(data);
        _boxData = (BoxData)data["Data"];
        OnDetailsClosedCallback = (Action)data["CloseCallback"];
        Texture2D photoTexture = (Texture2D)data["Photo"];
        if (photoTexture != null)
        {
            PhotoImage.sprite = Sprite.Create(photoTexture, new Rect(0, 0, photoTexture.width, photoTexture.height), new Vector2(0.5f, 0.5f));
        }
        else
        {
            PhotoImage.sprite = EmptyBoxSprite;
        }

        BoxNumberLabel.text = _boxData.box_index.ToString("D4");
    }

    public void OnUpdateBoxPhotoButtonPressed()
    {
        transform.SetAsFirstSibling();
        TakePicturePopupWindow.ShowPopup(OnPictureTakedCallback);
    }

    public void OnRemoveBoxButtonPressed()
    {
        AppManager.Instance.Backend.Database.RemoveBox(AppManager.Instance.User.collection_id, _boxData, OnRemoveBoxCallback);
    }

    public void OnRemoveBoxCallback(bool success)
    {
        if (success)
        {
            ClosePopup();
        }
    }

    private void OnPictureTakedCallback(Texture2D imageTexture)
    {
        transform.SetAsLastSibling();

        if (imageTexture != null)
        {
            // make a square from picture
            Texture2D squareTexture = null;
            int w = imageTexture.width;
            int h = imageTexture.height;
            if (w > h)
            {
                int offsetX = (w - h) / 2;

                Color[] c = imageTexture.GetPixels(offsetX, 0, h, h);
                squareTexture = new Texture2D(h, h);
                squareTexture.SetPixels(c);
                squareTexture.Apply();
            }
            else
            {
                int offsetY = (h - w) / 2;

                Color[] c = imageTexture.GetPixels(0, offsetY, w, w);
                squareTexture = new Texture2D(w, w);
                squareTexture.SetPixels(c);
                squareTexture.Apply();
            }

            PhotoImage.sprite = Sprite.Create(squareTexture, new Rect(0, 0, squareTexture.width, squareTexture.height), new Vector2(0.5f, 0.5f));

            // resize texure to 512px
            //squareTexture.Resize(512, 512);
            CreateBoxTask newBoxTask = new CreateBoxTask(_boxData.box_index, AppManager.Instance.User.collection_id, squareTexture, OnBoxCreated);
        }
    }

    public void OnBoxCreated(bool success)
    {
        if (success)
        {
            Debug.Log("Box created success!!!");
        }
        else
        {
            Debug.Log("Error creating new box!");
        }
    }
}
