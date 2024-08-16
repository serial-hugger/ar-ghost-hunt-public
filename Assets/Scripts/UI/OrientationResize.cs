using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationResize : MonoBehaviour
{
    /*public float portraitSize;
    public float landscapeSize;
    public float portraitX;
    public float landscapeX;
    public float portraitY;
    public float landscapeY;

    Vector3 startPosition;

    RectTransform myRect;

    // Start is called before the first frame update
    void Awake()
    {
        myRect = GetComponent<RectTransform>();
        startPosition = myRect.localPosition;
    }

    // Update is called once per frame
    void OnEnable()
    {
        if (Screen.width<Screen.height)
        {
            transform.localScale = new Vector3(portraitSize, portraitSize, portraitSize);
            myRect.localPosition = new Vector3(startPosition.x + portraitX, startPosition.y + portraitY, myRect.localPosition.z);
        }
        if (Screen.width > Screen.height)
        {
            transform.localScale = new Vector3(landscapeSize, landscapeSize, landscapeSize);
            myRect.localPosition = new Vector3(startPosition.x + landscapeX, startPosition.y + landscapeY, myRect.localPosition.z);
        }
    }*/
}
