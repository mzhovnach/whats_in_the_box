using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;
using UnityEngine.SceneManagement;

public class MainScreenController : MonoBehaviour
{
    //public Image QRTestImage;
    public BoxList BoxList;

    private Texture2D _boxPhotoTexture;

	public void Awake()
	{
		if (!AppManager.Instance.Initialized)
		{
			AppManager.Instance.Initialize();
		}

        EventManager.OnBoxesDataChangedEvent += OnCollectionDataChanged;
	}

    public void OnDestroy()
    {
        EventManager.OnBoxesDataChangedEvent -= OnCollectionDataChanged;
    }

    public void Start()
    {
        LoadCollection();
    }

    private void LoadCollection()
    {
        AppManager.Instance.Backend.Database.LoadCollection(AppManager.Instance.User.collection_id, OnCollectionLoaded);
    }

    private void OnCollectionLoaded(CollectionData data)
    {
        if (data != null)
        {
            //subscribe to collection data update event
            StartListeningToCollectionUpdates();
        }
        else
        {
            CollectionData collection = new CollectionData(PlayerPrefs.GetString("email"));
            AppManager.Instance.Backend.Database.CreateNewCollection(AppManager.Instance.User.collection_id, collection, OnCollectionCreated);
        }
    }

    private void OnCollectionCreated(bool success)
    {
        if (success)
        {
            StartListeningToCollectionUpdates();
        }
        else
        {
            Invoke("LoadCollection", 2.0f);
        }
    }

    private void StartListeningToCollectionUpdates()
    {
        AppManager.Instance.Backend.Database.StartCollectionListener(AppManager.Instance.User.collection_id);
    }

    private void OnCollectionDataChanged(EventData e)
    {
        Debug.Log("Collection data changed! Updating UI...");
        DataSnapshot dataList = (DataSnapshot)e.Data["box_list"];
        Debug.Log(dataList.GetRawJsonValue());
        CollectionData collection = JsonUtility.FromJson<CollectionData>(dataList.GetRawJsonValue());
        BoxList.UpdateList(collection);
    }

    private void OnBoxQRCodeScanned(string qrCode)
	{
        int boxNumber = Convert.ToInt32(qrCode);

        BoxData boxData = new BoxData();
        boxData.box_index = boxNumber;
        boxData.photo_url = BoxData.EMPTY_PHOTO_URL;

        Texture2D boxPhoto = null;

        foreach(var box in BoxList.BoxItemsList)
        {
            if (box.GetData().box_index == boxNumber)
            {
                boxData = box.GetData();
                boxPhoto = box.PhotoImage.sprite.texture;
                continue;
            }
        }

        //show popup of scanned item
        BoxDetailsPopupWindow.ShowPopup(boxData, boxPhoto, null);
	}

    public void OnScanBoxButtonPressed()
    {
        ScanQRCodePopupWindow.ShowPopup(OnBoxQRCodeScanned);
    }

    public void OnPrintCodesButtonPressed()
    {
        AppManager.Instance.Backend.Database.StopCollectionListener(AppManager.Instance.User.collection_id);
        SceneManager.LoadScene("CodesGeneratorScene");
    }

    public void OnXViewerButtonPressed()
    {
        XViewerWindow.ShowPopup(BoxList);
    }
}
