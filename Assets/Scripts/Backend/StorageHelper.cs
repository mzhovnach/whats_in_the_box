using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Firebase;
using Firebase.Storage;
using UnityEngine;

public class StorageHelper : MonoBehaviour
{
    protected FirebaseStorage storage;
    protected string MyStorageBucket = "gs://watsinthebox-d368f.appspot.com/";
    protected static string UriFileScheme = Uri.UriSchemeFile + "://";

    public void Initialize()
    {
        var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
        storage = FirebaseStorage.DefaultInstance;
        if (!String.IsNullOrEmpty(appBucket))
        {
            MyStorageBucket = String.Format("gs://{0}/", appBucket);
        }
        storage.LogLevel = LogLevel.Debug;
    }

    // Retrieve a storage reference from the user specified path.
    protected StorageReference GetStorageReference(string storageLocation)
    {
        // If this is an absolute path including a bucket create a storage instance.
        if (storageLocation.StartsWith("gs://") ||
            storageLocation.StartsWith("http://") ||
            storageLocation.StartsWith("https://"))
        {
            var storageUri = new Uri(storageLocation);
            var firebaseStorage = FirebaseStorage.GetInstance(
              String.Format("{0}://{1}", storageUri.Scheme, storageUri.Host));
            return firebaseStorage.GetReferenceFromUrl(storageLocation);
        }
        // When using relative paths use the default storage instance which uses the bucket supplied
        // on creation of FirebaseApp.
        return FirebaseStorage.DefaultInstance.GetReference(storageLocation);
    }

    // Upload file text to Cloud Storage using a byte array.
    public void UploadBytes(string path, byte[] data, UnityEngine.Events.UnityAction<bool, string> callback)
    {
        var storageReference = GetStorageReference(path);
        // Create file metadata including the content type
        var new_metadata = new Firebase.Storage.MetadataChange();
        new_metadata.ContentType = "image/jpeg";
        Debug.Log(String.Format("Uploading to {0} ...", storageReference.Path));
        storageReference.PutBytesAsync(data, new_metadata, null, CancellationToken.None, null)
           .ContinueWith((Task<StorageMetadata> task) =>
           {
               if (task.IsFaulted || task.IsCanceled)
               {
                   Debug.Log(task.Exception.ToString());
                   // Uh-oh, an error occurred!
                   callback(false, "");
               }
               else
               {
                   storageReference.GetDownloadUrlAsync().ContinueWith((Task<Uri> taskUri) =>
                   {
                       //// Metadata contains file metadata such as size, content-type, and download URL.
                       //Firebase.Storage.StorageMetadata metadata = task.Result;
                       //string download_url = metadata.DownloadUrl.ToString();
                       //Debug.Log("Finished uploading...");
                       //callback(true, download_url);
                       if (task.IsFaulted || task.IsCanceled)
                       {
                           callback(false, "");
                       }
                       else
                       {
                           Uri uri = taskUri.Result;
                            callback(true, uri.ToString());
                       }
                   });
               }
           });
    }
}
