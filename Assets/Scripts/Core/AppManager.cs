using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppManager : Singleton<AppManager>
{
    public Backend Backend;
	public EventManager EventManager;
	public PopupWindowController PopupController;
    public UserData User;
    public ZagravaNativeShare NativeShare;
    public PhotoCache PhotoCache;
    public TutorialManager Tutorials;
    public InputFieldForScreenKeyboardPanelAdjuster KeyboardAdjuster;

    // guarantee this will be always a singleton only - can't use the constructor!
    protected AppManager()
    {

    }
    public bool Initialized;

    public void Initialize()
    {
		EventManager = new EventManager();
		Backend = gameObject.AddComponent<Backend>();
        Backend.InitializeFirebase();
        NativeShare = gameObject.AddComponent<ZagravaNativeShare>();
        PhotoCache = new PhotoCache();
        PhotoCache.LoadCache();
        Tutorials = new TutorialManager();
        Initialized = true;
    }
}