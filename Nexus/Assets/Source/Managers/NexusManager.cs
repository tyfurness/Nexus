using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

public class NexusManager : MonoBehaviour
{
    public static event Action<ShowResult> AdsComplete;
    public static event Action NoAdsAvailable;

    private static NexusManager _instance = null;
    public static NexusManager Instance
    {
        get
        {
            _instance = (NexusManager)FindObjectOfType(typeof(NexusManager));
            if(_instance == null)
            {
                _instance = (new GameObject("NexusManager")).AddComponent<NexusManager>();
            }
            return _instance;
        }
        
    }

    public void TrackEvent(string eventName, Dictionary<string, object> data)
    {
        if(data == null || data.Count < 1)
        {
            return;
        }
        AnalyticsResult result = Analytics.CustomEvent(eventName, data);

        switch(result)
        {
            case AnalyticsResult.Ok:
                Logger.Log("Analytic fired off.", "Analytics", "cc4");
                break;
            case AnalyticsResult.AnalyticsDisabled:
                Logger.Log("Analytics are disabled.", "Analytics", "cc4");
                break;
            case AnalyticsResult.InvalidData:
                Logger.Log("Invalid data passed in.", "Analytics", "cc4");
                break;
            case AnalyticsResult.NotInitialized:
                Logger.Log("Analytics not initialized.", "Analytics", "cc4");
                break;
            case AnalyticsResult.SizeLimitReached:
                Logger.Log("Analytic size limit reached.", "Analytics", "cc4");
                break;
            case AnalyticsResult.TooManyItems:
                Logger.Log("Too many items to fire off.", "Analytics", "cc4");
                break;
            case AnalyticsResult.TooManyRequests:
                Logger.Log("Too many requests.", "Analytics", "cc4");
                break;
            case AnalyticsResult.UnsupportedPlatform:
                Logger.Log("Unsupported platform.", "Analytics", "cc4");
                break;
        }
    }

    public void ShowAd()
    {
        
        if(Advertisement.IsReady())
        {
            ShowOptions showOptions = new ShowOptions();
            showOptions.resultCallback = OnAdComplete;
            Advertisement.Show(showOptions);
        }
        else
        {
            if (NoAdsAvailable != null)
            {
                NoAdsAvailable();
            }
        }
    }

    public void OnAdComplete(ShowResult result)
    {
        if(AdsComplete != null)
        {
            AdsComplete(result);
        }
    }
}
