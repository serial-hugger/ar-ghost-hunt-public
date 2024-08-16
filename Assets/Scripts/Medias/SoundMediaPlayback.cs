using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoundMediaPlayback : MonoBehaviour
{
    public GameObject dotPrefab;
    public GameObject[] dots = new GameObject[1024];

    public Transform dotsHolder;

    public float[] displayWaveform;

    public Evidence myEvidence = null;
    public Evidence prevEvidence = null;

    public GameObject evidenceTagPrefab;

    public GameObject tagsHolder;

    public AudioSource myAudio;

    public Sprite playSprite;
    public Sprite stopSprite;

    public Image controlButton;

    public Color playedDotColor;
    public Color unplayedDotColor;
    public GameObject playbackLine;

    public AudioInput inputScript;

    public int lastAudibleEvidence = -1;

    public List<GameObject> ghostSpeech = new List<GameObject>();

    public bool setupComplete;

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

    public GameObject adBubble;

    // Start is called before the first frame update
    private void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        SetColor();
        if (myAudio.isPlaying)
        {
            controlButton.sprite = stopSprite;
        }
        else
        {
            controlButton.sprite = playSprite;
            lastAudibleEvidence = -1;
        }
        if (myEvidence!=prevEvidence)
        {
            prevEvidence = myEvidence;
            Reset();
            myAudio.Stop();
            myAudio.Play();
        }
        PlayEvidence();
        playbackLine.transform.localPosition = new Vector3(-320f + GetPosition() * (0.042f / 4f), 0f, 0f);
    }
    private void AddEvidenceMarks()
    {
        foreach (Transform child in tagsHolder.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        foreach (var t in myEvidence.audibleEvidences)
        {
            GameObject newEvidence = Instantiate(evidenceTagPrefab, this.transform);
            newEvidence.transform.localPosition = new Vector3(-320f + t.position * (0.042f/4f), 0f, 0f);
            newEvidence.transform.SetParent(tagsHolder.transform);
        }
    }
    public void Reset()
    {
        if (!setupComplete)
        {
            Setup();
        }
        lastAudibleEvidence = -1;
        myAudio.clip.SetData(myEvidence.sound,inputScript.samplerate);
        myAudio.clip.GetData(displayWaveform,0);
        float[] wf = displayWaveform;
        float increaseAmt = (inputScript.frequency * 15) / 1024;
        for (int i = 0; i < 1024; i++)
        {
            float size = (1f);
            dots[i].transform.localScale = new Vector3(1f,Mathf.Max(-300f, Mathf.Min(300f, wf[(i * Mathf.FloorToInt(increaseAmt))] * 1000f)) * size, 1f);
        }
        AddEvidenceMarks();
    }
    private int GetPosition()
    {
        return Mathf.FloorToInt(myAudio.time * ((inputScript.frequency*15)/15));
    }
    private void PlayEvidence()
    {
        foreach (var t in myEvidence.audibleEvidences)
        {
            if (GetPosition()>= t.position && lastAudibleEvidence< t.position&&myAudio.isPlaying)
            {
                lastAudibleEvidence = GetPosition();
                foreach (var t1 in ghostSpeech)
                {
                    if (t1.name == t.word)
                    {
                        GameObject newSound = Instantiate(t1, this.transform);
                    }
                }
            }
        }
    }
    public void ControlButtonPress()
    {
        if (myAudio.isPlaying)
        {
            myAudio.Stop();
            Reset();
        }
        else
        {
            myAudio.Play();
            Reset();
        }
        SetColor();
    }
    public void OnEnable()
    {
        Reset();
        if (saveScript.gameData.deskEvidence.Contains(myEvidence) && (myEvidence.messageUploadingTo == null || myEvidence.messageUploadingTo == ""))
        {
            archiveButton.SetActive(true);
        }
        else
        {
            archiveButton.SetActive(false);
        }
        if (mainScript.controlScript.selectQuestItems.Count > 0)
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
        if (mainScript.controlScript.selectQuestItems.Count > 0 && !mainScript.controlScript.archiveSelectAllowed)
        {
            int reward = 0;
            if (saveScript.HasBonusType(myEvidence))
            {
                reward = Mathf.RoundToInt(myEvidence.GetScore() * saveScript.controlScript.analysisRewardMultiplier) * 2;
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
    private void Setup()
    {
        displayWaveform = new float[inputScript.frequency * 15];
        for (int i = 0; i < 1024; i++)
        {
            GameObject newDot = Instantiate(dotPrefab, this.transform);
            newDot.transform.localPosition = new Vector3(-320f + (i * .625f), 0f, 0f);
            dots[i] = newDot;
            dots[i].transform.SetParent(dotsHolder);
        }
        foreach (var t in dots)
        {
            t.transform.GetChild(0).gameObject.SetActive(false);
        }
        myAudio.clip = AudioClip.Create("Recording", inputScript.frequency*15, 1, inputScript.frequency, false);
        myAudio.clip.SetData(myEvidence.sound, inputScript.samplerate);
        setupComplete = true;
    }
    private void SetColor()
    {
        float increaseAmt = (inputScript.frequency * 15) / 1024;
        for (int i = 0; i < 1024; i++)
        {
            if (playbackLine.transform.localPosition.x>dots[i].transform.localPosition.x)
            {
                dots[i].GetComponentsInChildren<Image>()[1].color = playedDotColor;
            }
            else
            {
                dots[i].GetComponentsInChildren<Image>()[1].color = unplayedDotColor;
            }
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
