using System;
using UnityEngine.Advertisements;

/// <summary>
/// Handles showing and controlling ads
/// </summary>
/// <remarks>
/// Author: Tyler Furness, March 2017
/// </remarks>
public class ADManager
{
    private const string ADS_LOGGING_COLOR = "cc5";

    public event Action<ShowResult> AdsComplete;
    public event Action NoAdsAvailable;

    /// <summary>
    /// Shows an ad if one is available. 
    /// </summary>
    /// <remarks>
    /// If ad is available it will show an add and then call OnAdsComplete with the result (finished, skipped, etc)
    /// If no ads are available, it will call NoAdsAvailable
    /// </remarks>
    public void ShowAd()
    {
        if (IsAdAvailable())
        {
            // Set the callback so we can call on complete
            ShowOptions showOptions = new ShowOptions();
            showOptions.resultCallback = OnAdComplete;
            Advertisement.Show(showOptions);
        }
        else
        {
            Logger.Log("No ads available.", Logger.LogClasses.Ads.ToString(), ADS_LOGGING_COLOR);
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
        Logger.Log("Ad result is: " + result, Logger.LogClasses.Ads.ToString(), ADS_LOGGING_COLOR);
        if (AdsComplete != null)
        {
            AdsComplete(result);
        }
    }

    /// <summary>
    /// Determines if an ad is available or not.
    /// </summary>
    public bool IsAdAvailable()
    {
        return Advertisement.IsReady();
    }
}
