using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class InputFieldKeyboardSubscriber : MonoBehaviour, ISelectHandler {

    private bool _focused;
    private InputField _input;
	// Use this for initialization
	void Start ()
    {
        //Debug.Log("KEYBOARD: " + gameObject.name + "  " + transform.position.y);
        if (transform.position.y > Screen.height / 2.0f)
        {
            Destroy(this);
        }
        _input = GetComponent<InputField>();
	}
	
    public void OnSelect(BaseEventData eventData)
    {
        AppManager.Instance.KeyboardAdjuster.SetActiveInputField(_input);
    }
}
