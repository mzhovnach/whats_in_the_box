using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PhotoCache {

    private const string CACHE_DATA_FILE = "/data.jsn";

    [Serializable]
    class PhotoCacheData
    {
        [Serializable]
        public class PhotoData
        {
            public int box_index;
            public string filename;
            public string date;
        }

        public List<PhotoData> photos = new List<PhotoData>();
    }

    private PhotoCacheData _cache = new PhotoCacheData();

    public void LoadCache()
    {
        if (!Directory.Exists(GetCacheDir()))
        {
            Directory.CreateDirectory(GetCacheDir());
        }
        if (File.Exists(GetCacheDir() + CACHE_DATA_FILE))
        {
            byte[] fileDataBytes = File.ReadAllBytes(GetCacheDir() + CACHE_DATA_FILE);
            string jsonData = System.Text.Encoding.UTF8.GetString(fileDataBytes);
            _cache = JsonUtility.FromJson<PhotoCacheData>(jsonData);
        }
        else
        {
            _cache = new PhotoCacheData();
        }
    }

    public void SaveCache()
    {
        string jsonData = JsonUtility.ToJson(_cache);
        File.WriteAllText(GetCacheDir() + CACHE_DATA_FILE, jsonData);
    }

    private string GetCacheDir()
    {
        return Application.persistentDataPath + "/photo_cache";
    }

    public void AddPhotoToCache(int box_index, string date, Texture2D photo)
    {
        PhotoCacheData.PhotoData data = null;
        foreach (var photoData in _cache.photos)
        {
            if (photoData.box_index == box_index)
            {
                data = photoData;
                break;
            }
        }

        if (data == null)
        {
            data = new PhotoCacheData.PhotoData();
            data.box_index = box_index;
            _cache.photos.Add(data);
        }

        data.date = date;
        //save image to file
        var bytes = photo.EncodeToJPG();
        string fileName = "box_" + box_index + ".jpg";
        string filePath = GetCacheDir() + "/" + fileName;
        var file = File.Open(filePath, FileMode.Create);
        var binary = new BinaryWriter(file);
        binary.Write(bytes);
        file.Close();
        data.filename = fileName;
        SaveCache();
    }

    public Texture2D LoadPhotoFromCache(int box_index, string date)
    {
        Texture2D photoTexture = null;
        foreach(var photoData in _cache.photos)
        {
            if (photoData.box_index == box_index && photoData.date == date)
            {
                string filePath = GetCacheDir() + "/" + photoData.filename;
                //load local file here
                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    photoTexture = new Texture2D(2, 2);
                    photoTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
                }
            }
        }
        return photoTexture;
    }
}
