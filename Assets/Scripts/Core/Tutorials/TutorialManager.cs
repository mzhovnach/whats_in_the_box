using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tutorials
{
    PrintCodesTutorial = 0
}

public class TutorialManager {

    public bool IsTutorialShowed(Tutorials tutorial)
    {
        return PlayerPrefs.GetInt(string.Format("TUTORIAL_{0}", (int)tutorial), 0) == 1;
    }

    public void SetTutorialShowed(Tutorials tutorial)
    {
        PlayerPrefs.SetInt(string.Format("TUTORIAL_{0}", (int)tutorial), 1);
    }

    public void ShowTutorial(Tutorials tutorial)
    {
        if (!IsTutorialShowed(tutorial))
        {
            EventData eventData = new EventData("OnShowTutorialEvent");
            eventData.Data["Tutorial"] = tutorial;
            AppManager.Instance.EventManager.CallOnShowTutorialEvent(eventData);
        }
    }
}
