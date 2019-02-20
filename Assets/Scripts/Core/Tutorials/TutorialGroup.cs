using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGroup : MonoBehaviour {

    private Dictionary<Tutorials, Tutorial> _tutorials;

    private void Awake()
    {
        EventManager.OnShowTutorialEvent += OnShowTutorial;
        _tutorials = new Dictionary<Tutorials, Tutorial>();
    }

    private void OnDestroy()
    {
        EventManager.OnShowTutorialEvent -= OnShowTutorial;
    }

    // Use this for initialization
    void Start()
    {
        foreach(Transform t in transform)
        {
            Tutorial tutorial = t.gameObject.GetComponent<Tutorial>();
            if (tutorial != null)
            {
                tutorial.gameObject.SetActive(false);
                _tutorials.Add(tutorial.TutorialID, tutorial);
            }
        }
    }

    private void OnShowTutorial(EventData e)
    {
        Tutorials tutorial = (Tutorials)e.Data["Tutorial"];
        if (_tutorials.ContainsKey(tutorial))
        {
            Tutorial tutorObj = _tutorials[tutorial];
            AppManager.Instance.Tutorials.SetTutorialShowed(tutorial);
            tutorObj.gameObject.SetActive(true);
        }
    }
}
