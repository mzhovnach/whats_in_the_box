using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iPhoneXPanelSwitch : MonoBehaviour {

    public GameObject OriginalObject;
    public GameObject iPhoneXObject;

    // Use this for initialization
    void Awake()
    {
#if UNITY_IOS
        if (Screen.width == 1125 && Screen.height == 2436)
        {
            Switch(true);
        }
        else
        {
            Switch(false);
        }
#else
        Switch(false);
#endif
    }	

    private void Switch(bool iPhoneX)
    {
        OriginalObject.SetActive(!iPhoneX);
        iPhoneXObject.SetActive(iPhoneX);
    }
}
