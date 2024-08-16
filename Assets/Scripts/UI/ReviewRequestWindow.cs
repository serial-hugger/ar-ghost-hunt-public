using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewRequestWindow : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SendEmail()
    {
        Application.OpenURL("mailto:" + "pseudosparkgames@gmail.com");
    }
    public void OpenDiscord()
    {
        Application.OpenURL("https://discord.gg/kvPNmfw6B4");
    }
    public void RequestReview()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.PseudoSparkGames.ARGhostHunt");
#elif UNITY_IOS
        Application.OpenURL("https://apps.apple.com/us/app/ar-ghost-hunt/id6467179325");
#endif
    }
}
