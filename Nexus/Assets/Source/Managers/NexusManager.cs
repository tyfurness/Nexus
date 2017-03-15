using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;

/// <summary>
/// Manager that handles ADS, Analytics and IAP
/// </summary>
/// <remarks>
/// Author Tyler Furness, March 2017
/// </remarks>
public class NexusManager : MonoBehaviour
{

    private const string LOGGING_STRING = "Analytics";
    private const string LOGGING_COLOR = "cc4";

    public static event Action<ShowResult> AdsComplete;
    public static event Action NoAdsAvailable;

    /// <summary>
    /// Make sure we have a single instance of this class, if one wasn't created ahead of time then make one and return the instance
    /// </summary>
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

    /// <summary>
    /// Analytic - Fire an event to track various data through Unity's Ad system and log the result
    /// </summary>
    /// <param name="eventName">The action name of the event. i.e MenuButtonPressed</param>
    /// <param name="data">Any data passed in. i.e - MenuButton, PlayButton</param>
    public void TrackEvent(string eventName, Dictionary<string, object> data)
    {
        // Just in case the data that was passed in was null
        if(data == null || data.Count < 1)
        {
            Logger.Log("Data passed in was or empty.", LOGGING_STRING, LOGGING_COLOR);
            return;
        }

        // Set the result so we can log the output
        AnalyticsResult result = Analytics.CustomEvent(eventName, data);

        #region Analytic Logging
        switch(result)
        {
            case AnalyticsResult.Ok:
                Logger.Log("Analytic fired off.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.AnalyticsDisabled:
                Logger.Log("Analytics are disabled.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.InvalidData:
                Logger.Log("Invalid data passed in.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.NotInitialized:
                Logger.Log("Analytics not initialized.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.SizeLimitReached:
                Logger.Log("Analytic size limit reached.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.TooManyItems:
                Logger.Log("Too many items to fire off.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.TooManyRequests:
                Logger.Log("Too many requests.", LOGGING_STRING, LOGGING_COLOR);
                break;
            case AnalyticsResult.UnsupportedPlatform:
                Logger.Log("Unsupported platform.", LOGGING_STRING, LOGGING_COLOR);
                break;

            default:
                Logger.Log("Unknown error.", LOGGING_STRING, LOGGING_COLOR);
                break;
        }

        #endregion
    }

    /// <summary>
    /// Shows an ad if one is available. 
    /// </summary>
    /// <remarks>
    /// If ad is available it will show an add and then call OnAdsComplete with the result (finished, skipped, etc)
    /// If no ads are available, it will call NoAdsAvailable
    /// </remarks>
    public void ShowAd()
    {
        if(Advertisement.IsReady())
        {
            // Set the callback so we can call on complete
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

    /// <summary>
    /// Ad was shown and completed in some sense
    /// </summary>
    /// <param name="result">Failed, Skipped, Finished</param>
    public void OnAdComplete(ShowResult result)
    {
        if(AdsComplete != null)
        {
            AdsComplete(result);
        }
    }
}
