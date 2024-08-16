using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Handprint : MonoBehaviour
{
    public MeshRenderer myRender;
    public GameObject detection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Tutorial.inTutorial && Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction == "evidencehandprint")
        {
            Color tempColor = myRender.material.color;
            tempColor.a = Mathf.MoveTowards(tempColor.a, 1f, 10f * Time.deltaTime);
            myRender.material.color = tempColor;
        }
        else
        {
            Color tempColor = myRender.material.color;
            tempColor.a = Mathf.MoveTowards(tempColor.a, 0f, 5f * Time.deltaTime);
            myRender.material.color = tempColor;
        }
        if (myRender.material.color.a>0.02f) {
            detection.SetActive(true);
        }
        else
        {
            detection.SetActive(false);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Equipment")
        {
            Equipment equipScript = other.GetComponentInParent<Equipment>();
            if (equipScript.uv)
            {
                Color tempColor = myRender.material.color;
                tempColor.a = Mathf.MoveTowards(tempColor.a,1f,10f*Time.deltaTime);
                myRender.material.color = tempColor;
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Equipment")
        {
            Equipment equipScript = other.GetComponentInParent<Equipment>();
            if (equipScript.uv)
            {
                equipScript.controlScript.revealingHandprint = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Equipment")
        {
            Equipment equipScript = other.GetComponentInParent<Equipment>();
            if (equipScript.uv)
            {
                equipScript.controlScript.revealingHandprint = false;
            }
        }
    }
}
