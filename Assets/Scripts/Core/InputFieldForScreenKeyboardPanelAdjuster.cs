using UnityEngine;
using UnityEngine.UI;

public class InputFieldForScreenKeyboardPanelAdjuster : MonoBehaviour
{
    private const float ANIM_TIME = 0.2f;
    // Assign panel here in order to adjust its height when TouchScreenKeyboard is shown
    public GameObject panel;

    private InputField _inputField;
    private RectTransform _panelRectTrans;
    private float _currentKeyboardHeightRatio;
    private float _designHeight = 1920.0f;
    private bool _waitForActivation;

    public void Start()
    {
        //_inputField = transform.GetComponent<InputField>();
        _panelRectTrans = panel.GetComponent<RectTransform>();
        AppManager.Instance.KeyboardAdjuster = this;
    }

    public void OnDestroy()
    {
        AppManager.Instance.KeyboardAdjuster = null;
    }

    public void SetActiveInputField(InputField field)
    {
        Debug.Log(field.gameObject.name + " " + field.isFocused.ToString());
        _inputField = field;
        _waitForActivation = true;
    }

    public void LateUpdate()
    {
        if (_waitForActivation)
        {
            if (_inputField.isFocused)
            {
                _waitForActivation = false;
            }
            else
            {
                return;
            }
        }
        if (_inputField != null && _inputField.isFocused)
        {
            float newKeyboardHeightRatio = GetKeyboardHeightRatio();
            if (_currentKeyboardHeightRatio != newKeyboardHeightRatio)
            {
                Debug.Log("InputFieldForScreenKeyboardPanelAdjuster: Adjust to keyboard height ratio: " + newKeyboardHeightRatio);
                _currentKeyboardHeightRatio = newKeyboardHeightRatio;
                //_panelRectTrans.anchoredPosition3D = new Vector3(0.0f, _designHeight * newKeyboardHeightRatio, 0.0f);
                LeanTween.value(gameObject, _panelRectTrans.anchoredPosition3D, new Vector3(0.0f, _designHeight * newKeyboardHeightRatio, 0.0f), ANIM_TIME)
                         .setEase(LeanTweenType.easeInOutSine)
                         .setOnUpdate((Vector3 pos) =>
                         {
                             _panelRectTrans.anchoredPosition3D = pos;
                         });
            }
        }
        else if (_currentKeyboardHeightRatio != 0f)
        {
            if (_panelRectTrans.anchoredPosition3D != Vector3.zero)
            {
                LeanTween.delayedCall(0.01f, () =>
                {
                    Debug.Log("InputFieldForScreenKeyboardPanelAdjuster: Revert to original");
                    //_panelRectTrans.anchoredPosition3D = Vector3.zero;
                    LeanTween.value(gameObject, _panelRectTrans.anchoredPosition3D, Vector3.zero, ANIM_TIME)
                             .setEase(LeanTweenType.easeOutSine)
                         .setOnUpdate((Vector3 pos) =>
                         {
                             _panelRectTrans.anchoredPosition3D = pos;
                         });
                });
            }
            _currentKeyboardHeightRatio = 0f;
        }
    }

    private float GetKeyboardHeightRatio()
    {
        if (Application.isEditor)
        {
            return 0.4f; // fake TouchScreenKeyboard height ratio for debug in editor        
        }
#if UNITY_ANDROID        
        using (AndroidJavaClass UnityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject View = UnityClass.GetStatic<AndroidJavaObject>("currentActivity").Get<AndroidJavaObject>("mUnityPlayer").Call<AndroidJavaObject>("getView");
            using (AndroidJavaObject rect = new AndroidJavaObject("android.graphics.Rect"))
            {
                View.Call("getWindowVisibleDisplayFrame", rect);
                return ((float)(Screen.height - rect.Call<int>("height")) * 1.2f) / Screen.height;
            }
        }
#else
        return (float)TouchScreenKeyboard.area.height / Screen.height;
#endif
    }
}