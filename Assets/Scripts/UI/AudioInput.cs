using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
#endif

public class AudioInput : MonoBehaviour
{
    public AudioClip microphoneClip;
    public AudioClip recordedMicrophoneClip;
    public float[] displayWaveform;
    public SoundMediaPlayback playbackScript;

    public EvidenceCapture evidenceScript;

    public int position = 0;
    public int samplerate = 44100;
    public int frequency;

    public int dataOffset = 0;

    public bool audioRecording;

    public long recordingStart;

    public GameObject backingPanel;

    public RecorderWaveform waveScript;

    public GameObject recordLine;

    public float evidenceCheck;
    public float evidenceFind;

    public GameObject evidenceTagPrefab;

    public GameObject tagsHolder;

    public Button exitButton;

    public MainUI uiScript;



    //int freq = 22050;

    private void Awake()
    {
        frequency = 4096;
    }

    // Start is called before the first frame update
    void Start()
    {
        frequency = 4096;
        displayWaveform = new float[frequency * 15];
        microphoneClip = AudioClip.Create("Microphone",1, 1, frequency, true);
        if (IsMicrophonePermissionGranted()) {
            if (Microphone.devices.Length > 0)
            {
                microphoneClip = Microphone.Start(null, true, 1, 1024);
                int breaker = 0;
                while (!(Microphone.GetPosition(null) > 0) && breaker<1000)
                {
                    breaker++;
                }
            }
        }
        evidenceCheck = Random.Range(1f, 3f);
        evidenceFind = Random.Range(-1f, 1f);
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsMicrophonePermissionGranted())
        {
            if (Microphone.devices.Length > 0)
            {
                //time till attempt spawn audio evidence
                evidenceCheck -= 1f * Time.deltaTime;

                if (evidenceCheck <= 0f)
                {
                    evidenceCheck = Random.Range(1f, 3f);
                    evidenceFind = Random.Range(-1f, 1f);
                    if (Microphone.GetPosition(null) > 4096 && Microphone.GetPosition(null) < (frequency * 15) - 4096 && audioRecording)
                    {
                        int micPos = Microphone.GetPosition(null);
                        if (Mathf.Abs((displayWaveform[micPos]) - evidenceFind) <= 0.13f)
                        {
                            List<string> ghostPhrases = new List<string>();
                            foreach (var t in playbackScript.ghostSpeech)
                            {
                                ghostPhrases.Add(t.name);
                            }
                            string say = ghostPhrases[Random.Range(0, ghostPhrases.Count)];
                            evidenceScript.audioEvidences.Add(new EvidenceCapture.AudibleEvidence(say, Random.Range(0f, 1f), 0f, micPos));
                            AddEvidenceMark(micPos);
                        }
                    }
                }
                recordLine.transform.localPosition = new Vector3(-320f + Microphone.GetPosition(null) * (0.042f / 4f), 0f, 0f);
                if (audioRecording)
                {
                    recordLine.SetActive(true);
                    if (!Microphone.IsRecording(null))
                    {
                        Microphone.End(null);
                        microphoneClip = Microphone.Start(null, true, 1, 1024);
                        int breaker = 0;
                        while (!(Microphone.GetPosition(null) > 0)  && breaker<1000)
                        {
                            breaker++;
                        }
                        audioRecording = false;
                        waveScript.ShowBackOfDots();
                        //SET SOUND
                        float[] data = new float[frequency * 15];
                        CloneAudioClip(recordedMicrophoneClip, System.Guid.NewGuid().ToString()).GetData(data, samplerate);
                        evidenceScript.saveScript.AddEvidenceToDesk(-1, -1, null, null, data, "sound", 0f, null, new List<EvidenceCapture.AudibleEvidence>(evidenceScript.audioEvidences));
                        evidenceScript.ShowButtons();
                        evidenceScript.audioEvidences.Clear();
                        evidenceScript.audioButtonHideTimer = 1f;
                        AudioClip.Destroy(recordedMicrophoneClip);
                        foreach (Transform child in tagsHolder.transform)
                        {
                            GameObject.Destroy(child.gameObject);
                        }
                        exitButton.interactable = true;
                        evidenceScript.saveScript.WriteFile();
                    }
                    else
                    {
                        recordedMicrophoneClip.GetData(displayWaveform, 0);
                    }
                    backingPanel.transform.localScale = new Vector3(
                        Mathf.Lerp(backingPanel.transform.localScale.x, 5f, 1f * Time.deltaTime),
                        Mathf.Lerp(backingPanel.transform.localScale.y, 5f, 1f * Time.deltaTime),
                        Mathf.Lerp(backingPanel.transform.localScale.z, 5f, 1f * Time.deltaTime)
                        );
                }
                else
                {
                    recordLine.SetActive(false);
                    microphoneClip.GetData(displayWaveform, Microphone.GetPosition(null));
                    backingPanel.transform.localScale = new Vector3(
                        Mathf.Lerp(backingPanel.transform.localScale.x, 1f, 10f * Time.deltaTime),
                        Mathf.Lerp(backingPanel.transform.localScale.y, 1f, 10f * Time.deltaTime),
                        Mathf.Lerp(backingPanel.transform.localScale.z, 1f, 10f * Time.deltaTime)
                    );
                }
            }
            else
            {
                uiScript.currentMenu = "default";
            }
        }
    }
    public void BeginRecording()
    {
        if (!evidenceScript.analysisRunning && evidenceScript.saveScript.gameData.deskEvidence.Count < evidenceScript.saveScript.gameData.deskSlots)
        {
            exitButton.interactable = false;
            waveScript.HideBackOfDots();
            Microphone.End(null);
            AudioClip.Destroy(microphoneClip);
            recordedMicrophoneClip = AudioClip.Create("Recording", frequency * 15, 1, frequency, false);
            recordingStart = System.DateTime.Now.Ticks;
            if (Microphone.devices.Length > 0)
            {
                recordedMicrophoneClip = Microphone.Start(null, false, 15, frequency);
                audioRecording = true;
                //while (!(Microphone.GetPosition(null) > 0)) { }
            }
        }
    }
    public AudioClip CloneAudioClip(AudioClip audioClip, string newName)
    {
        AudioClip newAudioClip = AudioClip.Create(newName, audioClip.samples, audioClip.channels, audioClip.frequency, false);
        float[] copyData = new float[audioClip.samples * audioClip.channels];
        audioClip.GetData(copyData, 0);
        newAudioClip.SetData(copyData, 0);
        return newAudioClip;
    }
    public void AddEvidenceMark(int position)
    {
        GameObject newEvidence = Instantiate(evidenceTagPrefab, this.transform);
        newEvidence.transform.localPosition = new Vector3(-320f + position * (0.042f/4f), 0f, 0f);
        newEvidence.transform.SetParent(tagsHolder.transform);
    }
    public void OnEnable()
    {
        frequency = 4096;
        if (IsMicrophonePermissionGranted())
        {
            if (Microphone.devices.Length > 0)
            {
                microphoneClip = Microphone.Start(null, true, 1, frequency);
                int breaker = 0;
                while (!(Microphone.GetPosition(null) > 0)  && breaker<1000)
                {
                    breaker++;
                }
            }
        }
    }
    public void OnDisable()
    {
        if (IsMicrophonePermissionGranted())
        {
            if (Microphone.devices.Length > 0)
            {
                Microphone.End(null);
            }
        }
    }
    //MICROPHONE
    public bool IsMicrophonePermissionGranted()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Microphone);
#elif UNITY_IOS
        return Application.HasUserAuthorization(UserAuthorization.Microphone);
#endif
        return true;
    }
}
