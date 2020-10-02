using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Unity.Editor;
using System;
using System.Threading.Tasks;
using Firebase.Database;

public class Backend : MonoBehaviour
{
    [NonSerialized]
    public DatabaseHelper Database;
    [NonSerialized]
    public StorageHelper Storage;
    [NonSerialized]
    public AuthHelper Authentification;
    public bool Initialized;

	public async System.Threading.Tasks.Task InitializeFirebase()
    {
        Initialized = false;
        Firebase.DependencyStatus dependencyStatus = await Firebase.FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == Firebase.DependencyStatus.Available)
        {
            Debug.Log("InitializeFirebase: Available!");
            // Set a flag here indiciating that Firebase is ready to use by your
            // application.
            LateInitialize();
            Initialized = true;
        }
        else
        {
            UnityEngine.Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
    }

    private void LateInitialize()
    {
        Debug.Log("Setting up Firebase Auth");

        Authentification = gameObject.AddComponent<AuthHelper>();
        Authentification.Initialize();

        Database = gameObject.AddComponent<DatabaseHelper>();
        Database.Initialize();

        Storage = gameObject.AddComponent<StorageHelper>();
        Storage.Initialize();
    }
}
