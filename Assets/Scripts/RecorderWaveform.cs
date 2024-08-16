using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecorderWaveform : MonoBehaviour
{
    public GameObject dotPrefab;
    public GameObject[] dots = new GameObject[1024];

    public AudioInput inputScript;

    public Transform dotsHolder;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 1024;i++)
        {
            GameObject newDot = Instantiate(dotPrefab,this.transform);
            newDot.transform.localPosition = new Vector3(-320f+(i*.625f),0f,0f);
            dots[i] = newDot;
            dots[i].transform.SetParent(dotsHolder);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputScript.IsMicrophonePermissionGranted()) {
            float[] wf = inputScript.displayWaveform;
            if (!inputScript.audioRecording) {
                for (int i = 0; i < 1024; i++)
                {
                    float size = (1f - ((Mathf.Abs(512f - i)) / 512f));
                    dots[i].transform.localScale = new Vector3(1f, Mathf.Lerp(dots[i].transform.localScale.y, Mathf.Max(-100f, Mathf.Min(100f, wf[i] * 1000f)) * size, 10f * Time.deltaTime), 1f);
                }
            }
            else
            {
                float increaseAmt = (inputScript.frequency * 15) / 1024;
                for (int i = 0; i < 1024; i++)
                {
                    float size = (1f);
                    dots[i].transform.localScale = new Vector3(1f, Mathf.Lerp(dots[i].transform.localScale.y, Mathf.Max(-300f, Mathf.Min(300f, wf[(i * Mathf.FloorToInt(increaseAmt))] * 1000f)) * size, 10f * Time.deltaTime), 1f);
                }
            }
        }
    }
    public void ShowBackOfDots()
    {
        for (int i = 0; i< dots.Length;i++)
        {
            dots[i].transform.GetChild(0).gameObject.SetActive(true);
        }
    }
    public void HideBackOfDots()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
