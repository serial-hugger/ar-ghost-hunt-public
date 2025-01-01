using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public RectTransform appsUI;
    public Image appsPanel;
    public GameObject appsClose;
    public GameObject cameraUI;
    public GameObject videoUI;
    public GameObject soundUI;
    public GameObject stashUI;
    public GameObject messagesUI;
    public GameObject analyzeUI;
    public GameObject shopUI;
    public GameObject defaultUI;
    public GameObject placeUI;
    public GameObject settingsUI;
    public GameObject dailyUI;
    public GameObject premiumUI;
    public GameObject helpUI;
    public GameObject reviewUI;

    public int notched;

    public GameObject timeErrorText;
    public GameObject timeErrorButton;

    public Button analysisButton;
    public Button messageButton;

    //playback windows
    public PhotoMediaPlayback playbackPhotoUI;
    public VideoMediaPlayback playbackVideoUI;
    public SoundMediaPlayback playbackSoundUI;

    public string currentMenu;
    public string prevMenu;

    public Controller controlScript;

    public StashUI stashScript;

    public GameObject openSound;
    public GameObject closeSound;
    public GameObject cameraSound;

    public Image levelBack;
    public TextMeshProUGUI experienceText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI moneyText;

    public Image experienceFill;

    public GameObject infoObject;
    public GameObject levelObject;
    public GameObject moneyObject;

    public GameObject premiumObject;

    public GameObject appsHolder;

    public Button tripodButton;
    public RectTransform tripodRect;

    public GameObject adBack;

    public float displayMoney = -1f;
    public float displayExperience = -1f;

    public GameObject zoomer;
    public Vector2 zoomerStart;
    public int zoomerXpNeeded = 0;
    public int zoomerMoneyNeeded = 0;

    public float timeTillNextZoomer = 0f;

    public LevelUp levelScript;
    public int prevLevel;
    public int prevExp;
    private Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;
        adBack.SetActive(false);
        notched = Screen.height - Mathf.RoundToInt(Screen.safeArea.height);
    }

    // Update is called once per frame
    void Update()
    {
        timeTillNextZoomer -= 1f * Time.deltaTime;
        if (timeTillNextZoomer<=0f)
        {
            timeTillNextZoomer = 0.01f;
            if (zoomerXpNeeded>0)
            {
                zoomerXpNeeded--;
                IncreaseZoomer zoomerScript = Instantiate(zoomer,transform).GetComponent<IncreaseZoomer>();
                zoomerScript.render.sprite = zoomerScript.xpSprite;
                zoomerScript.transform.position = zoomerStart;
                zoomerScript.goPos = levelObject.transform.position;
            }
            if (zoomerMoneyNeeded > 0)
            {
                zoomerMoneyNeeded--;
                IncreaseZoomer zoomerScript = Instantiate(zoomer,transform).GetComponent<IncreaseZoomer>();
                zoomerScript.render.sprite = zoomerScript.moneySprite;
                zoomerScript.transform.position = zoomerStart;
                zoomerScript.goPos = moneyObject.transform.position;
            }
        }
        if (controlScript.saveScript && displayMoney == -1)
        {
            displayMoney = controlScript.saveScript.gameData.money;
            displayExperience = controlScript.saveScript.gameData.experience;
            prevLevel = controlScript.saveScript.GetPlayerLevel(controlScript.saveScript.gameData.experience);
            prevExp = controlScript.saveScript.gameData.experience;
        }
        if (controlScript.saveScript)
        {
            if (prevExp != displayExperience)
            {
                prevExp = (int)displayExperience;
                if (prevLevel != controlScript.saveScript.GetPlayerLevel((int)displayExperience))
                {
                    prevLevel = controlScript.saveScript.GetPlayerLevel((int)displayExperience);
                    levelScript.levelUp(prevLevel);
                }
            }
        }
        displayMoney = Mathf.MoveTowards(displayMoney, controlScript.saveScript.gameData.money,300f*Time.deltaTime);
        displayExperience = Mathf.MoveTowards(displayExperience, controlScript.saveScript.gameData.experience, 300f * Time.deltaTime);
        if (TimeManager.validTime)
        {
            timeErrorText.SetActive(false);
            timeErrorButton.SetActive(false);
            analysisButton.interactable = true;
            messageButton.interactable = true;
        }
        else
        {
            timeErrorText.SetActive(true);
            timeErrorButton.SetActive(true);
            analysisButton.interactable = false;
            messageButton.interactable = false;
        }
        if (currentMenu!="apps")
        {
            timeErrorText.SetActive(false);
            timeErrorButton.SetActive(false);
        }
        //show premium button if the current menu is default
        if ((currentMenu=="default")&&!Tutorial.inTutorial&&!controlScript.saveScript.gameData.premium)
        {
            premiumObject.SetActive(true);
        }
        else
        {
            premiumObject.SetActive(false);
        }
        if (controlScript.heldEquipment && (controlScript.heldEquipment.liveCamera || controlScript.heldEquipment.uv))
        {
            tripodRect.pivot = Vector2.Lerp(tripodRect.pivot, new Vector2(0.1f, .5f), 10f * Time.deltaTime);
            Vector3 tempPos = tripodButton.transform.position;
            tempPos.x = 0f;
            tripodButton.transform.position = tempPos;
            
        }
        else
        {
            tripodRect.pivot = Vector2.Lerp(tripodRect.pivot, new Vector2(1.1f, .5f), 10f * Time.deltaTime);
            Vector3 tempPos = tripodButton.transform.position;
            tempPos.x = 0f;
            tripodButton.transform.position = tempPos;
        }

        if (controlScript.canTripod)
        {
            tripodButton.image.color = Color.white;
        }
        else
        {
            tripodButton.image.color = Color.gray;
        }
        if (currentMenu is "camera" or "video" or "default" or "none")
        {
            Color temp = appsPanel.color;
            temp.a = Mathf.Lerp(temp.a, 0f, 15f * Time.deltaTime);
            appsPanel.color = temp;
        }
        else
        {
            Color temp = appsPanel.color;
            temp.a = Mathf.Lerp(temp.a, 0.5f, 15f * Time.deltaTime);
            appsPanel.color = temp;
        }
        if (currentMenu is "none" or "camera" or "video" or "sound" or "stash")
        {
            levelObject.SetActive(false);
            moneyObject.SetActive(false);
        }
        else
        {
            levelObject.SetActive(true);
            moneyObject.SetActive(true);
            if (currentMenu == "default")
            {
                infoObject.transform.localScale = Vector3.Lerp(infoObject.transform.localScale, new Vector3(1f,1f,1f),5f*Time.deltaTime);
            }
            else
            {
                infoObject.transform.localScale = Vector3.Lerp(infoObject.transform.localScale, new Vector3(.5f, .5f, .5f), 5f * Time.deltaTime);

            }
            if (controlScript.saveScript.gameData.premium)
            {
                infoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0-notched);
                premiumObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0-notched);
            }
            else
            {
                infoObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0-(controlScript.adHeight+notched+10));
                premiumObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0 - (controlScript.adHeight+notched+10));
            }
        }
        moneyText.text = "$" + (int)displayMoney;
        experienceText.text = "Exp: " + controlScript.saveScript.GetPlayerSubExperience((int)displayExperience)+"/"+ controlScript.saveScript.GetPlayerNextLevel((int)displayExperience);
        levelText.text = controlScript.saveScript.GetPlayerLevel((int)displayExperience).ToString();
        experienceFill.fillAmount = (float)controlScript.saveScript.GetPlayerSubExperience((int)displayExperience) / (float)controlScript.saveScript.GetPlayerNextLevel((int)displayExperience);
        if (currentMenu == "none" && !controlScript.placingToolDesk)
        {
            placeUI.SetActive(true);
        }
        else
        {
            placeUI.SetActive(false);
        }
        if (currentMenu == "default" && !controlScript.heldEquipment)
        {
            defaultUI.SetActive(true);
            controlScript.selectedEvidences = new List<string>();
            controlScript.selectQuestItems = new List<QuestItem>();
        }
        else
        {
            defaultUI.SetActive(false);
        }
        //show/hide app UIs
        if (currentMenu == "camera")
        {
            cameraUI.SetActive(true);
        }
        else
        {
            cameraUI.SetActive(false);
        }
        if (currentMenu == "video")
        {
            videoUI.SetActive(true);
        }
        else
        {
            videoUI.SetActive(false);
        }
        if (currentMenu == "sound")
        {
            soundUI.SetActive(true);
        }
        else
        {
            soundUI.SetActive(false);
        }
        if (currentMenu == "stash")
        {
            stashUI.SetActive(true);
        }
        else
        {
            stashUI.SetActive(false);
        }
        if (currentMenu == "messages")
        {
            messagesUI.SetActive(true);
        }
        else
        {
            messagesUI.SetActive(false);
        }
        if (currentMenu == "analyze")
        {
            analyzeUI.SetActive(true);
        }
        else
        {
            analyzeUI.SetActive(false);
        }
        if (currentMenu == "shop")
        {
            shopUI.SetActive(true);
        }
        else
        {
            shopUI.SetActive(false);
        }
        if (currentMenu == "help")
        {
            helpUI.SetActive(true);
        }
        else
        {
            helpUI.SetActive(false);
        }
        if (currentMenu == "daily")
        {
            dailyUI.SetActive(true);
        }
        else
        {
            dailyUI.SetActive(false);
        }
        if (currentMenu == "premium")
        {
            premiumUI.SetActive(true);
        }
        else
        {
            premiumUI.SetActive(false);
        }
        //show/hide playback UIs
        if (currentMenu == "playbackphoto")
        {
            playbackPhotoUI.gameObject.SetActive(true);
        }
        else
        {
            playbackPhotoUI.gameObject.SetActive(false);
        }
        if (currentMenu == "playbackvideo")
        {
            playbackVideoUI.gameObject.SetActive(true);
        }
        else
        {
            playbackVideoUI.gameObject.SetActive(false);
        }
        if (currentMenu == "playbacksound")
        {
            playbackSoundUI.gameObject.SetActive(true);
        }
        else
        {
            playbackSoundUI.gameObject.SetActive(false);
        }
        if (currentMenu == "settings")
        {
            settingsUI.gameObject.SetActive(true);
        }
        else
        {
            settingsUI.gameObject.SetActive(false);
        }
        if (currentMenu == "review")
        {
            reviewUI.gameObject.SetActive(true);
        }
        else
        {
            reviewUI.gameObject.SetActive(false);
        }
        //hide/show app menu UI
        if (currentMenu == "apps")
        {
            UpdateApps();
            appsClose.SetActive(true) ;
            Vector2 tempPos = appsUI.localPosition;
            tempPos.y = Mathf.Lerp(tempPos.y, 0f, 5f * Time.deltaTime);
            appsUI.localPosition = tempPos;
            //Grows the apps
            Vector2 tempScale = appsUI.localScale;
            tempScale.x = Mathf.Lerp(tempScale.x, 1f, 5f * Time.deltaTime) ;
            tempScale.y = Mathf.Lerp(tempScale.y, 1f, 5f * Time.deltaTime);
            appsUI.localScale = tempScale;
        }
        else
        {
            appsClose.SetActive(false);
            Vector2 tempPos = appsUI.localPosition;
            tempPos.y = Mathf.Lerp(tempPos.y, -2000f, 1f * Time.deltaTime);
            appsUI.localPosition = tempPos;
            //Shrinks the apps
            Vector2 tempScale = appsUI.localScale;
            tempScale.x = Mathf.Lerp(tempScale.x, 0f, 2f * Time.deltaTime);
            tempScale.y = Mathf.Lerp(tempScale.y, 0f,2f * Time.deltaTime);
            appsUI.localScale = tempScale;
        }
    }
    public void RetryConnect()
    {
        TimeManager.SetTime();
    }
    public void UpdateApps()
    {
        for (int i = 0; i < appsHolder.transform.childCount;i++)
        {
            if (IsDefault(appsHolder.transform.GetChild(i).name) || controlScript.saveScript.CheckIfBought(appsHolder.transform.GetChild(i).name))
            {
                appsHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                appsHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    public bool IsDefault(string name)
    {
        if (name is "camera" or "analyze" or "quest" or "stash" or "shop" or "setting")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetCurrentMenu(string menu)
    {
        prevMenu = currentMenu;
        if (currentMenu!="none"&&menu == "default")
        {
            Instantiate(closeSound, _camera.transform.position, _camera.transform.rotation);
            controlScript.selectQuestItems.Clear();
            controlScript.selectedEvidences.Clear();
            controlScript.archiveSelectAllowed = true;
            controlScript.deskSelectAllowed = true;
        }
        currentMenu = menu;
        if (menu == "apps")
        {
            Instantiate(openSound, _camera.transform.position, _camera.transform.rotation);
        }
        if (menu is "camera" or "video" or "sound")
        {
            Instantiate(cameraSound, _camera.transform.position, _camera.transform.rotation);
        }
        if (menu is "shop" or "messages" or "analyze" or "stash" or "settings")
        {
            Instantiate(openSound, _camera.transform.position, _camera.transform.rotation);
        }
        controlScript.adScript.timeTillUpdate = 0f;
    }
    public void Close()
    {
        Instantiate(closeSound, _camera.transform.position, _camera.transform.rotation);
        prevMenu = "default";
        if (controlScript.selectQuestItems.Count>0 && currentMenu == "stash")
        {
            if (!controlScript.archiveSelectAllowed) {
                controlScript.selectedEvidences = new List<string>();
                controlScript.selectQuestItems = new List<QuestItem>();
                currentMenu = "analyze";
            }
            if (controlScript.archiveSelectAllowed&&controlScript.deskSelectAllowed)
            {
                currentMenu = "messages";
            }
            controlScript.selectQuestItems.Clear();
            controlScript.deskSelectAllowed = true;
            controlScript.archiveSelectAllowed = true;
            return;
        }
        if (controlScript.selectQuestItems.Count > 0 && currentMenu is "playbackphoto" or "playbackvideo" or "playbacksound")
        {
            currentMenu = "stash";
            return;
        }
        currentMenu = prevMenu;
    }
}
