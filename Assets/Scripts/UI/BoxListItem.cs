using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoxListItem : MonoBehaviour {

    public Image PhotoImage;
    public Text NumberText;

    private BoxData _data;
    private Vector2 _startClickPosition;
    private float _minDragDistance = 20.0f;

    public void InitWithData(BoxData data)
    {
        _data = data;
        NumberText.text = data.box_index.ToString("D4");
        Texture2D photoFromCache = AppManager.Instance.PhotoCache.LoadPhotoFromCache(data.box_index, data.photo_date);
        if (photoFromCache != null)
        {
            SetPhotoTexture(photoFromCache);
        }
        else
        {
            StartCoroutine(LoadPhotoCo(data.photo_url));
        }
    }

    public BoxData GetData()
    {
        return _data;
    }

    private void SetPhotoTexture(Texture2D texture)
    {
        PhotoImage.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    private IEnumerator LoadPhotoCo(string photoUrl)
    {
        WWW www = new WWW(photoUrl);
        yield return www;
        if (string.IsNullOrEmpty(www.error) && www.texture != null)
        {
            SetPhotoTexture(www.texture);
            // update cache with new photo
            AppManager.Instance.PhotoCache.AddPhotoToCache(_data.box_index, _data.photo_date, www.texture);
        }
        else
        {
            Debug.Log("Error loading box photo: " + www.error);
        }
    }

    public void OnClick()
    {
        BoxDetailsPopupWindow.ShowPopup(_data, PhotoImage.sprite.texture, null);
    }

    public void OnPointerDown()
    {
        _startClickPosition = Input.mousePosition;
    }

    public void OnPointerUp()
    {
        if (Vector2.Distance(Input.mousePosition, _startClickPosition) < _minDragDistance)
        {
            OnClick();
        }
    }
}
