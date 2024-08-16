using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PreGame : MonoBehaviour
{
    public SaveManager saveScript;
    public Ads adScript;

    bool finishedLoadingScene;
    public Image logo;
    public bool loading;
    public float timeLoading;

    public GameObject launchButton;
    public GameObject warningText;

    AsyncOperation asyncLoad = null;

    public AudioSource myAudio;

    private bool Active;
    private AndroidJavaObject camera1;
    // Start is called before the first frame update
    void Start()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        adScript = GameObject.Find("GameManager").GetComponent<Ads>();
        adScript.timeTillUpdate = 0f;
        Color tempColor = logo.color;
        tempColor.a = 0f;
        logo.color = tempColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (loading)
        {
            if (true||Application.isEditor) {
                launchButton.SetActive(false);
                warningText.SetActive(false);
                Color tempColor = logo.color;
                tempColor.a = Mathf.MoveTowards(tempColor.a, 1f, 0.4f * Time.deltaTime);
                logo.color = tempColor;

                if (asyncLoad == null)
                {
                    if (Tutorial.inTutorial)
                    {
                        StartCoroutine(StartTutorial());
                        saveScript.gameData = new GameData();
                        saveScript.SetupMessages();
                    }
                    else
                    {
                        StartCoroutine(StartGame());
                        saveScript.ReadFile();
                    }
                }
                if (finishedLoadingScene) {
                    timeLoading += 1f * Time.deltaTime;
                    if (!myAudio.isPlaying && timeLoading >= 5f)
                    {
                        asyncLoad.allowSceneActivation = true;
                    }
                }
            }
        }
    }
    IEnumerator StartGame()
    {
        saveScript.WriteFile();
        asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;
        if (!asyncLoad.isDone)
        {
            yield return null;
        }
        finishedLoadingScene = true;
    }
    IEnumerator StartTutorial()
    {
        saveScript.WriteFile();
        asyncLoad = SceneManager.LoadSceneAsync("Game");
        asyncLoad.allowSceneActivation = false;
        if (!asyncLoad.isDone)
        {
            yield return null;
        }
        finishedLoadingScene = true;
    }
    public void Boot()
    {
        if (saveScript.settingData.music) {
            myAudio.Play();
        }
        loading = true;
    }
}
