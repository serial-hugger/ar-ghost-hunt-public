using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIdentification : MonoBehaviour
{
    public Renderer myRender;
    public string myAction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction==myAction)
        {
            myRender.enabled = true;
        }
        else
        {
            myRender.enabled = false;
        }
    }
}
