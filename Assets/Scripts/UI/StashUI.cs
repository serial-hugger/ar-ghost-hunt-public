using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StashUI : MonoBehaviour
{
    public GameObject deskContent;
    public GameObject stashContent;
    public GameObject evidenceButtonPrefab;

    public SaveManager saveScript;
    public AudioInput inputScript;
    public MainUI uiScript;

    public RectTransform deskContentRect;
    public RectTransform stashContentRect;

    public TextMeshProUGUI deskAmount;
    public TextMeshProUGUI archiveAmount;

    public Color labelColorDefault;
    public Color labelColorAllow;
    public Color labelColorDisallow;
    public Color panelColorDefault;
    public Color panelColorAllow;
    public Color panelColorDisallow;

    public Image deskLabel;
    public Image deskPanel;
    public Image archiveLabel;
    public Image archivePanel;

    public TextMeshProUGUI selectHint;

    // Start is called before the first frame update
    void Awake()
    {
        saveScript = GameObject.Find("GameManager").GetComponent<SaveManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnEnable()
    {
        if (uiScript.controlScript.selectQuestItems.Count==0)
        {
            uiScript.stashScript.selectHint.text = "";
        }
        foreach (Transform child in deskContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        foreach (Transform child in stashContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0;i<saveScript.gameData.deskEvidence.Count;i++)
        {
            StashEvidenceButton buttonScript = Instantiate(evidenceButtonPrefab,deskContent.transform).GetComponent<StashEvidenceButton>();
            buttonScript.myButtonBlock.myAction = "stashdesk" + i.ToString();
            buttonScript.myEvidence = saveScript.gameData.deskEvidence[i];
            buttonScript.inputScript = inputScript;
            buttonScript.uiScript = uiScript;
            //disables button if doesnt satisfy a quest
            if (uiScript.controlScript.selectQuestItems.Count>0) {
                bool satisfies = false;
                for (int q = 0; q < uiScript.controlScript.selectQuestItems.Count; q++)
                {
                    if (buttonScript.myEvidence.SatisfiesQuest(uiScript.controlScript.selectQuestItems[q]))
                    {
                        satisfies = true;
                    }
                }
                if (satisfies && uiScript.controlScript.deskSelectAllowed && (saveScript.gameData.deskEvidence[i].messageUploadingTo == null || saveScript.GetMessageFromId(saveScript.gameData.deskEvidence[i].messageUploadingTo).name == "") && !uiScript.controlScript.selectedEvidences.Contains(buttonScript.myEvidence.id))
                {
                    buttonScript.selectable = true;
                }
                else
                {
                    buttonScript.selectable = false;
                }
            }
            else
            {
                buttonScript.selectable = true;
            }
        }
        for (int i = 0; i < saveScript.gameData.archiveEvidence.Count; i++)
        {
            StashEvidenceButton buttonScript = Instantiate(evidenceButtonPrefab, stashContent.transform).GetComponent<StashEvidenceButton>();
            buttonScript.myButtonBlock.myAction = "stasharchive" + i.ToString();
            buttonScript.myEvidence = saveScript.gameData.archiveEvidence[i];
            buttonScript.inputScript = inputScript;
            buttonScript.uiScript = uiScript;
            //disables button if doesnt satisfy a quest
            if (uiScript.controlScript.selectQuestItems.Count > 0)
            {
                bool satisfies = false;
                for (int q = 0; q < uiScript.controlScript.selectQuestItems.Count; q++)
                {
                    if (buttonScript.myEvidence.SatisfiesQuest(uiScript.controlScript.selectQuestItems[q]))
                    {
                        satisfies = true;
                    }
                }
                if (satisfies && uiScript.controlScript.archiveSelectAllowed && (saveScript.gameData.archiveEvidence[i].messageUploadingTo == null|| saveScript.GetMessageFromId(saveScript.gameData.archiveEvidence[i].messageUploadingTo).name == "")&& !uiScript.controlScript.selectedEvidences.Contains(buttonScript.myEvidence.id))
                {
                    buttonScript.selectable = true;
                }
                else
                {
                    buttonScript.selectable = false;
                }
            }
            else
            {
                buttonScript.selectable = true;
            }
        }
        deskContentRect.sizeDelta = new Vector2(700f, 141f + (141f*(Mathf.FloorToInt((saveScript.gameData.deskEvidence.Count-1) / 5f))));
        stashContentRect.sizeDelta = new Vector2(700f, 179f + (179f * (Mathf.FloorToInt((saveScript.gameData.archiveEvidence.Count - 1) / 4f))));
        deskAmount.text = saveScript.gameData.deskEvidence.Count + "/" + saveScript.gameData.deskSlots;
        archiveAmount.text = saveScript.gameData.archiveEvidence.Count + "/" + saveScript.gameData.archiveSlots;
        if (uiScript.controlScript.selectQuestItems.Count > 0)
        {
            if (uiScript.controlScript.deskSelectAllowed)
            {
                deskLabel.color = labelColorAllow;
                deskPanel.color = panelColorAllow;
            }
            else
            {
                deskLabel.color = labelColorDisallow;
                deskPanel.color = panelColorDisallow;
            }
            if (uiScript.controlScript.archiveSelectAllowed)
            {
                archiveLabel.color = labelColorAllow;
                archivePanel.color = panelColorAllow;
            }
            else
            {
                archiveLabel.color = labelColorDisallow;
                archivePanel.color = panelColorDisallow;
            }
        }
        else
        {
            deskLabel.color = labelColorDefault;
            deskPanel.color = panelColorDefault;
            archiveLabel.color = labelColorDefault;
            archivePanel.color = panelColorDefault;
        }
    }
}
