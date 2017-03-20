using System.Collections.Generic;
using UnityEngine.Analytics;

/// <summary>
/// Handles firing off analytic events to unity's analytics
/// </summary>
/// <remarks>
/// Author: Tyler Furness, March 2017
/// </remarks>
public class AnalyticManager
{
    private const string ANALYTICS_LOGGING_COLOR = "cc4";

    /// <summary>
    /// Analytic - Fire an event to track various data through Unity's Ad system and log the result
    /// </summary>
    /// <param name="eventName">The action name of the event. i.e MenuButtonPressed</param>
    /// <param name="data">Any data passed in. i.e - MenuButton, PlayButton</param>
    public void TrackEvent(string eventName, Dictionary<string, object> data)
    {
        // Just in case the data that was passed in was null
        if (data == null || data.Count < 1)
        {
            Logger.Log("Data passed in was null or empty.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
            return;
        }

        // Set the result so we can log the output
        AnalyticsResult result = Analytics.CustomEvent(eventName, data);

        #region Analytic Logging
        switch (result)
        {
            case AnalyticsResult.Ok:
                Logger.Log("Analytic fired off.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.AnalyticsDisabled:
                Logger.Log("Analytics are disabled.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.InvalidData:
                Logger.Log("Invalid data passed in.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.NotInitialized:
                Logger.Log("Analytics not initialized.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.SizeLimitReached:
                Logger.Log("Analytic size limit reached.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.TooManyItems:
                Logger.Log("Too many items to fire off.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.TooManyRequests:
                Logger.Log("Too many requests.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
            case AnalyticsResult.UnsupportedPlatform:
                Logger.Log("Unsupported platform.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;

            default:
                Logger.Log("Unknown error.", Logger.LogClasses.Analytics.ToString(), ANALYTICS_LOGGING_COLOR);
                break;
        }

        #endregion
    }

}
