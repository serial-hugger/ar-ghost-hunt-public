using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    bool claimed = false;

    public GameObject claimButton;
    public GameObject claimDoubleButton;

    public GameObject comeBack;
    public GameObject closeButton;

    Ads adScript;
    SaveManager saveScript;

    public Image reward1;
    public Image reward2;
    public Image reward3;
    public Image reward4;
    public Image reward5;

    int prevRewardedAds = 0;

    float timer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
        adScript = GameObject.Find("GameManager").GetComponent<Ads>();
        prevRewardedAds = adScript.rewardedAds;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= 1f * Time.deltaTime;
        if (adScript.rewardedAds > prevRewardedAds)
        {
            saveScript.gameData.prevDailyRewardClaimedTime = TimeManager.GetTime();
            claimed = true;
            prevRewardedAds = adScript.rewardedAds;
            saveScript.gameData.nextDailyReward++;
            if (saveScript.gameData.nextDailyReward > 4)
            {
                saveScript.gameData.nextDailyReward = 0;
            }
            saveScript.WriteFile();
        }
        if (!claimed)
        {
            comeBack.SetActive(false) ;
            closeButton.SetActive(false);
        }
        else
        {
            comeBack.SetActive(true);
            closeButton.SetActive(true);
        }
        if (timer<=0f && !claimed)
        {
            claimButton.SetActive(true);
            claimDoubleButton.SetActive(true);
        }
        else
        {
            claimButton.SetActive(false);
            claimDoubleButton.SetActive(false);
        }
        int nextReward = saveScript.gameData.nextDailyReward;
        if (nextReward == 0)
        {
            reward1.color = Color.yellow;
            reward1.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        if (nextReward > 0)
        {
            reward1.color = Color.gray;
            reward1.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (nextReward < 1)
        {
            reward2.color = Color.white;
            reward2.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (nextReward == 1)
        {
            reward2.color = Color.yellow;
            reward2.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        if (nextReward > 1)
        {
            reward2.color = Color.gray;
            reward2.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (nextReward < 2)
        {
            reward3.color = Color.white;
            reward3.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (nextReward == 2)
        {
            reward3.color = Color.yellow;
            reward3.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        if (nextReward > 2)
        {
            reward3.color = Color.gray;
            reward3.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (nextReward < 3)
        {
            reward4.color = Color.white;
            reward4.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (nextReward == 3)
        {
            reward4.color = Color.yellow;
            reward4.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
        }
        if (nextReward > 3)
        {
            reward4.color = Color.gray;
            reward4.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        if (nextReward < 4)
        {
            reward5.color = Color.white;
            reward5.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        if (nextReward == 4)
        {
            reward5.color = Color.yellow;
            reward5.transform.localScale = new Vector3(1.1f,1.1f,1.1f);
        }
        if (nextReward > 4)
        {
            reward5.color = Color.gray;
            reward5.transform.localScale = new Vector3(1f, 1f, 1f);
        }

    }
    public void OnEnable()
    {
        claimed = false;
        timer = 0f;
    }
    public void ClaimReward()
    {
        if (timer<=0f && !claimed) {
            saveScript.gameData.prevDailyRewardClaimedTime = TimeManager.GetTime();
            timer = 5f;
            claimed = true;
            if (saveScript.gameData.nextDailyReward == 4)
            {
                saveScript.gameData.skipTickets++;
                saveScript.gameData.nextDailyReward++;
            }
            if (saveScript.gameData.nextDailyReward == 3)
            {
                saveScript.gameData.money += 50;
                saveScript.gameData.nextDailyReward++;
            }
            if (saveScript.gameData.nextDailyReward == 2)
            {
                saveScript.gameData.skipTime += TimeSpan.TicksPerMinute * 60;
                saveScript.gameData.nextDailyReward++;
            }
            if (saveScript.gameData.nextDailyReward == 1)
            {
                saveScript.gameData.skipTime += TimeSpan.TicksPerMinute * 30;
                saveScript.gameData.nextDailyReward++;
            }
            if (saveScript.gameData.nextDailyReward == 0)
            {
                saveScript.gameData.experience += 50;
                saveScript.gameData.nextDailyReward++;
            }
            if (saveScript.gameData.nextDailyReward>4)
            {
                saveScript.gameData.nextDailyReward = 0;
            }
            saveScript.WriteFile();
        }
    }
    public void ClaimDoubleReward()
    {
        if (timer <= 0f && !claimed)
        {
            timer = 5f;
            if (saveScript.gameData.nextDailyReward == 4)
            {
                adScript.ShowRewardedAd("skiptickets2");
            }
            if (saveScript.gameData.nextDailyReward == 3)
            {
                adScript.ShowRewardedAd("money100");
            }
            if (saveScript.gameData.nextDailyReward == 2)
            {
                adScript.ShowRewardedAd("time120");
            }
            if (saveScript.gameData.nextDailyReward == 1)
            {
                adScript.ShowRewardedAd("time60");
            }
            if (saveScript.gameData.nextDailyReward == 0)
            {
                adScript.ShowRewardedAd("experience100");
            }
        }
    }
}
