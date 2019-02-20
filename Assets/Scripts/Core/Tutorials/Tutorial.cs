using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    public Tutorials TutorialID;	

    public void CloseTutorialButtonPressed()
    {
        gameObject.SetActive(false);
    }
}
