using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this data is saved in Firebase database and will be loaded on login

[System.Serializable]
public class UserData {

	public string collection_id;

    public UserData()
    {
        collection_id = "";
    }
}
