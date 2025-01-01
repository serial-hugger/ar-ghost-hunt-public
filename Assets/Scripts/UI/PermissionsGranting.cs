using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#elif UNITY_IOS
using UnityEngine.iOS;
using Unity.Notifications.iOS;
#endif

public class PermissionsGranting : MonoBehaviour
{
    public GameObject microphonePermission;
    public bool microphoneDecline = false;
    public GameObject locationPermission;
    public bool locationDecline = false;
    public GameObject notificationPermission;
    public bool notificationDecline = false;
    public GameObject cameraPermission;

    public GameObject boot;

    public int stage;
    // Start is called before the first frame update
    void Start()
    {
        if (IsCameraPermissionGranted())
        {
            stage = 4;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stage == 0)
        {
            boot.SetActive(false);
            microphonePermission.SetActive(true);
            locationPermission.SetActive(false);
            notificationPermission.SetActive(false);
            cameraPermission.SetActive(false);
        }
        if (stage == 1)
        {
            boot.SetActive(false);
            microphonePermission.SetActive(false);
            locationPermission.SetActive(true);
            notificationPermission.SetActive(false);
            cameraPermission.SetActive(false);
        }
        if (stage == 2)
        {
            boot.SetActive(false);
            microphonePermission.SetActive(false);
            locationPermission.SetActive(false);
            notificationPermission.SetActive(true);
            cameraPermission.SetActive(false) ;
#if UNITY_ANDROID
            if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
            {
                stage++;
            }
#elif UNITY_IOS
            stage++;
#endif

        }
        if (stage == 3)
        {
            boot.SetActive(false);
            microphonePermission.SetActive(false);
            locationPermission.SetActive(false);
            notificationPermission.SetActive(false);
            cameraPermission.SetActive(true);
        }
        if (stage>=4)
        {
            if (Input.location.status == LocationServiceStatus.Stopped) {
                Input.location.Start();
            }
            boot.SetActive(true);
            microphonePermission.SetActive(false);
            locationPermission.SetActive(false);
            cameraPermission.SetActive(false);
        }
    }
    public void DeclineMicrophone()
    {
        microphoneDecline = true;
        stage++;
    }
    public void DeclineLocation()
    {
        locationDecline = true;
        stage++;
    }

    public void RequestMicrophonePermission()
    {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Microphone);
#elif UNITY_IOS
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
#endif
        stage++;
    }
    public void RequestLocationPermission()
    {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.FineLocation);
#elif UNITY_IOS
            Input.location.Start();
#endif
        stage++;
    }
    public void RequestNotificationPermission()
    {
#if PLATFORM_ANDROID
        Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
#elif UNITY_IOS
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            new AuthorizationRequest(authorizationOption, false);
#endif
        stage++;
    }
    //CAMERA
    public bool IsCameraPermissionGranted()
    {
#if UNITY_ANDROID
        return Permission.HasUserAuthorizedPermission(Permission.Camera);
#elif UNITY_IOS
        return Application.HasUserAuthorization(UserAuthorization.WebCam);
#endif
        return true;
    }
    public void RequestCameraPermission()
    {
#if PLATFORM_ANDROID
            Permission.RequestUserPermission(Permission.Camera);
#elif UNITY_IOS
            Application.RequestUserAuthorization(UserAuthorization.WebCam);
            
#endif
        stage++;
    }
    public void ResetStage()
    {
        stage = 0;
    }
}
