using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyzeUI : MonoBehaviour
{
    public SaveManager saveScript;
    public Message analysisMessage;

    public Controller controlScript;

    public AddStashEvidenceButton submitButton;

    public Image collectButton;
    public TextMeshProUGUI collectText;
    public Color readyColor;
    public Color notReadyColor;

    public GameObject rewardSound;

    public GameObject skipTicketButton;
    public GameObject skipTimeButton;
    public GameObject premiumShopButton;

    public TextMeshProUGUI neededTimeText;

    public TextMeshProUGUI remainingTicketText;
    public TextMeshProUGUI remainingTimeText;

    public GameObject timeHand;
    public Image timeFill;

    public TextMeshProUGUI bonusText;

    private void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        bonusText.text = "2X Bonus: [" + saveScript.gameData.bonusType+"]";
        if (analysisMessage!=null && analysisMessage.evidenceUploading!=null && analysisMessage.evidenceUploading!="")
        {
            collectButton.gameObject.SetActive(true);
            skipTicketButton.SetActive(false);
            skipTimeButton.SetActive(false);
            premiumShopButton.SetActive(false);
            if (analysisMessage.timeUploadEnd<= TimeManager.GetTime())
            {
                collectButton.color = readyColor;
                collectText.color = readyColor;
                Evidence thisEvidence = saveScript.GetEvidenceFromId(analysisMessage.evidenceUploading);
                if (!Tutorial.inTutorial) {
                    int reward = 0;
                    if (saveScript.HasBonusType(thisEvidence))
                    {
                        reward = Mathf.RoundToInt(thisEvidence.GetScore()*controlScript.analysisRewardMultiplier) * 2;
                    }
                    else
                    {
                        reward = Mathf.RoundToInt(thisEvidence.GetScore() * controlScript.analysisRewardMultiplier);
                    }
                    collectText.text = "Reward: $" + reward + " + " + reward + "XP";
                }
                else
                {
                    collectText.text = "Reward: $" + "500" + " + " + "0" + "XP";
                }
                if (Tutorial.tutorial[Tutorial.tutorialPhase].advanceAction== "analysiscomplete")
                {
                    Tutorial.AdvanceTutorial();
                }
            }
            else
            {
                collectButton.color = notReadyColor;
                collectText.color = notReadyColor;
                collectText.text = "Not Ready to Collect...";
                collectButton.gameObject.SetActive(false);
                skipTicketButton.SetActive(true);
                skipTimeButton.SetActive(true);
                premiumShopButton.SetActive(true);
                string timeRemaining = TimeManager.GetTimeStringFromTicks(analysisMessage.timeUploadEnd - TimeManager.GetTime(),true);
                neededTimeText.text = "USE\n" + timeRemaining;
                remainingTimeText.text = "Remaining:\n" + TimeManager.GetTimeStringFromTicks(saveScript.gameData.skipTime,true);
                remainingTicketText.text = "Remaining:\n"+saveScript.gameData.skipTickets.ToString();

                float fillAmt = Mathf.Min(System.TimeSpan.TicksPerHour * 12f, saveScript.gameData.skipTime) / (12f*System.TimeSpan.TicksPerHour);
                timeHand.transform.localEulerAngles = new Vector3(0f,0f,0f-fillAmt*360f);
                timeFill.fillAmount = fillAmt;
            }
        }
        else
        {
            skipTicketButton.SetActive(false);
            skipTimeButton.SetActive(false);
            premiumShopButton.SetActive(false);
            collectButton.gameObject.SetActive(false);
        }
    }
    void OnEnable()
    {
        for (int i = 0; i < saveScript.gameData.messages.Count; i++)
        {
            if (saveScript.gameData.messages[i].name == "Serial")
            {
                analysisMessage = saveScript.gameData.messages[i];
            }
        }
        submitButton.myMessage = analysisMessage;
        submitButton.number = 0;
        if (controlScript.selectedEvidences.Count>0)
        {
            analysisMessage.evidenceUploading = controlScript.selectedEvidences[0];
            saveScript.GetEvidenceFromId(analysisMessage.evidenceUploading).messageUploadingTo = analysisMessage.id;
            analysisMessage.timeUploadStart = TimeManager.GetTime();
            if (Tutorial.inTutorial) {
                analysisMessage.timeUploadEnd = TimeManager.GetTime() + System.TimeSpan.TicksPerSecond*15;
            }
            else
            {
                if (!saveScript.gameData.premium) {
                    analysisMessage.timeUploadEnd = TimeManager.GetTime() + System.TimeSpan.TicksPerHour * 8;
                }
                else
                {
                    analysisMessage.timeUploadEnd = TimeManager.GetTime() + System.TimeSpan.TicksPerHour * 4;
                }
            }
            controlScript.selectedEvidences.Clear();
            controlScript.saveScript.WriteFileHang();
            saveScript.CreateNotifications();
            gameObject.SetActive(false);
        }
        for (int i = 0; i < saveScript.gameData.deskEvidence.Count; i++)
        {
            if (analysisMessage!=null && saveScript.gameData.deskEvidence[i].messageUploadingTo!= null && saveScript.gameData.deskEvidence[i].messageUploadingTo != "" && saveScript.GetMessageFromId(saveScript.gameData.deskEvidence[i].messageUploadingTo).name == analysisMessage.name)
            {
                submitButton.myEvidence = saveScript.gameData.deskEvidence[i];
                submitButton.Reset();
            }
        }
    }
    public void Collect()
    {
        if (analysisMessage != null && analysisMessage.evidenceUploading != null && analysisMessage.evidenceUploading !="") 
        {
            if (analysisMessage.timeUploadEnd <= TimeManager.GetTime()) {
                Evidence evidenceToRemove = saveScript.GetEvidenceFromId(analysisMessage.evidenceUploading);
                if (!Tutorial.inTutorial) {
                    int reward = 0;
                    if (saveScript.HasBonusType(evidenceToRemove))
                    {
                        reward = Mathf.RoundToInt(evidenceToRemove.GetScore() * controlScript.analysisRewardMultiplier) * 2;
                        saveScript.gameData.bonusType = "";
                    }
                    else
                    {
                        reward = Mathf.RoundToInt(evidenceToRemove.GetScore() * controlScript.analysisRewardMultiplier);
                    }
                    saveScript.gameData.experience += reward;
                    saveScript.gameData.money += reward;
                    controlScript.mainUI.zoomerXpNeeded = reward / 5;
                    controlScript.mainUI.zoomerMoneyNeeded = reward / 5;
                    controlScript.mainUI.zoomerStart = collectButton.transform.position;
                }
                else
                {
                    saveScript.gameData.experience += 0;
                    saveScript.gameData.money += 500;
                    controlScript.mainUI.zoomerMoneyNeeded = 500 / 5;
                    controlScript.mainUI.zoomerStart = collectButton.transform.position;
                }
                analysisMessage.evidenceUploading = null;
                saveScript.RemoveEvidenceFromDesk(evidenceToRemove);
                submitButton.myEvidence = null;
                controlScript.saveScript.WriteFileHang();
                saveScript.CreateNotifications();
                submitButton.Reset();
                Instantiate(rewardSound, Camera.main.transform.position, transform.rotation);
                if (!Tutorial.inTutorial && TimeManager.GetTime() - saveScript.gameData.prevReviewRequest > System.TimeSpan.TicksPerDay*100)
                {
                    controlScript.requestReview = true;
                }
            }
            else
            {
                if (!Tutorial.inTutorial)
                {
                    controlScript.popupScript.DisplayError("CAN'T COLLECT YET");
                }
            }
        }
    }
    public void UseTicket()
    {
        if (!saveScript.fileWriting) {
            if (saveScript.gameData.skipTickets > 0) {
                analysisMessage.timeUploadEnd = TimeManager.GetTime();
                saveScript.gameData.skipTickets--;
                saveScript.WriteFileHang();
                saveScript.CreateNotifications();
            }
            else
            {
                controlScript.popupScript.DisplayError("NO TICKETS REMAINING");
            }
        }
        else
        {
            controlScript.popupScript.DisplayError("PLEASE TRY AGAIN");
        }
    }
    public void UseTime()
    {
        if (!saveScript.fileWriting)
        {
            long timeRemaining = analysisMessage.timeUploadEnd - TimeManager.GetTime();
            if (saveScript.gameData.skipTime > 0)
            {
                long mostTime = System.Math.Min(timeRemaining, saveScript.gameData.skipTime);
                analysisMessage.timeUploadEnd -= mostTime;
                saveScript.gameData.skipTime -= mostTime;
                saveScript.WriteFileHang();
                saveScript.CreateNotifications();
            }
            else
            {
                controlScript.popupScript.DisplayError("NO TIME REMAINING");
            }
        }
        else
        {
            controlScript.popupScript.DisplayError("PLEASE TRY AGAIN");
        }
    }
    public void RequestReview()
    {

    }
}
