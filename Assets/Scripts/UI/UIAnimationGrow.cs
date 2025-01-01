using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationGrow : MonoBehaviour
{
    public Vector3 startSize;
    public Vector3 differenceSize;
    public Controller controlScript;

    // Start is called before the first frame update
    void Awake()
    {
        startSize = transform.localScale;
        controlScript = GameObject.Find("Controller").GetComponent<Controller>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale,startSize-(differenceSize*0.2f),10f*Time.deltaTime);
    }
    private void OnEnable()
    {
        if (controlScript.mainUI.currentMenu=="camera"|| controlScript.mainUI.currentMenu == "video"|| controlScript.mainUI.currentMenu == "sound") {
            differenceSize = new Vector3(0f,0f,0f);
        }
        else
        {
            differenceSize = new Vector3(((float)Screen.width / (float)Screen.height), ((float)Screen.width / (float)Screen.height), ((float)Screen.width / (float)Screen.height));
        }
        transform.localScale = new Vector3(0f,0f,0f);
    }
}
