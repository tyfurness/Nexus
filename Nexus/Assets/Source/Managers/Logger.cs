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
        Debug.Log("<color=#" + color + ">[" + header + "]</color> " + message);
    }
}
