using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StashEvidenceButton : MonoBehaviour
{

    public MainUI uiScript;
    public SaveManager saveScript;

    public Evidence myEvidence;
    public Image myImage;
    public Image myIcon;

    public Button myButton;

    public Sprite cameraIcon;
    public Sprite videoIcon;
    public Sprite soundIcon;

    public GameObject[] dots = new GameObject[1024];
    public GameObject dotsHolder;
    public GameObject dotPrefab;

    public AudioInput inputScript;
    public float[] displayWaveform;

    public List<Sprite> videoPreview = new List<Sprite>();
    public int currentFrame = 0;
    public float timeTillFrameChange;

    public bool selectable;

    public Slider uploadBar;
    public TextMeshProUGUI uploadTime;

    public ButtonBlocker myButtonBlock;

    public GameObject soundPage;

    // Start is called before the first frame update
    void Start()
    {
        saveScript = uiScript.controlScript.saveScript;
        if (myEvidence.type == "photo")
        {
            myIcon.sprite = cameraIcon;
            //TEXTURE SET
            Texture2D tex;
            try
            {
                tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, Controller.photoFormat, false);
            }
            catch
            {
                tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, TextureFormat.RGBA32, false);
            }
            tex.LoadRawTextureData(myEvidence.photo);
            tex.Apply();
            myImage.sprite = Sprite.Create(tex,new Rect(0,0, myEvidence.frameWidth, myEvidence.frameHeight),new Vector2(.5f,.5f));
        }
        if (myEvidence.type == "video")
        {
            myIcon.sprite = videoIcon;
            for (int i = 0; i <8; i++) {
                //TEXTURE SET
                Texture2D tex = new Texture2D(myEvidence.frameWidth, myEvidence.frameHeight, Controller.videoFormat, false);
                tex.LoadRawTextureData(myEvidence.GetVideoFrame(((EvidenceCapture.videoFramesPerSecond * 15) / 8) * i));
                tex.Apply();
                videoPreview.Add(Sprite.Create(tex, new Rect(0, 0, myEvidence.frameWidth, myEvidence.frameHeight), new Vector2(.5f, .5f)));
            }
            myImage.sprite = videoPreview[0];
        }
        if (myEvidence.type == "sound")
        {
            displayWaveform = new float[inputScript.frequency * 15];
            myImage.enabled = false;
            myIcon.sprite = soundIcon;
            for (int i = 0; i < 1024; i++)
            {
                GameObject newDot = Instantiate(dotPrefab, this.transform);
                newDot.transform.localPosition = new Vector3(-63f + (i * .123046875f), 0f, 0f);
                dots[i] = newDot;
                dots[i].transform.SetParent(dotsHolder.transform);
                Image dot = newDot.transform.GetChild(0).GetComponent<Image>();
                if (selectable) {
                    dot.color = Color.white;
                }
                else
                {
                    dot.color = new Color(.1f,.1f,.1f);
                }
            }
            //SET WAVEFORM
            displayWaveform = myEvidence.sound;
            float[] wf = displayWaveform;
            float increaseAmt = (inputScript.frequency * 15) / 1024;
            for (int i = 0; i < 1024; i++)
            {
                float size = (0.45f);
                dots[i].transform.localScale = new Vector3(0.5f, Mathf.Max(-300f, Mathf.Min(300f, wf[(i * Mathf.FloorToInt(increaseAmt))] * 1000f)) * size, 1f);
            }
        }
        myButton.enabled = selectable;
        if (!selectable)
        {
            myImage.color = new Color(.3f,.3f,.3f);
        }
        myButtonBlock.defaultState = selectable;
    }

    // Update is called once per frame
    void Update()
    {
        if (myEvidence != null && TimeManager.validTime)
        {
            if (myEvidence.messageUploadingTo!=null && saveScript.GetMessageFromId(myEvidence.messageUploadingTo).name!="")
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
        if (videoPreview.Count>0)
        {
            timeTillFrameChange -= 1f * Time.deltaTime;
            if (timeTillFrameChange<=0f)
            {
                timeTillFrameChange = 1f;
                currentFrame++;
                if (currentFrame >= videoPreview.Count)
                {
                    currentFrame = 0;
                }
                myImage.sprite = videoPreview[currentFrame];
            }
        }
    }
    public void OpenEvidence()
    {
        uiScript.prevMenu = uiScript.currentMenu;
        if (myEvidence.type == "photo")
        {
            uiScript.currentMenu = "playbackphoto";
            uiScript.playbackPhotoUI.myEvidence = myEvidence;
            uiScript.playbackPhotoUI.prevEvidence = null;
        }
        if (myEvidence.type == "video")
        {
            uiScript.currentMenu = "playbackvideo";
            uiScript.playbackVideoUI.myEvidence = myEvidence;
            uiScript.playbackVideoUI.prevEvidence = null;
        }
        if (myEvidence.type == "sound")
        {
            uiScript.currentMenu = "playbacksound";
            uiScript.playbackSoundUI.myEvidence = myEvidence;
            uiScript.playbackSoundUI.prevEvidence = null;
        }
        Instantiate(soundPage, Camera.main.transform.position, transform.rotation);
    }
}
