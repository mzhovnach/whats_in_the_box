using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectionData
{
    public List<BoxData> boxes;
    public string owner;

    public CollectionData(string ownerSet)
    {
        owner = ownerSet;
        boxes = new List<BoxData>();

        //create at least 2 boxes, so this field will be treated as array
        for (int i = 1; i <= 2; i++)
        {
            BoxData box = new BoxData();
            box.box_index = i;
            box.photo_url = BoxData.EMPTY_PHOTO_URL;
            boxes.Add(box);
        } 
    }
}
