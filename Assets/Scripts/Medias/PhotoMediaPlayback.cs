using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhotoMediaPlayback : MonoBehaviour
{
    public Evidence myEvidence = null;
    public Evidence prevEvidence = null;

    public Image myImage;

    public SaveManager saveScript;
    public MainUI mainScript;

    public GameObject trashSound;

    public GameObject archiveButton;
    public GameObject trashButton;
    public GameObject selectButton;
    public GameObject viewButton;

    public TextMeshProUGUI uploadTime;
    public Slider uploadBar;

    public TextMeshProUGUI rewardEstimate;

    public ButtonBlocker archiveBlocker;

    public GameObject adBubble;


    // Start is called before the first frame update
    void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myEvidence.messageUploadingTo != null && myEvidence.messageUploadingTo != "")
        {
            SetTimers();
        }
    }
    private void Reset()
    {
        //TEXTURE SET
        Texture2D tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, Controller.photoFormat, false);
        tex.LoadRawTextureData(myEvidence.photo);
        tex.Apply();
        myImage.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, myEvidence.frameWidth, myEvidence.frameHeight), new Vector2(0.5f, 0.5f));
        Vector2 tempScale = myImage.transform.localScale;
        if (myEvidence.frameWidth< myEvidence.frameHeight) {
            tempScale.y = 1f;
            tempScale.x = (float)myEvidence.frameWidth / (float)myEvidence.frameHeight;
        }
        else
        {
            tempScale.x = 1f;
            tempScale.y =  (float)myEvidence.frameHeight/ (float)myEvidence.frameWidth;
        }
        myImage.transform.localScale = tempScale;

    }
    public void ControlButtonPress()
    {
        Reset();
    }
    public void OnEnable()
    {
        Reset();
        if (mainScript.controlScript.selectQuestItems.Count>0)
        {
            selectButton.SetActive(true);
            trashButton.SetActive(false);
            archiveButton.SetActive(false);
            if (mainScript.controlScript.archiveSelectAllowed)
            {
                adBubble.SetActive(false);
            }
            else
            {
                adBubble.SetActive(true);
            }
        }
        else
        {
            selectButton.SetActive(false);
            trashButton.SetActive(true);
            archiveButton.SetActive(true);
        }
        if (myEvidence.messageUploadingTo != null && myEvidence.messageUploadingTo != "" && TimeManager.validTime)
        {
            uploadTime.gameObject.SetActive(true);
            uploadBar.gameObject.SetActive(true);
            selectButton.SetActive(false);
            trashButton.SetActive(false);
            archiveButton.SetActive(false);
            viewButton.SetActive(true);
        }
        else
        {
            if (myEvidence.messageUploadingTo != null && myEvidence.messageUploadingTo != "")
            {
                uploadTime.gameObject.SetActive(true);
                uploadTime.text = "UPLOADING";
                trashButton.SetActive(false);
                archiveButton.SetActive(false);
            }
            else
            {
                uploadTime.gameObject.SetActive(false);
                if (mainScript.controlScript.selectQuestItems.Count <= 0)
                {
                    trashButton.SetActive(true);
                    archiveButton.SetActive(true);
                }
            }
            uploadBar.gameObject.SetActive(false);
            viewButton.SetActive(false);
        }
        if (mainScript.controlScript.selectQuestItems.Count > 0 && !mainScript.controlScript.archiveSelectAllowed) {
            int reward = 0;
            if (saveScript.HasBonusType(myEvidence)) {
                reward = Mathf.RoundToInt(myEvidence.GetScore() * saveScript.controlScript.analysisRewardMultiplier) *2;
            }
            else
            {
                reward = Mathf.RoundToInt(myEvidence.GetScore() * saveScript.controlScript.analysisRewardMultiplier);
            }
            rewardEstimate.text = "Reward: $" + reward + " + " + reward + "XP";
        }
        else
        {
            rewardEstimate.text = "";
        }
        if (mainScript.controlScript.selectQuestItems.Count == 0 && TimeManager.validTime && saveScript.gameData.deskEvidence.Contains(myEvidence) && (myEvidence.messageUploadingTo == null || myEvidence.messageUploadingTo == ""))
        {
            archiveButton.SetActive(true);
        }
        else
        {
            archiveButton.SetActive(false);
        }
    }
    public void TrashEvidence()
    {
        saveScript.RemoveEvidenceFromDesk(myEvidence);
        saveScript.RemoveEvidenceFromArchive(myEvidence);
        Instantiate(trashSound, Camera.main.transform.position, new Quaternion());
        mainScript.currentMenu = mainScript.prevMenu;
        mainScript.prevMenu = "default";
        saveScript.WriteFileHang();
    }
    public void ArchiveEvidence()
    {
        if (saveScript.gameData.archiveEvidence.Count < saveScript.gameData.archiveSlots)
        {
            saveScript.AddEvidenceToArchive(myEvidence);
            saveScript.RemoveEvidenceFromDesk(myEvidence);
            Instantiate(trashSound, Camera.main.transform.position, new Quaternion());
            mainScript.currentMenu = mainScript.prevMenu;
            mainScript.prevMenu = "default";
            saveScript.WriteFileHang();
        }
        else
        {
            mainScript.controlScript.popupScript.DisplayError("ARCHIVE IS FULL");
        }
    }
    public void SelectThisAsEvidence()
    {
        if (mainScript.controlScript.archiveSelectAllowed || (Application.internetReachability != NetworkReachability.NotReachable || saveScript.gameData.prewatchedAds > 0))
        {
            mainScript.controlScript.selectedEvidences[mainScript.controlScript.currentEvidenceAdd] = myEvidence.id;
            if (!mainScript.controlScript.archiveSelectAllowed)
            {
                saveScript.controlScript.adScript.ShowAd();
                mainScript.currentMenu = "analyze";
            }
            if (mainScript.controlScript.archiveSelectAllowed)
            {
                mainScript.currentMenu = "messages";
            }
            mainScript.prevMenu = "default";
            mainScript.controlScript.selectQuestItems.Clear();
        }
        else
        {
            saveScript.controlScript.popupScript.DisplayError("NO INTERNET");
        }
    }
    public void ViewEvidence()
    {
        mainScript.prevMenu = "stash";
        mainScript.currentMenu = "analyze";
    }
    public void SetTimers()
    {
        if (myEvidence != null && TimeManager.validTime)
        {
            if (myEvidence.messageUploadingTo != null && saveScript.GetMessageFromId(myEvidence.messageUploadingTo).name != "")
            {
                uploadBar.gameObject.SetActive(true);
                uploadTime.gameObject.SetActive(true);
                if (saveScript.GetMessageFromId(myEvidence.messageUploadingTo).timeUploadEnd > TimeManager.GetTime())
                {
                    uploadBar.value = (float)(TimeManager.GetTime() - saveScript.GetMessageFromId(myEvidence.messageUploadingTo).timeUploadStart) / (float)(saveScript.GetMessageFromId(myEvidence.messageUploadingTo).timeUploadEnd - saveScript.GetMessageFromId(myEvidence.messageUploadingTo).timeUploadStart);
                    long timeRemaining = saveScript.GetMessageFromId(myEvidence.messageUploadingTo).timeUploadEnd - TimeManager.GetTime();
                    int days = Mathf.FloorToInt(timeRemaining / System.TimeSpan.TicksPerDay);
                    int hours = Mathf.FloorToInt((timeRemaining % System.TimeSpan.TicksPerDay) / System.TimeSpan.TicksPerHour);
                    int minutes = Mathf.FloorToInt((timeRemaining % System.TimeSpan.TicksPerHour) / System.TimeSpan.TicksPerMinute);
                    int seconds = Mathf.FloorToInt((timeRemaining % System.TimeSpan.TicksPerMinute) / System.TimeSpan.TicksPerSecond);
                    string newText = "";
                    if (days > 0)
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
                    uploadTime.text = newText;
                }
                else
                {
                    uploadBar.value = 1f;
                    uploadTime.text = "COMPLETE";
                }
            }
            else
            {
                uploadBar.gameObject.SetActive(false);
                uploadTime.gameObject.SetActive(false);
            }
        }
        else
        {
            uploadBar.gameObject.SetActive(false);
            uploadTime.gameObject.SetActive(false);
        }
    }
}
