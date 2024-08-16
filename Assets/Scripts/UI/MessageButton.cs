using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MessageButton : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI timeLeftText;
    public Message myMessage;

    public GameObject anyIcon;
    public TextMeshProUGUI anyAmt;
    public GameObject photoIcon;
    public TextMeshProUGUI photoAmt;
    public GameObject videoIcon;
    public TextMeshProUGUI videoAmt;
    public GameObject soundIcon;
    public TextMeshProUGUI soundAmt;

    public MessagesUI messageUI;

    public GameObject clickSound;

    // Start is called before the first frame update
    void Start()
    {
        SetupButton();
    }

    // Update is called once per frame
    void Update()
    {
        long timeRemaining = myMessage.timeExpire - TimeManager.GetTime();
        int days = Mathf.FloorToInt(timeRemaining / System.TimeSpan.TicksPerDay);
        int hours = Mathf.FloorToInt((timeRemaining% System.TimeSpan.TicksPerDay) / System.TimeSpan.TicksPerHour);
        int minutes = Mathf.FloorToInt((timeRemaining% System.TimeSpan.TicksPerHour) / System.TimeSpan.TicksPerMinute);
        int seconds = Mathf.FloorToInt((timeRemaining% System.TimeSpan.TicksPerMinute) / System.TimeSpan.TicksPerSecond);
        string newText = "Expires: ";
        if (days>0)
        {
            newText += days.ToString() + "d ";
        }
        if (hours > 0)
        {
            newText += hours.ToString() + "h ";
        }
        if (minutes > 0)
        {
            newText += minutes.ToString() + "m ";
        }
        if (seconds > 0)
        {
            newText += seconds.ToString() + "s ";
        }
        if (myMessage.timeExpire==-1)
        {
            newText = "Expires: Never";
        }
        timeLeftText.text = newText;
        if (myMessage.timeExpire<= TimeManager.GetTime() && myMessage.timeExpire!=-1)
        {
            Destroy(gameObject);
        }
    }
    public void ButtonPress()
    {
        messageUI.uiScript.controlScript.selectQuestItems.Clear();
        messageUI.uiScript.controlScript.selectedEvidences.Clear();
        messageUI.uiScript.controlScript.archiveSelectAllowed = true;
        messageUI.uiScript.controlScript.deskSelectAllowed = true;
        messageUI.openMessage = myMessage;
        messageUI.controlScript.selectedEvidences = new List<string>();
        for (int i = 0; i < myMessage.questItems.Count;i++)
        {
            messageUI.controlScript.selectedEvidences.Add(null);
        }
        messageUI.OpenMessage();
        Instantiate(clickSound, Camera.main.transform.position, transform.rotation);
    }
    public void SetupButton()
    {
        int xPos = -80;
        nameText.text = myMessage.name;
        if (myMessage.timeExpire <= 0)
        {
            timeLeftText.text = "Expires: Never";
        }
        int any = 0;
        int photo = 0;
        int video = 0;
        int sound = 0;
        for (int i = 0; i< myMessage.questItems.Count;i++)
        {
            if (myMessage.questItems[i].type == "any")
            {
                any++;
            }
            if (myMessage.questItems[i].type == "photo")
            {
                photo++;
            }
            if (myMessage.questItems[i].type == "video")
            {
                video++;
            }
            if (myMessage.questItems[i].type == "sound")
            {
                sound++;
            }
        }
        anyAmt.text = any.ToString();
        photoAmt.text = photo.ToString();
        videoAmt.text = video.ToString();
        soundAmt.text = sound.ToString();
        if (any!=0)
        {
            anyIcon.SetActive(true);
            anyIcon.transform.localPosition = new Vector2(xPos,-3);
            xPos += 42;
        }
        else
        {
            anyIcon.SetActive(false);
        }
        if (photo != 0)
        {
            photoIcon.SetActive(true);
            photoIcon.transform.localPosition = new Vector2(xPos, -3);
            xPos += 42;
        }
        else
        {
            photoIcon.SetActive(false);
        }
        if (video != 0)
        {
            videoIcon.SetActive(true);
            videoIcon.transform.localPosition = new Vector2(xPos, -3);
            xPos += 42;
        }
        else
        {
            videoIcon.SetActive(false) ;
        }
        if (sound != 0)
        {
            soundIcon.SetActive(true);
            soundIcon.transform.localPosition = new Vector2(xPos, -3);
            xPos += 42;
        }
        else
        {
            soundIcon.SetActive(false);
        }
    }
}
