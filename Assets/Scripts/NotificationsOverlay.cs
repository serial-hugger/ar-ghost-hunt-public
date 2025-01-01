using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationsOverlay : MonoBehaviour
{
    public RectTransform popup;
    public Image iconImage;
    public Text popupText;
    public AudioSource popupSound;

    public GameObject errorSound;

    public GameObject errorPopup;
    public TextMeshProUGUI errorText;
    public float timeToDisplayError;

    public float timeToDisplay;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        timeToDisplay -= 1f * Time.deltaTime;
        timeToDisplayError -= 1f * Time.deltaTime;
        if (timeToDisplay>0f)
        {
            popup.pivot = Vector2.Lerp(popup.pivot, new Vector2(.5f, 1.2f), 10f * Time.deltaTime);
            Vector2 tempPos = popup.position;
            tempPos.y = Screen.height;
            popup.position = tempPos;
        }
        else
        {
            popup.pivot = Vector2.Lerp(popup.pivot, new Vector2(.5f, 0f), 10f * Time.deltaTime);
            Vector2 tempPos = popup.position;
            tempPos.y = Screen.height;
            popup.position = tempPos;
        }
        if (timeToDisplayError>0f)
        {
            errorPopup.transform.localScale = Vector3.MoveTowards(errorPopup.transform.localScale, new Vector3(1f, 1f, 1f), 5f * Time.deltaTime);
        }
        else {
            errorPopup.transform.localScale = Vector3.MoveTowards(errorPopup.transform.localScale, new Vector3(0f, 1f, 1f), 5f * Time.deltaTime);
        }
    }
    public void DisplayPopup(int icon, string text)
    {
        timeToDisplay = 5f;
        popupText.text = text;
        popupSound.Play();
    }
    public void DisplayError(string text)
    {
        errorPopup.transform.localScale = new Vector3(0f,1f,1f);
        timeToDisplayError = 5f;
        errorText.text = "!!!   "+text+"   !!!";
        Instantiate(errorSound, Camera.main.transform.position, transform.rotation);
    }
}
