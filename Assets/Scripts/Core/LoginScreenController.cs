using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoginScreenController : MonoBehaviour
{
	public InputField EmailInput;
	public InputField PassInput;
    public InputField ConfirmPassInput;
	public Text ErrorText;
	public Button SignInButton;
	public Button SignUpButton;

    public GameObject LoginGroup;
    public GameObject SignUpGroup;

	private void Awake()
	{
		if (!AppManager.Instance.Initialized)
        {
            AppManager.Instance.Initialize();
        }
	}
	// Use this for initialization
	void Start () {
        if (IsLoggedIn())
        {
        	EmailInput.text = PlayerPrefs.GetString("email", "");
        	PassInput.text = PlayerPrefs.GetString("pass", "");
            OnSwithLoginButtonPressed();
            TryAutoLogin();
        }
        //CollectionData collection = new CollectionData("mykhailo.zhovnach@gmail.com");
        //for (int i = 1; i <= 2; i++)
        //{
        //    BoxData box = new BoxData();
        //    box.box_index = i;
        //    box.photo_url = BoxData.EMPTY_PHOTO_URL;
        //    collection.boxes.Add("box_" + i, box);
        //};

        //Debug.Log(JsonUtility.ToJson(collection));
	}

    private void TryAutoLogin()
    {
        if (AppManager.Instance.Backend.Initialized)
        {
            OnSignInButtonPressed();
        }
        else
        {
            Invoke("TryAutoLogin", 1.0f);
        }
    }

    bool IsLoggedIn()
	{
        return PlayerPrefs.HasKey("email");		
	}

	private void SetInputEnabled(bool enabled)
	{
		SignInButton.interactable = enabled;
		SignUpButton.interactable = enabled;
	}
    

    public void OnSignInButtonPressed()
	{
        PlayerPrefs.SetString("email", EmailInput.text);
        PlayerPrefs.SetString("pass", PassInput.text);
		SetInputEnabled(false);
        AppManager.Instance.Backend.Authentification.OnSignInCompleted = SignInCompleteCallback;
        AppManager.Instance.Backend.Authentification.SigninWithCredentialAsync(EmailInput.text, PassInput.text);
		ErrorText.text = "";
	}

	private void SignInCompleteCallback(bool error, string errorText)
	{
        AppManager.Instance.Backend.Authentification.OnSignInCompleted = null;
		SetInputEnabled(true);
		if (error)
        {
            ErrorText.text = errorText;
        }
        else
        {
            //loading user data
            AppManager.Instance.Backend.Database.LoadUserData(PlayerPrefs.GetString("user_id"), OnUserDataLoaded);
        }
	}

	public void OnSignUpButtonPressed()
    {
		SetInputEnabled(false);
        AppManager.Instance.Backend.Authentification.OnSignUpCompleted = SignUpCompleteCallback;
        AppManager.Instance.Backend.Authentification.CreateUserAsync(EmailInput.text, PassInput.text);
		ErrorText.text = "";
    }

	private void SignUpCompleteCallback(bool error, string errorText)
    {
        AppManager.Instance.Backend.Authentification.OnSignUpCompleted = null;
		SetInputEnabled(true);
		if (error)
        {
            ErrorText.text = errorText;
        }
		else
		{			
			//TransitToMainScreen();
		}
    }

	private void OnUserDataLoaded(UserData data)
	{
		if (data == null)
		{
			UserData userData = new UserData();
			userData.collection_id = PlayerPrefs.GetString("user_id");
			//creating new user
			AppManager.Instance.Backend.Database.CreateNewUser(PlayerPrefs.GetString("user_id"), userData, OnUserCreated);
		}
		else
		{
			TransitToMainScreen();
		}
	}

	private void OnUserCreated(bool success)
	{
        if (success)
        {
            TransitToMainScreen();
        }
	}

	private void TransitToMainScreen()
	{
        if (AppManager.Instance.Tutorials.IsTutorialShowed(Tutorials.PrintCodesTutorial))
        {
            SceneManager.LoadScene("MainScene");
        } else
        {
            SceneManager.LoadScene("CodesGeneratorScene");
        }
	}

    public void OnSwithLoginButtonPressed()
    {
        LoginGroup.SetActive(true);
        SignUpGroup.SetActive(false);
    }

    public void OnSwitchSignUpButtonPressed()
    {
        LoginGroup.SetActive(false);
        SignUpGroup.SetActive(true);
    }
}
