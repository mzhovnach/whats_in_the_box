using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AuthHelper : MonoBehaviour {

    protected Firebase.Auth.FirebaseAuth auth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
    new Dictionary<string, Firebase.Auth.FirebaseUser>();
    private string errorStr;
    private string _email;
    private string _password;

    public Action<bool, string> OnSignInCompleted;
    public Action<bool, string> OnSignUpCompleted;

    public void Initialize()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IsTokenChanged;
        AuthStateChanged(this, null);
    }

    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth.IdTokenChanged -= IsTokenChanged;
        auth = null;
    }

    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
                PlayerPrefs.SetString("user_id", user.UserId);
            }
        }
    }

    void IsTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
                task =>
                {
                    Debug.Log(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8)));
                    PlayerPrefs.SetString("email", _email);
                    PlayerPrefs.SetString("pass", _password);
                    PlayerPrefs.SetString("token", task.Result);
                    PlayerPrefs.Save();
                });
        }
    }

    // This is functionally equivalent to the Signin() function.  However, it
    // illustrates the use of Credentials, which can be aquired from many
    // different sources of authentication.
    public Task SigninWithCredentialAsync(string email, string password)
    {
        _email = email;
        _password = password;
        Debug.Log(String.Format("Attempting to sign in as {0}...", email));
        Firebase.Auth.Credential cred = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        return auth.SignInWithCredentialAsync(cred).ContinueWith(HandleSigninResult);
    }

    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        bool result = (LogTaskCompletion(authTask, "Sign-in"));
        if (OnSignInCompleted != null)
        {
            OnSignInCompleted(!result, errorStr);
        }
    }

    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    bool LogTaskCompletion(Task task, string operation)
    {
        errorStr = "";
        bool complete = false;
        if (task.IsCanceled)
        {
            Debug.Log(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            Debug.Log(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                    errorStr = exception.Message;
                }
                Debug.Log(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            Debug.Log(operation + " completed");
            complete = true;
        }
        return complete;
    }

    public Task CreateUserAsync(string email, string password)
    {
        _email = email;
        _password = password;
        Debug.Log(String.Format("Attempting to create user {0}...", email));

        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWith((task) => {
              return HandleCreateUserAsync(task);
          }).Unwrap();
    }

    Task HandleCreateUserAsync(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        bool res = LogTaskCompletion(authTask, "User Creation");
        if (res)
        {
            if (auth.CurrentUser != null)
            {
                Debug.Log(String.Format("User Info: {0}  {1}", auth.CurrentUser.Email,
                                       auth.CurrentUser.ProviderId));
            }
        }

        if (OnSignUpCompleted != null)
        {
            OnSignUpCompleted(!res, errorStr);
        }
        // Nothing to update, so just return a completed Task.
        return Task.FromResult(0);
    }
}
