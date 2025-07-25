using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using System;

public class IAPManager : MonoBehaviour, IStoreListener
{
    private static IStoreController StoreController;
    private static IExtensionProvider StoreExtensionProvider;

    // TODO: Replace with your actual product IDs
    public const string PRODUCT_PREMIUM = "com.tsglobal.soundscape.premium";

    void Awake()
    {
        // If not already initialized, initialize Unity IAP
        if (StoreController == null)
            InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        // Add non-consumable products
        builder.AddProduct(PRODUCT_PREMIUM, ProductType.NonConsumable);

        UnityPurchasing.Initialize(this, builder);
    }

    #region IStoreListener

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("IAP Initialized");
        StoreController = controller;
        StoreExtensionProvider = extensions;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError($"IAP Init Failed: {error}");
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        Debug.Log($"Purchase OK: {args.purchasedProduct.definition.id}");

        switch (args.purchasedProduct.definition.id)
        {
            case PRODUCT_PREMIUM:
                GrantPremiumFeatures();
                break;
            default:
                Debug.LogWarning($"Unknown product: {args.purchasedProduct.definition.id}");
                break;
        }

        // Return Complete if success; Pending if you use server-side receipts
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError($"Purchase FAILED: {product.definition.id}, Reason: {reason}");
    }

    #endregion

    #region Public Purchase Methods

    public void BuyPremium()
    {
        BuyProductID(PRODUCT_PREMIUM);
    }

    private void BuyProductID(string productId)
    {
        if (StoreController == null)
        {
            Debug.LogError("IAP not initialized.");
            return;
        }

        Product product = StoreController.products.WithID(productId);

        if (product != null && product.availableToPurchase)
        {
            Debug.Log($"Purchasing {productId}...");
            StoreController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogError($"Product not found or not available: {productId}");
        }
    }

    /// <summary>
    /// For iOS — restores previous purchases (non-consumables).
    /// </summary>
    public void RestorePurchases()
    {
        if (StoreController == null || StoreExtensionProvider == null)
        {
            Debug.LogError("IAP not initialized.");
            return;
        }

#if UNITY_IOS
        var apple = StoreExtensionProvider.GetExtension<IAppleExtensions>();
        apple.RestoreTransactions(result =>
        {
            Debug.Log($"RestorePurchases continuing: {result}. If no further messages, no purchases to restore.");
        });
#else
        Debug.Log("RestorePurchases is only supported on iOS.");
#endif
    }

    #endregion

    #region Grant Content

    private void GrantPremiumFeatures()
    {
        Debug.Log("Granting premium features!");
        // e.g. PlayerPrefs.SetInt("isPremium", 1);
    }

    private void RemoveAds()
    {
        Debug.Log("Removing ads!");
        // e.g. PlayerPrefs.SetInt("noAds", 1);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }

    #endregion
}
