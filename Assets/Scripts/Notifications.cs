using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using UnityEngine;

public class Notifications : MonoBehaviour
{
    public float timeTillWarning = 0f;
    public SaveManager saveScript;
    public Message analysisMessage;
    // Start is called before the first frame update
    void Start()
    {
        GetAnalysisMessage();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void GetAnalysisMessage()
    {
        for (int i = 0; i < saveScript.gameData.messages.Count; i++)
        {
            if (saveScript.gameData.messages[i].name == "Serial")
            {
                analysisMessage = saveScript.gameData.messages[i];
            }
        }
    }
    public void DeleteNotifications()
    {
#if UNITY_ANDROID
        AndroidNotificationCenter.CancelAllScheduledNotifications();
#elif UNITY_IOS
        iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
    }
    public void ScheduleCalendarNotification(DateTime time,string icon,string title, string body, string subtitle,string category)
    {
#if UNITY_ANDROID
        var channel = new AndroidNotificationChannel()
        {
            Id = category,
            Name = category,
            Importance = Importance.Default,
            Description = category,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(channel);
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = body;
        notification.FireTime = time;
        notification.SmallIcon = icon;
        //notification.LargeIcon = "icon_1";


        AndroidNotificationCenter.SendNotification(notification, category);
#elif UNITY_IOS
        var calendarTrigger = new iOSNotificationCalendarTrigger()
        {
            Year = time.Year,
            Month = time.Month,
            Day = time.Day,
            Hour = time.Hour,
            Minute = time.Minute,
            Second = time.Second,
            Repeats = false
        };
        var notification = new iOSNotification()
        {
            // You can specify a custom identifier which can be used to manage the notification later.
            // If you don't provide one, a unique string will be generated automatically.
            Identifier = category,
            Title = "Title",
            Body = "Scheduled at: " + DateTime.Now.ToShortDateString() + " triggered in 5 seconds",
            Subtitle = "This is a subtitle, something, something important...",
            ShowInForeground = false,
            //ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
            CategoryIdentifier = category,
            ThreadIdentifier = "thread1",
            Trigger = calendarTrigger,
        };
        iOSNotificationCenter.ScheduleNotification(notification);
#endif
    }
}
