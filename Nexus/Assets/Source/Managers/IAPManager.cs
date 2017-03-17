using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

/// <summary>
/// Handles purchasing
/// </summary>
/// <remarks>
/// Author: Tyler Furness, March 2017
/// </remarks>
public class IAPManager : IStoreListener
{
    private const string IAP_LOGGING_COLOR = "cc6";
    private const string APPLE_SUB_NAME = "com.unity3d.subscription.new";
    private const string GOOGLEPLAY_SUB_NAME = "com.unity3d.subscription.original";

    public event Action<string> ProductPurchased;
    public event Action<string, string> ProductFailedToPurchase;
    public event Action ProductRestored;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    private Dictionary<string, ProductType> IAPProductInfo;

    /// <summary>
    /// Set up the store with the purchasable products
    /// </summary>
    /// <param name="productDict">Dictionary of </param>
    public void Initialize(Dictionary<string, ProductType> productDict)
    {
        IAPProductInfo = productDict;

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
    /// Determines if we've already initialized the store
    /// </summary>
    /// <returns>True if store controller and extention provider isn't null</returns>
    private bool IsInitialized()
    {
        return storeController != null && storeExtensionProvider != null;
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
                    ProductFailedToPurchase(e.purchasedProduct.definition.id, "");
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
            ProductFailedToPurchase(product.definition.id, error.ToString());
        }
    }

}
