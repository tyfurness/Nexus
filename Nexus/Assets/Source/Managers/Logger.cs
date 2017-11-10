using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Logger
{
    public enum LogClasses
    {
        Ads,
        Analytics,
        IAP
    }
    public static void Log(string message, string header, string color)
    {
        string formatted = string.Format("<color=#{0}>[{1}]</color> {2}", color, header, message);
        //"<color=#" + color + ">[" + header + "]</color> " + message
        Debug.Log(formatted);
    }

    public static void LogError(string header, string message)
    {
        Debug.LogError(string.Format("{0}: {1}", header, message));
    }
}
