using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// box will be created in 2 steps
// 1. Upload image of the box to storage and get URL
// 2. Post data to database

public class CreateBoxTask
{
    private int _boxNumber;
    private string _collectionId;
    private Texture2D _boxPhoto;
    UnityEngine.Events.UnityAction<bool> _callback;

    public CreateBoxTask(int boxNumber, string collectionId, Texture2D boxPhoto, UnityEngine.Events.UnityAction<bool> callback)
    {
        _boxNumber = boxNumber;
        _collectionId = collectionId;
        _boxPhoto = boxPhoto;
        _callback = callback;

        // 1. Upload image of the box to storage and get URL
        string storagePath = string.Format("collections/{0}/box_{1}.jpg", _collectionId, _boxNumber.ToString());
        AppManager.Instance.Backend.Storage.UploadBytes(storagePath, _boxPhoto.EncodeToJPG(50), ImageUploadedCallback);
    }

    private void ImageUploadedCallback(bool success, string url)
    {
        if (success)
        {
            // 2. Post data to database
            BoxData box = new BoxData();
            box.box_index = _boxNumber;
            box.photo_url = url;
            box.photo_date = System.DateTime.Now.ToString();

            //save image in cache
            AppManager.Instance.PhotoCache.AddPhotoToCache(box.box_index, box.photo_date, _boxPhoto);

            AppManager.Instance.Backend.Database.CreateNewBox(_collectionId, box, BoxCreatedInDatabaseCallback);
        }
        else
        {
            _callback(false);
        }
    }

    private void BoxCreatedInDatabaseCallback(bool success)
    {
        _callback(success);
    }
}
