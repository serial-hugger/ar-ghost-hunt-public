using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject menuCamera;
    public Vector3 cameraGoTo;

    public GameObject menuSound;

    public SaveManager saveScript;
    public Ads adScript;

    public TextMeshProUGUI versionText;

    public TextMeshProUGUI prewatchedText;
    // Start is called before the first frame update
    void Start()
    {
        versionText.text = "Ver. "+Application.version;
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        if (saveScript.isSaveInvalid && !Application.isEditor)
        {
            menuCamera.transform.position = new Vector3(360, -1288, -994);
        }
        else
        {
            menuCamera.transform.position = new Vector3(360, 760, -994);
        }
        cameraGoTo = menuCamera.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        menuCamera.transform.position = Vector3.Lerp(menuCamera.transform.position,cameraGoTo,10f*Time.deltaTime);
        prewatchedText.text = "Prewatched Ads: " + saveScript.gameData.prewatchedAds;
    }
    public void OpenSettings()
    {
        Instantiate(menuSound, menuCamera.transform.position, transform.rotation);
        cameraGoTo = new Vector3(1896, 760, -994);
    }
    public void OpenMain()
    {
        Instantiate(menuSound, menuCamera.transform.position, transform.rotation);
        cameraGoTo = new Vector3(360, 760, -994);
    }
    public void PrewatchAd()
    {
        adScript.ShowRewardedAd("prewatched");
    }
    public void OpenMainSaveSetting()
    {
        saveScript.WriteSetting();
        Instantiate(menuSound, menuCamera.transform.position, transform.rotation);
        cameraGoTo = new Vector3(360, 760, -994);
    }
    public void OpenCredits()
    {
        Instantiate(menuSound, menuCamera.transform.position, transform.rotation);
        cameraGoTo = new Vector3(-1176, -1288, -994);
    }
    public void LaunchGame()
    {
        Tutorial.inTutorial = false;
        saveScript.WriteFile();
        saveScript.ReadFile();
        SceneManager.LoadScene("PreGame");
    }
    public void LaunchTutorial()
    {
        Tutorial.inTutorial = true;
        SceneManager.LoadScene("PreGame");
    }
    public void OpenPrivacyPolicy()
    {
        Application.OpenURL("https://pseudosparkgames.com/arghosthunt-privacy");
    }
    public void CreateNewSave()
    {
        saveScript.NewSave();
    }
    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/kvPNmfw6B4");
    }
}
