using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUp : MonoBehaviour
{
    public TMP_Text levelUpText;
    public Transform underline1;
    public Transform underline2;
    public Transform panel;
    public float timeToDisplayLevel = 0f;
    // Start is called before the first frame update
    void Start()
    {
        underline1.localPosition = new Vector3(-300f,0f,0f);
        underline2.localPosition = new Vector3(300f, -15f, 0f);
        panel.localScale = new Vector2(1f,0f);
    }

    // Update is called once per frame
    void Update()
    {
        timeToDisplayLevel -= 1f * Time.deltaTime;
        if (timeToDisplayLevel>=0f)
        {
            underline1.localPosition = Vector2.Lerp(underline1.localPosition,new Vector3(0f,0f,0f),15f*Time.deltaTime);
            underline2.localPosition = Vector2.Lerp(underline2.localPosition, new Vector3(0f, -15f, 0f), 15f * Time.deltaTime);
            panel.localScale = Vector2.Lerp(panel.localScale,new Vector2(1f, 1f),15f*Time.deltaTime);
            if (Mathf.Abs(underline1.transform.localPosition.x)<0.5f)
            {
                levelUpText.transform.localScale = Vector3.Lerp(levelUpText.transform.localScale,new Vector3(1f,1f,1f),15f*Time.deltaTime);
            }
        }
        else
        {
            panel.localScale = Vector2.Lerp(panel.localScale, new Vector2(1f, 0f), 15f * Time.deltaTime);
            underline1.localPosition = Vector2.Lerp(underline1.localPosition, new Vector3(-300f, 0f, 0f), 15f * Time.deltaTime);
            underline2.localPosition = Vector2.Lerp(underline2.localPosition, new Vector3(300f, -15f, 0f), 15f * Time.deltaTime);
        }
    }
    public void levelUp(int level)
    {
        levelUpText.transform.localScale = new Vector3(1f, 0f, 1f);
        underline1.localPosition = new Vector3(300f, 0f, 0f);
        underline2.localPosition = new Vector3(-300f, -15f, 0f);
        timeToDisplayLevel = 5f;
        levelUpText.text = "Level " + level;
    }
}
