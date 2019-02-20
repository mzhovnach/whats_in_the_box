using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindowController : MonoBehaviour {

    public const float POPUP_SHOW_HIDE_ANIM_TIME = 0.2f;
    public const float POPUP_FADER_ALPHA = 0.8f;

    [SerializeField]
    private PopupWindow[] PopupWindows;
    [SerializeField]
    private Image Fader;
    private PopupWindow _activePopupWindow;

    void Awake()
    {
		AppManager.Instance.PopupController = this;
        EventManager.OnShowPopupWindowEvent += OnShowPopupWindowEvent;
        EventManager.OnHidePopupWindowEvent += OnHidePopupWindowEvent;        
    }

    void OnDestroy()
    {        
        EventManager.OnShowPopupWindowEvent -= OnShowPopupWindowEvent;
        EventManager.OnHidePopupWindowEvent -= OnHidePopupWindowEvent;
    }    

    public T GetActivePopup<T>() where T : PopupWindow
    {
        return (_activePopupWindow as T);
    }

    private void OnShowPopupWindowEvent(EventData e)
    {
        //GameManager.Instance.BackgroundServerDataCheck.Paused = true;
        //bool showFader = !e.Data.ContainsKey("nofader");
        //if (showFader)
        //{
        //    SetFaderAlpha(0.0f);
        //    Fader.gameObject.SetActive(true);
        //    LeanTween.value(Fader.gameObject, 0.0f, POPUP_FADER_ALPHA, POPUP_SHOW_HIDE_ANIM_TIME)
        //        .setOnUpdate((float alpha) =>
        //       {
        //           SetFaderAlpha(alpha);
        //       })
        //       .setEase(LeanTweenType.easeOutSine);
        //}
        Type popupType = (Type)e.Data["Type"];
        ShowPopupScreen(popupType, e.Data);
    }

    private void OnHidePopupWindowEvent(EventData e)
    {
        //if (Fader.color.a > 0)
        //{
        //    Fader.gameObject.SetActive(true);
        //    LeanTween.value(Fader.gameObject, POPUP_FADER_ALPHA, 0.0f, POPUP_SHOW_HIDE_ANIM_TIME)
        //        .setOnUpdate((float alpha) =>
        //        {
        //            SetFaderAlpha(alpha);
        //        })
        //        .setOnComplete(() =>
        //       {
        //           Fader.gameObject.SetActive(false);
        //       })
        //       .setEase(LeanTweenType.easeInSine);
        //}
        Type popupType = (Type)e.Data["Type"];
        HidePopupScreen(popupType);
    }

    private void SetFaderAlpha(float alpha)
    {
        Fader.color = new Color(0, 0, 0, alpha);
    }

    public void ShowPopupScreen(Type screenType, Dictionary<string, object> data)
    {
        //if (_activePopupWindow != null)
        //{
        //    _activePopupWindow.gameObject.SetActive(false);
        //}

        foreach (var popup in PopupWindows)
        {
            if (popup.GetType() == screenType)
            {                
                popup.Show(data);
                _activePopupWindow = popup;
                break;
            }
        }
    }

    public void HidePopupScreen(Type screenType)
    {
        foreach (var popup in PopupWindows)
        {
            if (popup.GetType() == screenType)
            {
                popup.Hide();
                _activePopupWindow = null;
                break;
            }
        }
    }

    void Start()
    {
        //Fader.gameObject.SetActive(false);
        _activePopupWindow = null;
        foreach (var popup in PopupWindows)
        {
            popup.Init();
            popup.gameObject.SetActive(false);
        }
    }

    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Q))
    //    {
    //        EventData e = new EventData("OnShowPopupWindowEvent");
    //        e.Data["Type"] = typeof(DoctorWantsPopupWindow);
    //        OnShowPopupWindowEvent(e);            
    //    }

    //    if (Input.GetKeyDown(KeyCode.W))
    //    {
    //        EventData e = new EventData("OnShowPopupWindowEvent");
    //        e.Data["Type"] = typeof(RateDoctorPopupWindow);
    //        OnShowPopupWindowEvent(e);
    //    }
    //}    
}
