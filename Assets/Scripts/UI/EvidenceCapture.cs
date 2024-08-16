using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class EvidenceCapture : MonoBehaviour
{
    public static int videoFramesPerSecond = 5;

    public GameObject cameraSound;
    public Material cameraFlash;

    public GameObject recordingStartSound;
    public GameObject recordingEndSound;

    public List<GameObject> hideDuringRecording;

    public Controller controlScript;
    public SaveManager saveScript;

    public Camera mainCam;
    public Camera detectionCam;

    public LayerMask mainSettings;
    public LayerMask photoSettings;
    public LayerMask videoSettings;

    public LayerMask liveCameraSettings;

    public LayerMask photoDetectionSettings;
    public LayerMask videoDetectionSettings;

    public Button cameraButton;
    public Button cameraButtonTimer;
    public Button cameraButtonTimerBack;

    public Button videoButton;
    public Button videoButtonTimer;
    public Button videoButtonTimerBack;

    public Button audioButton;
    public Button audioButtonTimer;
    public Button audioButtonTimerBack;

    public Image videoButtonRing;
    public Image audioButtonRing;

    public float cameraButtonHideTimer = 0f;
    public float videoButtonHideTimer = 0f;
    public float audioButtonHideTimer = 0f;



    public float totalVideoScore = 0;

    public int handprint;
    public Material handprintMat;
    public int ghostorb;
    public Material ghostorbMat;
    public int ghosttrail;
    public Material ghosttrailMat;
    public int ghostbody;
    public Material ghostbodyMat;
    public int ghostfoot;
    public Material ghostfootMat;
    public int ghosthand;
    public Material ghosthandMat;
    public int ghosthead;
    public Material ghostheadMat;
    public int candle;
    public Material candleMat;
    public int clipboard;
    public Material clipboardMat;
    public int compass;
    public Material compassMat;
    public int dowsing;
    public Material dowsingMat;
    public int emf;
    public Material emfMat;
    public int motion;
    public Material motionMat;
    public int thrown;
    public Material thrownMat;

    public Texture2D captureImage;
    public float captureScore;

    public List<FocalEvidence> videoEvidences = new List<FocalEvidence>();
    public List<FocalEvidence> photoEvidences = new List<FocalEvidence>();

    public List<AudibleEvidence> audioEvidences = new List<AudibleEvidence>();

    public bool recording;

    public long frameNextAt;

    public List<Texture2D> currentVideoFrames;

    public List<float> currentAudioSound = new List<float>();

    public AudioInput audioScript;

    public bool analysisRunning;

    public Color recordColor;
    public Color stopColor;

    public Sprite recordSymbol;
    public Sprite stopSymbol;

    public Text recStatus;

    public Image recSymbolImage;
    private Image cameraTimerImage;
    private Image videoTimerImage;
    private Image audioTimerImage;
    private Image videoButtonRingImage;
    private Image audioButtonRingImage;


    // Start is called before the first frame update
    private void Start()
    {
        audioButtonRingImage = audioButtonRing.gameObject.GetComponent<Image>();
        videoButtonRingImage = videoButtonRing.gameObject.GetComponent<Image>();
        audioTimerImage = audioButtonTimer.gameObject.GetComponent<Image>();
        videoTimerImage = videoButtonTimer.gameObject.GetComponent<Image>();
        cameraTimerImage = cameraButtonTimer.gameObject.GetComponent<Image>();
        cameraButtonHideTimer = 0f;
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (recording)
        {
            recSymbolImage.sprite = recordSymbol;
            if (Mathf.RoundToInt(currentVideoFrames.Count / 10) % 2 == 0)
            {
                recStatus.text = "REC";
            }
            else
            {
                recStatus.text = "";
            }
            recStatus.color = recordColor;
        }
        else
        {
            recSymbolImage.sprite = stopSymbol;
            recStatus.text = "STOP";
            recStatus.color = stopColor;
        }
        float amt = 0f;
        if (Tutorial.inTutorial)
        {
            amt = 2f;
        }
        else
        {
            amt = 0.25f;
        }
        cameraButtonHideTimer -= amt * Time.deltaTime;
        videoButtonHideTimer -= amt * Time.deltaTime;
        audioButtonHideTimer -= amt * Time.deltaTime;
        Color tempColor = cameraFlash.color;
        tempColor.a = Mathf.MoveTowards(tempColor.a,0f,1f*Time.deltaTime);
        tempColor.a = Mathf.Min(tempColor.a, 1f);
        cameraFlash.color = tempColor;
        if (controlScript.heldPhotoTex || !controlScript.ghostVisionActive)
        {
        }
        else
        {
            if (cameraButtonHideTimer<=0f) {
                cameraButton.gameObject.SetActive(true);
                cameraButtonTimer.gameObject.SetActive(false);
                cameraButtonTimerBack.gameObject.SetActive(false);
            }
            else
            {
                cameraButton.gameObject.SetActive(false);
                cameraButtonTimer.gameObject.SetActive(true);
                cameraButtonTimerBack.gameObject.SetActive(true);
                cameraTimerImage.fillAmount = 1f-cameraButtonHideTimer;
            }
            if (videoButtonHideTimer <= 0f && !recording)
            {
                videoButton.gameObject.SetActive(true);
                videoButtonTimer.gameObject.SetActive(false);
                videoButtonTimerBack.gameObject.SetActive(false);
            }
            else
            {
                videoButton.gameObject.SetActive(false);
                videoButtonTimer.gameObject.SetActive(true);
                videoButtonTimerBack.gameObject.SetActive(true);
                if (!recording) {
                    videoTimerImage.fillAmount = 1f - videoButtonHideTimer;
                }
                else
                {
                    videoTimerImage.fillAmount = 0f;
                }
            }
            if (audioButtonHideTimer <= 0f && !audioScript.audioRecording)
            {
                audioButton.gameObject.SetActive(true);
                audioButtonTimer.gameObject.SetActive(false);
                audioButtonTimerBack.gameObject.SetActive(false);
            }
            else
            {
                audioButton.gameObject.SetActive(false);
                audioButtonTimer.gameObject.SetActive(true);
                audioButtonTimerBack.gameObject.SetActive(true);
                if (!audioScript.audioRecording)
                {
                    audioTimerImage.fillAmount = 1f - audioButtonHideTimer;
                }
                else
                {
                    audioTimerImage.fillAmount = 0f;
                }
            }
        }
        videoButtonRingImage.fillAmount = currentVideoFrames.Count * (1f / ((float)videoFramesPerSecond * 15f));
        if (audioScript.audioRecording) {
            audioButtonRingImage.fillAmount = (float)Microphone.GetPosition(null) / (float)(audioScript.frequency * 15);
        }
        else {
            audioButtonRingImage.fillAmount = 0f;
        }
        if (recording)
        {
            videoButton.enabled = false;
        }
        if (recording && System.DateTime.Now.Ticks>=frameNextAt && currentVideoFrames.Count < videoFramesPerSecond*15)
        {
            TakeVideoFrame();
            frameNextAt += (System.TimeSpan.TicksPerSecond / videoFramesPerSecond);
        }
        if (recording && currentVideoFrames.Count>=videoFramesPerSecond*15 && !analysisRunning)
        {
            StartCoroutine(SaveVideo());
            recording = false;
            videoButtonHideTimer = 100f;
            Instantiate(recordingEndSound, transform.position, transform.rotation);
        }
    }
    // ReSharper disable Unity.PerformanceAnalysis
    public IEnumerator SaveVideo()
    {
        List<byte> videoArray = new List<byte>();
        for (int r = 0; r < currentVideoFrames.Count; r++)
        {
            byte[] tempFrame = currentVideoFrames[r].GetRawTextureData();
            videoArray.AddRange(tempFrame);
            yield return null;
        }
        saveScript.AddEvidenceToDesk(currentVideoFrames[0].width, currentVideoFrames[0].height, null, videoArray.ToArray(), null, "video", totalVideoScore, new List<FocalEvidence>(videoEvidences), null);
        yield return null;
        videoEvidences.Clear();
        currentVideoFrames.Clear();
        yield return null;
        totalVideoScore = 0f;
        saveScript.WriteFile();
        videoButtonHideTimer = 1f;
        videoButton.enabled = true;
        ShowButtons();
    }
    public void TakePhoto()
    {
        if (!analysisRunning)
        {
            if (saveScript.gameData.deskEvidence.Count < saveScript.gameData.deskSlots) {
                float scaleAmt = 512f / detectionCam.pixelHeight;
                photoEvidences.Clear();
                detectionCam.cullingMask = photoDetectionSettings;
                Capture(detectionCam, "photoDetect", 1f);
                mainCam.cullingMask = photoSettings;
                Capture(mainCam, "photoImage", scaleAmt);
                mainCam.cullingMask = mainSettings;

                Color tempColor = cameraFlash.color;
                tempColor.a = 100f;
                cameraFlash.color = tempColor;

                saveScript.AddEvidenceToDesk(captureImage.width, captureImage.height, captureImage.GetRawTextureData(), null, null, "photo", captureScore, new List<FocalEvidence>(photoEvidences), null);
                photoEvidences.Clear();

                Instantiate(cameraSound, transform.position, transform.rotation);
                cameraButtonHideTimer = 1f;
                saveScript.WriteFile();
            }
            else
            {
                controlScript.popupScript.DisplayError("DESK IS FULL");
            }
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void TakeVideoFrame()
    {
        float scaleAmt = 128f / detectionCam.pixelHeight;
        if (!analysisRunning && currentVideoFrames.Count%5==0) {
            detectionCam.cullingMask = videoDetectionSettings;
            Capture(detectionCam, "photoDetect", 1f);
        }
        mainCam.cullingMask = videoSettings;
        Capture(mainCam, "photoImage", scaleAmt);
        mainCam.cullingMask = mainSettings;

        currentVideoFrames.Add(captureImage);
    }
    public void TakeVideo()
    {
        if (!analysisRunning && saveScript.gameData.deskEvidence.Count< saveScript.gameData.deskSlots) {
            HideButtons();
            videoEvidences.Clear();
            recording = true;
            totalVideoScore = 0f;
            currentVideoFrames.Clear();
            frameNextAt = System.DateTime.Now.Ticks;

            Instantiate(recordingStartSound, transform.position, transform.rotation);
        }
        else
        {
            controlScript.popupScript.DisplayError("DESK IS FULL");
        }
    }
    public void TakeSoundRecording()
    {
        if (!analysisRunning && saveScript.gameData.deskEvidence.Count < saveScript.gameData.deskSlots)
        {
            audioEvidences.Clear();
            HideButtons();
            currentAudioSound.Clear();
            //start recording the microphone
            audioScript.BeginRecording();

            Instantiate(recordingStartSound, transform.position, transform.rotation);
        }
        else
        {
            controlScript.popupScript.DisplayError("DESK IS FULL");
        }
    }
    bool ColorsAreClose(Color a, Color z, float threshold = 0.01f)
    {
        float r = (a.r - z.r),
            g = (a.g - z.g),
            b = (a.b - z.b);
        return (r * r + g * g + b * b) <= threshold * threshold;
    }
    public void Capture(Camera useCam,string fileName, float sizeMulti)
    {
        if (fileName.Contains("Detect")) {
            useCam.targetTexture = new RenderTexture(Mathf.FloorToInt(128 *sizeMulti), Mathf.FloorToInt(128 * sizeMulti), 32);
        }
        else
        {
            useCam.targetTexture = new RenderTexture((Mathf.FloorToInt((useCam.pixelWidth* sizeMulti)/4)*4), (Mathf.FloorToInt((useCam.pixelHeight* sizeMulti) / 4)*4), 24);
        }

        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = useCam.targetTexture;

        useCam.Render();
        Texture2D image;
        if (recording) {
            if (fileName.Contains("Detect"))
            {
                image = new Texture2D(useCam.targetTexture.width, useCam.targetTexture.height, TextureFormat.RGB24, false);
            }
            else
            {
                image = new Texture2D(useCam.targetTexture.width, useCam.targetTexture.height, Controller.videoFormat, false);
            }
        }
        else
        {
            if (fileName.Contains("Detect"))
            {
                image = new Texture2D(useCam.targetTexture.width, useCam.targetTexture.height,TextureFormat.RGB24, false);
            }
            else
            {
                image = new Texture2D(useCam.targetTexture.width, useCam.targetTexture.height, Controller.photoFormat, false);
            }
        }
        image.ReadPixels(new Rect(0, 0, useCam.targetTexture.width, useCam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        if (fileName.Contains("Detect"))
        {
            Analyze(image, fileName,recording);
        }
        else
        {
            controlScript.photos.Insert(0,image);
        }

        File.WriteAllBytes(Application.persistentDataPath+"/"+fileName+".png", image.EncodeToPNG());

        useCam.targetTexture = null;
        captureImage = image;
    }

    void AddFocalEvidence(string type, int x, int y)
    {
        bool found = false;
        if (!recording)
        {
            for (int e = 0; e < photoEvidences.Count; e++)
            {
                if (photoEvidences[e].type == type)
                {
                    photoEvidences[e].SetX((x + photoEvidences[e].x) / 2);
                    photoEvidences[e].SetY((y + photoEvidences[e].y) / 2);
                    photoEvidences[e].SetPixels(photoEvidences[e].pixels+1);
                    found = true;
                }
            }
            if (!found)
            {
                photoEvidences.Add(new FocalEvidence(type, x, y, 1, 0f));
            }
        }
        if (recording)
        {
            for (int e = 0; e < videoEvidences.Count; e++)
            {
                FocalEvidence currentEv = videoEvidences[e];
                if (currentEv.type == type)
                {
                    currentEv.SetX((x + videoEvidences[e].x) / 2);
                    currentEv.SetY((y + videoEvidences[e].y) / 2);
                    currentEv.SetPixels(currentEv.pixels+1);
                    videoEvidences[e] = currentEv;
                    found = true;
                }
            }
            if (!found)
            {
                videoEvidences.Add(new FocalEvidence(type, x, y, 1, 0f));
            }
        }
        
    }
    public void Analyze(Texture2D detection,string fileName,bool video)
    {
        analysisRunning = true;


        handprint = 0;
        ghostorb = 0;
        ghosttrail = 0;
        ghostbody = 0;
        ghostfoot = 0;
        ghosthand = 0;
        ghosthead = 0;
        candle = 0;
        clipboard = 0;
        compass = 0;
        dowsing = 0;
        emf = 0;
        motion = 0;
        thrown = 0;
        //yield return null;

        Color[] pic = detection.GetPixels(0, 0, detection.width, detection.height);
        for (int i = 0; i<pic.Length;i++)
        {
            int currentX = i % detection.width;
            int currentY = i / detection.width;
            if (pic[i]!=Color.black)
            {
                if (ColorsAreClose(pic[i],handprintMat.color))
                {
                    handprint += 1;
                    AddFocalEvidence("handprint",currentX,currentY);
                }
                if (ColorsAreClose(pic[i],ghostorbMat.color))
                {
                    ghostorb += 1;
                    AddFocalEvidence("ghostorb", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], ghosttrailMat.color))
                {
                    ghosttrail += 1;
                    AddFocalEvidence("ghosttrail", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], ghostbodyMat.color))
                {
                    ghostbody += 1;
                    AddFocalEvidence("ghostbody", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], ghostfootMat.color))
                {
                    ghostfoot += 1;
                    AddFocalEvidence("ghostfoot", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], ghosthandMat.color))
                {
                    ghosthand += 1;
                    AddFocalEvidence("ghosthand", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], ghostheadMat.color))
                {
                    ghosthead += 1;
                    AddFocalEvidence("ghosthead", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], candleMat.color))
                {
                    candle += 1;
                    AddFocalEvidence("candle", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], clipboardMat.color))
                {
                    clipboard += 1;
                    AddFocalEvidence("clipboard", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], compassMat.color))
                {
                    compass += 1;
                    AddFocalEvidence("compass", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], dowsingMat.color))
                {
                    dowsing += 1;
                    AddFocalEvidence("dowsing", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], emfMat.color))
                {
                    emf += 1;
                    AddFocalEvidence("emf", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], motionMat.color))
                {
                    motion += 1;
                    AddFocalEvidence("motion", currentX, currentY);
                }
                if (ColorsAreClose(pic[i], thrownMat.color))
                {
                    thrown += 1;
                    AddFocalEvidence("thrown", currentX, currentY);
                }
            }
        }


        Destroy(detection);
        analysisRunning = false;
    }
    [System.Serializable]
    public class FocalEvidence
    {
        public string type;
        public int x;
        public int y;
        public int pixels;
        public float score;
        public FocalEvidence(string newType, int newX, int newY, int newPixels, float newScore)
        {
            this.type = newType;
            this.x = newX;
            this.y = newY;
            this.pixels = newPixels;
            this.score = newScore;
        }
        public void SetX(int newX)
        {
            this.x = newX;
        }
        public void SetY(int newY)
        {
            this.y = newY;
        }
        public void SetPixels(int newPixels)
        {
            this.pixels = newPixels;
        }
        public void SetScore()
        {
            this.score = 0f;
            if (type == "handprint")
            {
                this.score += (30f)*Mathf.Min(30,pixels);
            }
            if (type == "ghostorb")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "ghosttrail")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "ghostbody")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "ghostfoot")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "ghosthand")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "ghosthead")
            {
                this.score += (20f) * Mathf.Min(30, pixels);
            }
            if (type == "candle")
            {
                this.score += (5f) * Mathf.Min(30, pixels);
            }
            if (type == "dowsing")
            {
                this.score += (5f) * Mathf.Min(30, pixels);
            }
            if (type == "emf")
            {
                this.score += (5f) * Mathf.Min(30, pixels);
            }
            if (type == "motion")
            {
                this.score += (5f) * Mathf.Min(30, pixels);
            }
            if (type == "thrown")
            {
                this.score += (10f) * Mathf.Min(30, pixels);
            }
        }
    }
    [System.Serializable]
    public class AudibleEvidence
    {
        public string word;
        public float distance;
        public float score;
        public int position;
        public AudibleEvidence(string newWord, float newDistance, float newScore, int newPosition)
        {
            this.word = newWord;
            this.distance = newDistance;
            this.score = newScore;
            this.position = newPosition;
        }
        public void SetWord(string newWord)
        {
            this.word = newWord;
        }
        public void SetDistance(float newDistance)
        {
            this.distance = newDistance;
        }
        public void SetScore()
        {
            this.score = 0f;
            if (word=="behind")
            {
                this.score += (10f) * (1f-Mathf.Min(1f,distance));
            }
            if (word == "away")
            {
                this.score += (10f) * (1f - Mathf.Min(1f, distance));
            }
            if (word == "goaway")
            {
                this.score += (10f) * (1f - Mathf.Min(1f, distance));
            }
            if (word == "river")
            {
                this.score += (10f) * (1f - Mathf.Min(1f, distance));
            }
            if (word == "murder")
            {
                this.score += (10f) * (1f - Mathf.Min(1f, distance));
            }
        }
        public void SetPosition(int newPosition)
        {
            this.position = newPosition;
        }
    }
    public void HideButtons()
    {
        for (int i = 0; i< hideDuringRecording.Count;i++)
        {
            hideDuringRecording[i].SetActive(false);
        }
    }
    public void ShowButtons()
    {
        for (int i = 0; i < hideDuringRecording.Count; i++)
        {
            hideDuringRecording[i].SetActive(true);
        }
    }
}
