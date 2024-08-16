using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessagesUI : MonoBehaviour
{
    public Controller controlScript;

    public GameObject messageButton;
    public GameObject evidenceButton;

    public GameObject messageContent;
    public GameObject evidenceContent;

    public RectTransform messageContentRect;
    public RectTransform evidenceContentRect;

    public SaveManager saveScript;

    public Message openMessage;
    public TextMeshProUGUI messageText;

    public MainUI uiScript;

    public Sprite enableUploadSprite;
    public Sprite disableUploadSprite;

    public Button uploadButton;
    public Image uploadImage;

    public GameObject rewardSound;

    private void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        openMessage = null;
    }
    // Start is called before the first frame update
    void OnEnable()
    {
        saveScript.SetupMessages();
        for (int i = 0; i< saveScript.gameData.messages.Count;i++)
        {
            if (TimeManager.GetTime() >= saveScript.gameData.messages[i].timeExpire && saveScript.gameData.messages[i].timeExpire!=-1)
            {
                saveScript.gameData.messages.RemoveAt(i);
                i = 0;
            }
        }
        foreach (Transform child in messageContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in evidenceContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < saveScript.gameData.messages.Count;i++)
        {
            if (saveScript.gameData.messages[i].name!="Serial" && !saveScript.gameData.messages[i].completed) {
                MessageButton message = Instantiate(messageButton, messageContent.transform).GetComponent<MessageButton>();
                message.myMessage = saveScript.gameData.messages[i];
                message.messageUI = this;
            }
        }
        messageContentRect.sizeDelta = new Vector2(200f, 100f + (100f * (Mathf.FloorToInt((saveScript.gameData.messages.Count - 1)))));
        if (controlScript.selectedEvidences.Count==0) {
            openMessage = null;
        }
        OpenMessage();
        UpdateUploadButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OpenMessage()
    {
        string message = "";
        if (openMessage!=null) {
            for (int i = 0; i < openMessage.messageParts.Count; i++)
            {
                message += "<color=#000000>" + openMessage.messageParts[i] + "</color>";
                if (i < openMessage.questItems.Count)
                {
                    if (Satisfied(openMessage.questItems[i],i)) {
                        message += "<color=#008000>[" + openMessage.questItems[i].messageText + "]</color>";
                    }
                    else
                    {
                        message += "<color=#800000>[" + openMessage.questItems[i].messageText + "]</color>";
                    }
                }
            }
            message += "\n\n<b>REWARDS: $" + openMessage.rewardMoney + " + " + openMessage.rewardExperience + "XP</b>";
            messageText.text = message;



            foreach (Transform child in evidenceContent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            for (int i = 0; i < openMessage.questItems.Count; i++)
            {
                AddStashEvidenceButton evidenceAdd = Instantiate(evidenceButton, evidenceContent.transform).GetComponent<AddStashEvidenceButton>();
                evidenceAdd.uiScript = uiScript;
                evidenceAdd.myMessage = openMessage;
                evidenceAdd.number = i;
                if (i < uiScript.controlScript.selectedEvidences.Count && uiScript.controlScript.selectedEvidences[i] != null)
                {
                    evidenceAdd.myEvidence = saveScript.GetEvidenceFromId(uiScript.controlScript.selectedEvidences[i]);
                }
            }
            evidenceContentRect.sizeDelta = new Vector2(700f, 90f + (90f * (Mathf.FloorToInt((openMessage.questItems.Count - 1) / 4f))));
        }
        else
        {
            messageText.text = "<color=#000000>Select a message to open!</color>";
        }
        UpdateUploadButton();
    }
    public void UpdateUploadButton()
    {
        if (openMessage!=null) {
            int satisfied = 0;
            for (int e = 0; e < uiScript.controlScript.selectedEvidences.Count; e++)
            {
                for (int q = 0; q < openMessage.questItems.Count; q++)
                {
                    if (uiScript.controlScript.selectedEvidences[e]!=null && saveScript.GetEvidenceFromId(uiScript.controlScript.selectedEvidences[e]).SatisfiesQuest(openMessage.questItems[q]))
                    {
                        satisfied++;
                    }
                }
            }
            if (satisfied >= openMessage.questItems.Count)
            {
                uploadImage.sprite = enableUploadSprite;
                uploadButton.enabled = true;
            }
            else
            {
                uploadImage.sprite = disableUploadSprite;
                uploadButton.enabled = false;
            }
        }
        else
        {
            uploadImage.sprite = disableUploadSprite;
            uploadButton.enabled = false;
        }
    }
    public void Upload()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable || saveScript.gameData.prewatchedAds>0) {
            int satisfied = 0;
            for (int e = 0; e < uiScript.controlScript.selectedEvidences.Count; e++)
            {
                for (int q = 0; q < openMessage.questItems.Count; q++)
                {
                    if (saveScript.GetEvidenceFromId(uiScript.controlScript.selectedEvidences[e]).SatisfiesQuest(openMessage.questItems[q]))
                    {
                        satisfied++;
                    }
                }
            }
            if (satisfied >= openMessage.questItems.Count && !controlScript.selectedEvidences.Contains(null))
            {
                foreach (Transform child in evidenceContent.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                controlScript.adScript.ShowAd();
                for (int i = 0; i < controlScript.selectedEvidences.Count; i++)
                {
                    if (saveScript.gameData.deskEvidence.Contains(saveScript.GetEvidenceFromId(controlScript.selectedEvidences[i])))
                    {
                        saveScript.gameData.deskEvidence.Remove(saveScript.GetEvidenceFromId(controlScript.selectedEvidences[i]));
                    }
                    if (saveScript.gameData.archiveEvidence.Contains(saveScript.GetEvidenceFromId(controlScript.selectedEvidences[i])))
                    {
                        saveScript.gameData.archiveEvidence.Remove(saveScript.GetEvidenceFromId(controlScript.selectedEvidences[i]));
                    }
                }
                saveScript.gameData.experience += openMessage.rewardExperience;
                saveScript.gameData.money += openMessage.rewardMoney;
                controlScript.mainUI.zoomerXpNeeded = openMessage.rewardExperience / 5;
                controlScript.mainUI.zoomerMoneyNeeded = openMessage.rewardMoney / 5;
                controlScript.mainUI.zoomerStart = uploadButton.transform.position;
                openMessage.completed = true;
                controlScript.selectedEvidences.Clear();
                controlScript.selectQuestItems.Clear();
                openMessage = null;
                OpenMessage();
                saveScript.WriteFile();
                Instantiate(rewardSound, Camera.main.transform.position, transform.rotation);
                gameObject.SetActive(false);
            }
        }
        else
        {
            controlScript.popupScript.DisplayError("NO INTERNET");
        }
    }
    public bool Satisfied(QuestItem item,int quest)
    {
        bool satisfied = false;
        if (quest==-1) {
            for (int i = 0; i < controlScript.selectedEvidences.Count; i++)
            {
                if (controlScript.selectedEvidences[i] != null && saveScript.GetEvidenceFromId(controlScript.selectedEvidences[i]).SatisfiesQuest(item))
                {
                    satisfied = true;
                }
            }
        }
        else
        {
            if (controlScript.selectedEvidences[quest] != null && saveScript.GetEvidenceFromId(controlScript.selectedEvidences[quest]).SatisfiesQuest(item))
            {
                satisfied = true;
            }
        }
        return satisfied;
    }
}
