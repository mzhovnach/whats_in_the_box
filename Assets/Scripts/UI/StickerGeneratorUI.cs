using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StickerGeneratorUI : MonoBehaviour {

    public StickerFrameGenerator StickerGenerator;
    public InputField FromInput;
    public InputField ToInput;
    public Camera RenderCamera;

    public void Awake()
    {
        if (!AppManager.Instance.Initialized)
        {
            AppManager.Instance.Initialize();
        }
    }

    public void Start()
    {
        OnGenerateButtonPressed();
        Invoke("ShowTuturials", 1.0f);
    }

    private void ShowTuturials()
    {
        AppManager.Instance.Tutorials.ShowTutorial(Tutorials.PrintCodesTutorial);
    }

    public void OnSendButtonPressed()
    {
        StartCoroutine("OnSendButtonPressedCo");
    }

    private IEnumerator OnSendButtonPressedCo()
    {
        RenderCamera.gameObject.SetActive(true);
        yield return new WaitForEndOfFrame();
        //save image
        RenderTexture renderTexture = RenderCamera.targetTexture;
        RenderTexture.active = renderTexture;
        Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
        texture.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        byte[] bytes;
        bytes = texture.EncodeToPNG();
        string fileName = Application.persistentDataPath + "/qr_codes.png";
        File.Delete(fileName);
        File.WriteAllBytes(fileName, bytes);
        RenderCamera.gameObject.SetActive(false);
        AppManager.Instance.NativeShare.Share("Print this QR codes, cut and put them on boxes.", fileName, "");
    }

    public void OnGenerateButtonPressed()
    {
        StickerGenerator.GenerateStickers(Convert.ToInt16(FromInput.text), Convert.ToInt16(ToInput.text));
        AppManager.Instance.Tutorials.ShowTutorial(Tutorials.PrintCodesTutorial);
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene("MainScene");
    }
}
