using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonBlocker : MonoBehaviour
{
    public string myAction;
    public Button myButton;
    public bool defaultState = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (myButton!=null) {
            if (Tutorial.inTutorial)
            {
                if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == myAction)
                {
                    myButton.enabled = defaultState;
                }
                else
                {
                    myButton.enabled = false;
                }
                if (myAction == "capture")
                {
                    if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction.Contains("evidence"))
                    {
                        myButton.enabled = defaultState;
                    }
                    else
                    {
                        myButton.enabled = false;
                    }
                }
            }
            else
            {
                myButton.enabled = defaultState;
            }
        }
    }
    public void ButtonPress()
    {
        Tutorial.AdvanceTutorial();
        myButton.enabled = false;
    }
}
