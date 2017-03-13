using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Button button;
    // Use this for initialization
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
        NexusManager.AdsComplete += OnAdsComplete;
        NexusManager.NoAdsAvailable += OnNoAdsAvailable;
    }

    private void OnButtonClicked()
    {
        NexusManager.Instance.ShowAd();
    }

    private void OnAdsComplete(ShowResult result)
    {
        Logger.Log("Result of ad was: " + result, "ADs", "cc5");
    }

    private void OnNoAdsAvailable()
    {
        Logger.Log("No Ads Available", "ADs", "cc5");
    }
}
