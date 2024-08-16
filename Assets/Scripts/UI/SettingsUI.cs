using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Purchasing;

public class SettingsUI : MonoBehaviour
{
    public SaveManager saveScript;

    public TextMeshProUGUI musicButton;
    public TextMeshProUGUI soundButton;

    public TextMeshProUGUI shadowButton;

    public GameObject restoreButton;

    public TextMeshProUGUI orientationButton;
    public TextMeshProUGUI meshingButton;
    public TextMeshProUGUI meshUpdateButton;
    public TextMeshProUGUI waypointButton;
    public TextMeshProUGUI masterWaypointButton;
    public TextMeshProUGUI waypointUpdateButton;


    public GameObject selectSound;

    public GameObject debugButton;
    public GameObject maxOutButton;

    CodelessIAPStoreListener myListener;


    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_IOS
        restoreButton.SetActive(true);
        #endif
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        SetButtons();
        if (Debug.isDebugBuild && debugButton!=null&&maxOutButton!=null)
        {
            debugButton.SetActive(true);
            maxOutButton.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //SetButtons();
    }
    public void ToggleMusic()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.music = !saveScript.settingData.music;
        SetButtons();
    }
    public void ToggleSounds()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.sounds = !saveScript.settingData.sounds;
        SetButtons();
    }
    public void ToggleShadows()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.shadows = !saveScript.settingData.shadows;
        SetButtons();
    }
    public void ToggleOcclusion()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.occlusion = !saveScript.settingData.occlusion;
        SetButtons();
    }
    public void ToggleOrientation()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.orientation ++;
        if (saveScript.settingData.orientation>1)
        {
            saveScript.settingData.orientation = 0;
        }
        SetButtons();
    }
    public void ToggleDebug()
    {
        saveScript.controlScript.testMode = !saveScript.controlScript.testMode;
    }
    public void MaxOut()
    {
        saveScript.gameData.money = 9999999;
        saveScript.gameData.experience = 9999999;
    }
    public void ToggleMeshing()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.highMeshing = !saveScript.settingData.highMeshing;
        SetButtons();
    }
    public void ToggleMeshingUpdates()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        if (saveScript.settingData.meshUpdatePerSecond>=20)
        {
            saveScript.settingData.meshUpdatePerSecond = 0;
        }
        saveScript.settingData.meshUpdatePerSecond += 5;
        SetButtons();
    }
    public void ToggleWaypointDistance()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        if (saveScript.settingData.waypointDistance >= 3f)
        {
            saveScript.settingData.waypointDistance = 0f;
        }
        saveScript.settingData.waypointDistance += 0.5f;
        SetButtons();
    }
    public void ToggleMasterWaypointDistance()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        if (saveScript.settingData.masterWaypointDistance >= 15f)
        {
            saveScript.settingData.masterWaypointDistance = 0f;
        }
        saveScript.settingData.masterWaypointDistance += 5f;
        SetButtons();
    }
    public void ToggleWaypointUpdateFrequency()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        if (saveScript.settingData.waypointUpdateFrequency >= 5f)
        {
            saveScript.settingData.waypointUpdateFrequency = 0f;
        }
        saveScript.settingData.waypointUpdateFrequency += 0.5f;
        SetButtons();
    }
    public void BestPerformance()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.waypointDistance = 3f;
        saveScript.settingData.masterWaypointDistance = 15f;
        saveScript.settingData.highMeshing = false;
        saveScript.settingData.meshUpdatePerSecond = 5;
        saveScript.settingData.waypointUpdateFrequency = 5f;
        saveScript.settingData.shadows = false;
        SetButtons();
    }
    public void BestQuality()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.waypointDistance = 0.5f;
        saveScript.settingData.masterWaypointDistance = 5f;
        saveScript.settingData.highMeshing = true;
        saveScript.settingData.meshUpdatePerSecond = 20;
        saveScript.settingData.waypointUpdateFrequency = 0.5f;
        saveScript.settingData.shadows = true;
        SetButtons();
    }
    public void Default()
    {
        Instantiate(selectSound, Camera.main.transform.position, transform.rotation);
        saveScript.settingData.waypointDistance = 1.5f;
        saveScript.settingData.masterWaypointDistance = 10f;
        saveScript.settingData.highMeshing = true;
        saveScript.settingData.meshUpdatePerSecond = 15;
        saveScript.settingData.waypointUpdateFrequency = 1.0f;
        saveScript.settingData.shadows = true;
        SetButtons();
}
    void SetButtons()
    {
        if (saveScript.settingData.music)
        {
            musicButton.text = "Music:ON";
        }
        else
        {
            musicButton.text = "Music:OFF";
        }
        if (saveScript.settingData.sounds)
        {
            soundButton.text = "Sound:ON";
        }
        else
        {
            soundButton.text = "Sound:OFF";
        }
        if (saveScript.settingData.shadows)
        {
            shadowButton.text = "Shadows:ON";
        }
        else
        {
            shadowButton.text = "Shadows:OFF";
        }
        if (saveScript.settingData.orientation==0)
        {
            orientationButton.text = "Orientation:PORTRAIT";
        }
        else
        {
            orientationButton.text = "Orientation:LANDSCAPE";
        }
        if (meshingButton!=null) {
            if (saveScript.settingData.highMeshing)
            {
                meshingButton.text = "Meshing Detail:HIGH";
            }
            else
            {
                meshingButton.text = "Meshing Detail:LOW";
            }
        }
        if (meshUpdateButton != null)
        {
            meshUpdateButton.text = "Meshing Updates:"+ saveScript.settingData.meshUpdatePerSecond;
        }
        if (waypointButton != null)
        {
            waypointButton.text = "Waypoint Distance:" + saveScript.settingData.waypointDistance;
        }
        if (masterWaypointButton != null)
        {
            masterWaypointButton.text = "Master Distance:" + saveScript.settingData.masterWaypointDistance;
        }
        if (waypointUpdateButton != null)
        {
            waypointUpdateButton.text = "Waypoint Updates:" + saveScript.settingData.waypointUpdateFrequency;
        }
    }
}
