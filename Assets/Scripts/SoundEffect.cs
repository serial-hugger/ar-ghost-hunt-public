using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffect : MonoBehaviour
{
    public AudioClip mySound;
    public AudioSource mySource;
    public float maxPitch = 1f;
    public float minPitch = 1f;
    public SaveManager saveScript;

    // Start is called before the first frame update
    void Start()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        if (!saveScript.settingData.sounds)
        {
            GameObject.Destroy(gameObject);
        }
        mySource.clip = mySound;
        mySource.pitch = Random.Range(minPitch,maxPitch);
        mySource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mySource.isPlaying)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
