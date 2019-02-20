using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCameraScaler : MonoBehaviour
{
    private Camera _camera;
    
    // Use this for initialization
    void Start()
    {
        if (_camera == null)
        {
            _camera = transform.GetComponent<Camera>();
        }
        
#if UNITY_IOS
        if (Screen.width == 1125 && Screen.height == 2436)
        {
            _camera.orthographicSize = 35.0f;
        }
#endif
    }    
}
