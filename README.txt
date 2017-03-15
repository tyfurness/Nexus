NexusManager Notes:
- Make sure you have Unity service's enabled.
-- Make sure Ads are enabled (for testing in unity make sure you have test mode on)
-- Make sure analytics is on (if you're using it)
-- Make sure IAP is enabled and imported (if you're using it) (on import make sure that the other app package gets imported as well)

IAP Specifically:
Make sure to add your products to NexusManager's IAPProductInfo by calling AddProductToDictionary and passing an individual productID with the type, or pass a dictionary of all of them.
Once that is done, call InitializePurchasing() to set the store up with the products you have.
From there on out you can call BuyProduct and pass in the ID of the product to initiate a purchase.
-Successful purchases will trigger the ProductPurchased event and pass the product id back to confirm which one was purchased
-Failures will trigger the ProductFailedToPurchase event and pass the product id back to confirm which one failed
You are also able to restore purchases by calling RestorePurchases