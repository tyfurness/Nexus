using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;

/// <summary>
/// Manager that handles ADS, Analytics and IAP
/// </summary>
/// <remarks>
/// Author Tyler Furness, March 2017
/// </remarks>
public class NexusManager : MonoBehaviour, IStoreListener
{
    #region Variables

    private const string ANALYTICS_LOGGING_COLOR = "cc4";
    private const string ADS_LOGGING_COLOR = "cc5";
    private const string IAP_LOGGING_COLOR = "cc6";
    private const string APPLE_SUB_NAME = "com.unity3d.subscription.new";
    private const string GOOGLEPLAY_SUB_NAME = "com.unity3d.subscription.original";

    public static event Action<ShowResult> AdsComplete;
    public static event Action NoAdsAvailable;

    public static event Action<string> ProductPurchased;
    public static event Action<string> ProductFailedToPurchase;
    public static event Action ProductRestored;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    private Dictionary<string, ProductType> IAPProductInfo;

    private static bool _isDestroying = false;

    #endregion

    /// <summary>
    /// Make sure we have a single instance of this class, if one wasn't created ahead of time then make one and return the instance
    /// </summary>
    private static NexusManager _instance = null;
    public static NexusManager Instance
    {
        get
        {
            _instance = (NexusManager)FindObjectOfType(typeof(NexusManager));
            if (_instance == null && !_isDestroying)
            {
                _instance = (new GameObject("NexusManager")).AddComponent<NexusManager>();
            }
            return _instance;
        }
    }
    
    private void OnDestroy()
    {
        _isDestroying = true;
    }

    private void Start()
    {
        IAPProductInfo = new Dictionary<string, ProductType>();
    }

    #region Analytics

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

    #endregion

    #region Ads

    /// <summary>
    /// Shows an ad if one is available. 
    /// </summary>
    /// <remarks>
    /// If ad is available it will show an add and then call OnAdsComplete with the result (finished, skipped, etc)
    /// If no ads are available, it will call NoAdsAvailable
    /// </remarks>
    public void ShowAd()
    {
        if (Advertisement.IsReady())
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

    #endregion

    #region IAP

    /// <summary>
    /// Determines if we've already initialized the store
    /// </summary>
    /// <returns>True if store controller and extention provider isn't null</returns>
    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
    }

    /// <summary>
    /// Add a single product to the dictionary
    /// </summary>
    /// <param name="productID">ID of the product to add. i.e 100_coins</param>
    /// <param name="productType">Consumable, Subscription, Non-consumable</param>
    public void AddProductToDictionary(string productID, ProductType productType)
    {
        IAPProductInfo.Add(productID, productType);
    }

    /// <summary>
    /// Add a dictionary of products to the IAP dictionary.
    /// </summary>
    /// <param name="productDict">Dictionary of products to add. I.E 100_coins, Consumable</param>
    public void AddProductToDictionary(Dictionary<string, ProductType> productDict)
    {
        IAPProductInfo = productDict;
    }

    /// <summary>
    /// Once all the products have been added to the dictionary, call this function to initialize the store with the products
    /// </summary>
    public void InitializePurchasing()
    {
        if (IAPProductInfo != null && IAPProductInfo.Count > 0)
        {
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (KeyValuePair<string, ProductType> kvp in IAPProductInfo)
            {
                // The stores have different identifiers for subs
                if (kvp.Value == ProductType.Subscription)
                {
                    builder.AddProduct(kvp.Key, kvp.Value, new IDs()
                    {
                        { APPLE_SUB_NAME, AppleAppStore.Name },
                        { GOOGLEPLAY_SUB_NAME, GooglePlay.Name }
                    });
                }
                else
                {
                    builder.AddProduct(kvp.Key, kvp.Value);
                }
            }
            UnityPurchasing.Initialize(this, builder);
        }
    }

    /// <summary>
    /// Internal buy product that will call the store controller to begin the purchase process
    /// </summary>
    public void BuyProductID(string productID)
    {
        if (IsInitialized())
        {
            // Check the store for products with the passed in product
            Product product = storeController.products.WithID(productID);

            if (productID != null && product.availableToPurchase)
            {
                Logger.Log("Purchasing product: " + productID, Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
                storeController.InitiatePurchase(product);
            }
            else
            {
                Logger.Log("Cannot buy product, either not found or not available for purchase", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
            }
        }
        else
        {
            Logger.Log("Buying product failed, store isn't initialized", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
        }
    }

    /// <summary>
    /// Used if the account is put on another device, app gets deleted, etc. Will restore the purchases the user previously had
    /// </summary>
    public void RestorePurchases()
    {
        if (!IsInitialized())
        {
            Logger.Log("Restore purchases failed, store isn't initialized.", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
            return;
        }

        // Apple requires explicit purchase restoration for IAP, displaying a password prompt most the time
        // Most platforms automatically restore automatically
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
           Application.platform == RuntimePlatform.OSXPlayer)
        {
            Logger.Log("Restore purchases has begun.", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);

            IAppleExtensions apple = storeExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                if (ProductRestored != null)
                {
                    ProductRestored();
                }

                Logger.Log("Restore purchases continueing: " + result + ". If no further messages, no purchases available to restore.", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
            });
        }
        else
        {
            Logger.Log("Restore purchases fail. Not supported on this platform, or may have automatically restored depending on platform. Current = " + Application.platform,
                       Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
        }
    }

    /// <summary>
    /// Called when the store is initialized successfully. Sets the store controller and extensions
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Logger.Log("Store initialized.", Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);

        storeController = controller;
        storeExtensionProvider = extensions;
    }

    /// <summary>
    /// Callback for when the initializtion failed
    /// </summary>
    /// <param name="error">Reason the initialization failed</param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Logger.Log("Store failed to initialize: " + error, Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
    }

    /// <summary>
    /// Purchase success callback. Will fire off product purchased action with the id
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        // Compare our id's with the successful purchase one to make sure we award the right thing
        foreach (KeyValuePair<string, ProductType> kvp in IAPProductInfo)
        {
            if (string.Equals(e.purchasedProduct.definition.id, kvp.Key, StringComparison.Ordinal))
            {
                Logger.Log("Purchase success. Product: " + e.purchasedProduct.definition.id, Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
                if (ProductPurchased != null)
                {
                    ProductPurchased(e.purchasedProduct.definition.id);
                }
            }
            else
            {
                Logger.Log("Product does not exist. Product: " + e.purchasedProduct.definition.id, Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
                if (ProductFailedToPurchase != null)
                {
                    ProductFailedToPurchase(e.purchasedProduct.definition.id);
                }
                break;
            }
        }
        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Purchase failed to complete callback
    /// </summary>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason error)
    {
        Logger.Log("Purchase failed: " + error + " and Product: " + product, Logger.LogClasses.IAP.ToString(), IAP_LOGGING_COLOR);
        if (ProductFailedToPurchase != null)
        {
            ProductFailedToPurchase(product.definition.id);
        }
    }

    #endregion

}
