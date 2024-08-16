using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScannerOverlay : MonoBehaviour
{
    public float timeToScan;
    public Material scannerGrid;
    public Material scannerDot;
    public RectTransform scanLine;
    public RectTransform scanDot1;
    public RectTransform scanDot2;
    public bool goingUp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToScan>0f)
        {
            Color tempColor = scannerGrid.color;
            tempColor.a = Mathf.MoveTowards(scannerGrid.color.a,0.8f,1f*Time.deltaTime);
            scannerGrid.color = tempColor;
            tempColor = scannerDot.color;
            tempColor.a = Mathf.MoveTowards(scannerDot.color.a, 1f, 2f * Time.deltaTime);
            scannerDot.color = tempColor;
        }
        else
        {
            Color tempColor = scannerGrid.color;
            tempColor.a = Mathf.MoveTowards(scannerGrid.color.a, 0f, 1f * Time.deltaTime);
            scannerGrid.color = tempColor;
            tempColor = scannerDot.color;
            tempColor.a = Mathf.MoveTowards(scannerDot.color.a, 0f, 2f * Time.deltaTime);
            scannerDot.color = tempColor;
        }
        timeToScan -= 1f * Time.deltaTime;
        scannerGrid.mainTextureOffset = new Vector2(-Time.time,Time.time);
        if (!goingUp)
        {
            scanLine.transform.position = Vector2.MoveTowards(scanLine.position, new Vector2(0,-Screen.height), Screen.height/2f * Time.deltaTime);
            scanDot1.transform.position = Vector2.MoveTowards(scanDot1.position, new Vector2(scanDot1.position.x, -Screen.height), Screen.height / 2f * Time.deltaTime);
            scanDot2.transform.position = Vector2.MoveTowards(scanDot2.position, new Vector2(scanDot2.position.x, -Screen.height), Screen.height / 2f * Time.deltaTime);
        }
        else
        {
            scanLine.transform.position = Vector2.MoveTowards(scanLine.position, new Vector2(0, Screen.height), 500f * Time.deltaTime);
        }
        if (scanLine.transform.position.y>= (Screen.height)/2f)
        {
            goingUp = false;
        }
        if (scanLine.transform.position.y <= 0)
        {
            goingUp = false;
            scanLine.transform.position = new Vector2(0, Screen.height);
            scanDot1.transform.position = new Vector2(scanDot1.position.x, Screen.height);
            scanDot2.transform.position = new Vector2(scanDot2.position.x, Screen.height);
        }
    }
}
