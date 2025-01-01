using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Computer : MonoBehaviour
{
    public Controller controlScript;
    public Canvas myCanvas;

    public float timeTillChange;

    public GameObject itemScanningScreen;
    public GameObject activateGhostVisionScreen;
    public GameObject warningScreen;


    // Start is called before the first frame update
    void Start()
    {
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
        myCanvas.worldCamera = Camera.main;
        ActivateGhostVision();
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void ActivateGhostVision()
    {
        controlScript.ghostVisionActive = true;
        activateGhostVisionScreen.SetActive(false);
        warningScreen.SetActive(true);
    }
}
