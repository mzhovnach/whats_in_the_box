using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class BoxData
{
    public static string EMPTY_PHOTO_URL = "https://firebasestorage.googleapis.com/v0/b/watsinthebox-d368f.appspot.com/o/resources%2Fempty_box.png?alt=media&token=cb8f38c9-dbbd-4f15-b464-a0f68180b306";
	public int box_index;
	public string photo_url;
    public string photo_date;
}
