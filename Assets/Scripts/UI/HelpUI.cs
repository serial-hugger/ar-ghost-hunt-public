using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : MonoBehaviour
{
    public Image spiriiRender;

    public Sprite spirii1;
    public Sprite spirii2;
    public Sprite spirii3;
    public Sprite spirii4;

    public int spiriiSprite = 0;

    public float spriteOffset = 0f;
    public bool goingUp;

    public float timeTillTextAdd = 0f;

    public TextMeshProUGUI speechText;
    public int currentSpeech = 0;
    public string speech;
    // Start is called before the first frame update
    void Awake()
    {
        speech = "How may I assist you?";
        speechText.text += speech[currentSpeech];
    }

    // Update is called once per frame
    void Update()
    {
        timeTillTextAdd -= 1f * Time.deltaTime;
        if (timeTillTextAdd<0f)
        {
            timeTillTextAdd = 0.1f;
            currentSpeech++;
            spiriiSprite++;
            if (spiriiSprite > 3)
            {
                spiriiSprite = 0;
            }
            if (currentSpeech >= speech.Length)
            {
                currentSpeech = speech.Length;
                spiriiSprite = 0;
            }
            else
            {
                speechText.text += speech[currentSpeech];
            }
        }
        if (goingUp)
        {
            spriteOffset += 1f * Time.deltaTime;
            if (spriteOffset>1f)
            {
                goingUp = false;
            }
        }
        else
        {
            spriteOffset -= 1f * Time.deltaTime;
            if (spriteOffset < -1f)
            {
                goingUp = true;
            }
        }
        Vector3 tempPos = spiriiRender.transform.localPosition;
        tempPos.y = spriteOffset*10f;
        spiriiRender.transform.localPosition = tempPos;
        if (spiriiSprite==0)
        {
            spiriiRender.sprite = spirii1;
        }
        if (spiriiSprite == 1)
        {
            spiriiRender.sprite = spirii2;
        }
        if (spiriiSprite == 2)
        {
            spiriiRender.sprite = spirii3;
        }
        if (spiriiSprite == 3)
        {
            spiriiRender.sprite = spirii4;
        }
    }
    private void OnEnable()
    {
        currentSpeech = 0;
        speechText.text = "";
        speechText.text += speech[currentSpeech];
    }
}
