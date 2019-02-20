using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine;

public class DatabaseHelper : MonoBehaviour {

	private BoxData _boxToUpdate;

    public void Initialize()
	{
		FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl("https://watsinthebox-d368f.firebaseio.com/");
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);		
	}

    public void StartCollectionListener(string collectionID)
    {
        Debug.Log("Starting listening to collection " + collectionID);
        Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.Child("collections").Child(collectionID)
                .ValueChanged += OnCollectionChangedCallback;
    }

    public void StopCollectionListener(string collectionID)
    {
        Debug.Log("Stop listening to collection " + collectionID);
        Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.Child("collections").Child(collectionID)
                .ValueChanged -= OnCollectionChangedCallback;
    }

    private void OnCollectionChangedCallback(object sender2, ValueChangedEventArgs e2) 
    {
        if (e2.DatabaseError != null)
        {
            Debug.LogError(e2.DatabaseError.Message);
            return;
        }
        Debug.Log("Received values for Boxes.");
        if (e2.Snapshot != null && e2.Snapshot.ChildrenCount > 0)
        {
            EventData eChangedDataEvent = new EventData("");
            eChangedDataEvent.Data["box_list"] = e2.Snapshot;
            AppManager.Instance.EventManager.CallOnBoxesDataChangedEvent(eChangedDataEvent);
        }
    }
    
	public void AddBox(BoxData boxData)
    {        
		string json = JsonUtility.ToJson(boxData);
		Firebase.Database.DatabaseReference dbRef = Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference;
        dbRef.Child("users").Push().SetRawJsonValueAsync(json);
    }

	public void LoadUserData(string userId, UnityEngine.Events.UnityAction<UserData> callback)
    {
		Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                callback(null);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (snapshot.HasChildren)
                    {
                        Debug.Log("UserData found. Loading user..");
                        string str = snapshot.GetRawJsonValue();
						UserData user = new UserData();
						user = JsonUtility.FromJson<UserData>(str);
                        AppManager.Instance.User = user;
                        callback(user);
                    }
                    else
                    {
                        Debug.Log("UserData empty");
                        callback(null);
                    }
                }
                else
                {
                    Debug.Log("User not exists");
                    callback(null);
                }
            }
        });
    }

	public void CreateNewUser(string userID, UserData userData, UnityEngine.Events.UnityAction<bool> callback)
	{
		FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(userID).SetRawJsonValueAsync(JsonUtility.ToJson(userData)).ContinueWith(task => {
            if (task.IsFaulted)
            {
				callback(false);
            }
            else
            {
                callback(true);            
            }
        });
	}

    public void LoadCollection(string collectionID, UnityEngine.Events.UnityAction<CollectionData> callback)
	{
        Firebase.Database.FirebaseDatabase.DefaultInstance.RootReference.Child("collections").Child(collectionID).GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                callback(null);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    if (snapshot.HasChildren)
                    {
                        Debug.Log("Collection found. Loading collection..");
                        string str = snapshot.GetRawJsonValue();
                        Debug.Log(str);
                        CollectionData collection = JsonUtility.FromJson<CollectionData>(str);
                        callback(collection);
                    }
                    else
                    {
                        Debug.Log("Collection empty");
                        callback(null);
                    }
                }
                else
                {
                    Debug.Log("Collection not exist");
                    callback(null);
                }
            }
        });
	}

    public void CreateNewCollection(string collectionID, CollectionData collectionData, UnityEngine.Events.UnityAction<bool> callback)
    {
        FirebaseDatabase.DefaultInstance.RootReference.Child("collections").Child(collectionID).SetRawJsonValueAsync(JsonUtility.ToJson(collectionData)).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Error creating new collection");
                callback(false);
            }
            else
            {
                Debug.Log("Collection created succesfull");
                callback(true);
            }
        });
    }

    public void CreateNewBox(string collectionID, BoxData boxData, UnityEngine.Events.UnityAction<bool> callback)
    {
        string path = string.Format("collections/{0}/boxes/{1}", collectionID, boxData.box_index - 1);
        var boxRef = FirebaseDatabase.DefaultInstance.GetReference(path);
        boxRef.SetRawJsonValueAsync(JsonUtility.ToJson(boxData)).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Error creating new box");
                callback(false);
            }
            else
            {
                Debug.Log("Box created succesfull");
                callback(true);
            }
        });
    }

    public void RemoveBox(string collectionID, BoxData boxData, UnityEngine.Events.UnityAction<bool> callback)
    {
        string path = string.Format("collections/{0}/boxes/{1}", collectionID, boxData.box_index - 1);
        var boxRef = FirebaseDatabase.DefaultInstance.GetReference(path);
        boxRef.RemoveValueAsync().ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.Log("Error removing box");
                callback(false);
            }
            else
            {
                Debug.Log("Box removed succesfull");
                callback(true);
            }
        });
    }
}
