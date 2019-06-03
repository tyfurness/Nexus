using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
//using UnityEngine.Purchasing;
using UnityEngine.UI;
using NexusManager;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Button button = null;
    [SerializeField]
    private Text textField = null;

    // Use this for initialization
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
        Nexus.Instance.ADManager.AdsComplete += OnAdsComplete;
        Nexus.Instance.ADManager.NoAdsAvailable += OnNoAdsAvailable;
        // Dictionary<string, ProductType> productDict = new Dictionary<string, ProductType>();
        //productDict.Add("100_coins", UnityEngine.Purchasing.ProductType.Consumable);
        // Nexus.Instance.IAPManager.Initialize(productDict);
        //  Nexus.Instance.IAPManager.ProductPurchased += OnProductPurchased;
        //  Nexus.Instance.IAPManager.ProductFailedToPurchase += OnFailedToPurchase;
    }

    private void OnProductPurchased(string productID)
    {
        textField.text = productID + " was successfully purchased.";
    }

    private void OnFailedToPurchase(string productID, string error)
    {
        if(!string.IsNullOrEmpty(error))
        {
            textField.text = productID + " failed to purchase: " + error;
        }
        else
        {
            textField.text = productID + " failed to purchase, product doesn't exist.";
        }
    }

    private void OnButtonClicked()
    {
        Nexus.Instance.ADManager.ShowAd();
       // Nexus.Instance.IAPManager.BuyProductID("100_coins");
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
