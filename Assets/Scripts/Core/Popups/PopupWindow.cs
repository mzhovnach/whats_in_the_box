using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindow : MonoBehaviour {

    private CanvasGroup _canvasGroup;    

    public void Init()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public virtual void Show(Dictionary<string, object> data)
    {
        _canvasGroup.alpha = 0.0f;
        gameObject.SetActive(true);
        LeanTween.value(gameObject, 0.0f, 1.0f, PopupWindowController.POPUP_SHOW_HIDE_ANIM_TIME)
           .setOnUpdate((float alpha) =>
           {
               _canvasGroup.alpha = alpha;
           });
        gameObject.transform.localScale = new Vector3(0.9f, 0.9f, 1.0f);
        LeanTween.scale(gameObject, Vector3.one, PopupWindowController.POPUP_SHOW_HIDE_ANIM_TIME)
            .setEase(LeanTweenType.easeInOutBack);
    }

    public virtual void OnHideComplete()
    {

    }

    public void Hide()
    {
        LeanTween.value(gameObject, _canvasGroup.alpha, 0.0f, PopupWindowController.POPUP_SHOW_HIDE_ANIM_TIME)
           .setOnUpdate((float alpha) =>
           {
               _canvasGroup.alpha = alpha;
           })
           .setOnComplete(() =>
          {
              OnHideComplete();
              gameObject.SetActive(false);
          });
        
        LeanTween.scale(gameObject, new Vector3(0.9f, 0.9f, 1.0f), PopupWindowController.POPUP_SHOW_HIDE_ANIM_TIME)
            .setEase(LeanTweenType.easeInOutBack);
    }
}
