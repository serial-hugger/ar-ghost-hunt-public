using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static long ntpTimeStart;
    public static long ticksPassed;
    public static long prevTime;

    public static bool validTime = false;
    public bool prevValid = false;

    public SaveManager saveScript;

    public void Start()
    {
    }
    public void Update()
    {
        if (validTime!=prevValid)
        {
            if (validTime == false)
            {
                saveScript.controlScript.mainUI.SetCurrentMenu("default");
            }
            prevValid = validTime;
        }
        if (validTime)
        {
            saveScript.gameData.lastValidTime = ntpTimeStart;
            if (System.DateTime.UtcNow.Ticks> (prevTime + System.TimeSpan.TicksPerSecond*3))
            {
                SetTime();
            }
            else
            {
                ticksPassed += System.DateTime.UtcNow.Ticks - prevTime;
                prevTime = System.DateTime.UtcNow.Ticks;
            }
        }
        else
        {
        }
    }
    public static void SetTime()
    {
        try
        {
            byte[] ntpData = new byte[48];

            //LeapIndicator = 0 (no warning), VersionNum = 3 (IPv4 only), Mode = 3 (Client Mode)
            ntpData[0] = 0x1B;

            IPAddress[] addresses = Dns.GetHostEntry("pool.ntp.org").AddressList;
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(new IPEndPoint(addresses[0], 123));
            socket.ReceiveTimeout = 1000;

            socket.Send(ntpData);
            socket.Receive(ntpData);
            socket.Close();

            ulong intc = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | (ulong)ntpData[43];
            ulong frac = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | (ulong)ntpData[47];

            ntpTimeStart = (long)((double)((intc * 1000) + ((frac * 1000) / 0x100000000L))) * 10000;
            //Debug.Log(ntpTime);
            prevTime = System.DateTime.UtcNow.Ticks;
            validTime = true;
        }
        catch (Exception exception)
        {
            Debug.Log("Could not get NTP time");
            Debug.Log(exception);
            validTime = false;
            if (Tutorial.inTutorial)
            {
                validTime = true;
            }
        }
    }
    public static long GetTime()
    {
        if (Tutorial.inTutorial) {
            return System.DateTime.UtcNow.Ticks;
        }
        else
        {
            return ntpTimeStart + ticksPassed;
        }
    }
    public static string GetTimeStringFromTicks(long ticks, bool includeNones)
    {
        int days = Mathf.FloorToInt(ticks / System.TimeSpan.TicksPerDay);
        int hours = Mathf.FloorToInt((ticks % System.TimeSpan.TicksPerDay) / System.TimeSpan.TicksPerHour);
        int minutes = Mathf.FloorToInt((ticks % System.TimeSpan.TicksPerHour) / System.TimeSpan.TicksPerMinute);
        int seconds = Mathf.FloorToInt((ticks % System.TimeSpan.TicksPerMinute) / System.TimeSpan.TicksPerSecond);
        string newText = "";
        if (days > 0)
        {
            newText += days.ToString() + "d ";
        }
        if (hours > 0 || includeNones)
        {
            newText += hours.ToString() + "h ";
        }
        if (minutes > 0 || includeNones)
        {
            newText += minutes.ToString() + "m ";
        }
        if (seconds > 0 || includeNones)
        {
            newText += seconds.ToString() + "s ";
        }
        return newText;
    }

    //public static double LocalTime()
    //{
    //    return DateTime.UtcNow.Subtract(new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    //}
}
