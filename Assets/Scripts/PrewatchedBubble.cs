using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PrewatchedBubble : MonoBehaviour
{
    public SaveManager saveScript;
    public TextMeshProUGUI myText;
    public Text myLegText;

    public bool alwaysShow;

    // Start is called before the first frame update
    void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }
    void OnEnable()
    {
        if (saveScript.gameData.premium && !alwaysShow)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (saveScript.gameData.prewatchedAds>0) {
            myLegText.text = "Ad will skip, you have " + saveScript.gameData.prewatchedAds + " prewatches left";
        }
        else
        {
            myLegText.text = "Ad will show, try to keep device in the exact same position as the ad plays, you can prewatch ads on the main menu";
        }
    }
}
