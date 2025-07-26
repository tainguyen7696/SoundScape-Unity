using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

public class IAPManager : MonoBehaviour, IStoreListener
{
    public static IAPManager Instance;

    private static IStoreController storeController;
    private static IExtensionProvider storeExtensionProvider;

    public const string monthlyPremium = "monthly";
    public const string yearlyPremium = "yearly";

    private bool initialized = false;

    public static event Action OnPremiumUnlocked;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Debug_ResetIAP();
        if (storeController == null)
            InitializePurchasing();
    }

    public void InitializePurchasing()
    {
        var module = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
        // Simulate the real store UI in the Editor
        module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif

        var builder = ConfigurationBuilder.Instance(module);
        builder.AddProduct(monthlyPremium, ProductType.Subscription);
        builder.AddProduct(yearlyPremium, ProductType.Subscription);

        UnityPurchasing.Initialize(this, builder);
    }
    /// <summary>
    /// DEBUG: Removes premium and clears IAP state so you can re‑test purchases in the Editor.
    /// </summary>

    [ContextMenu("🧪 Reset IAP (Editor Only)")]
    public void Debug_ResetIAP()
    {
#if UNITY_EDITOR
        // 1) Clear your premium flag
        PlayerPrefs.DeleteKey("IsPremium");
        PlayerPrefs.Save();
        Debug.Log("⚙️ PlayerPrefs ‘IsPremium’ cleared.");

        // 2) Reset our in‑memory state
        initialized = false;
        storeController = null;
        storeExtensionProvider = null;

        // 3) (Re‑)initialize IAP so fake receipts are dropped
        InitializePurchasing();
        Debug.Log("⚙️ IAP re‑initialized.");
#endif
    }

    public void BuyMonthly()
    {
        BuyProductID(monthlyPremium);
    }

    public void BuyYearly()
    {
        BuyProductID(yearlyPremium);
    }

    private void BuyProductID(string productId)
    {
        if (storeController == null)
        {
            Debug.LogWarning("Store not initialized.");
            return;
        }

        Product product = storeController.products.WithID(productId);
        if (product != null && product.availableToPurchase)
        {
            Debug.Log("🛒 Purchasing: " + productId);
            storeController.InitiatePurchase(product);
        }
        else
        {
            Debug.LogWarning("Product not found or unavailable: " + productId);
        }
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("✅ IAP Initialized");
        storeController = controller;
        storeExtensionProvider = extensions;
        initialized = true;

        // Validate subscription in background
        ValidateAllReceipts();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogWarning("❌ IAP Init failed: " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        if (args.purchasedProduct.definition.id == monthlyPremium ||
            args.purchasedProduct.definition.id == yearlyPremium)
        {
            Debug.Log("✅ Subscription purchased: " + args.purchasedProduct.definition.id);
            UnlockPremium();
        }
        else
        {
            Debug.LogWarning("❓ Unknown product: " + args.purchasedProduct.definition.id);
        }

        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogWarning($"❌ Purchase failed: {product.definition.id} | Reason: {reason}");
    }

    // 🔓 Save premium access locally
    private void UnlockPremium()
    {
        PlayerPrefs.SetInt("IsPremium", 1);
        PlayerPrefs.Save();
        OnPremiumUnlocked?.Invoke();
        Debug.Log("🚀 Premium unlocked");
    }

    // 🔒 Revoke premium locally
    private void RevokePremium()
    {
        PlayerPrefs.SetInt("IsPremium", 0);
        PlayerPrefs.Save();
        Debug.Log("🚫 Premium revoked (expired)");
    }

    // 🔍 Fast check for UI gating
    public bool IsPremiumUser()
    {
        return PlayerPrefs.GetInt("IsPremium", 0) == 1;
    }

    // 🔄 Restore (iOS)
    public void RestorePurchases()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            var apple = storeExtensionProvider?.GetExtension<IAppleExtensions>();
            apple?.RestoreTransactions(result =>
            {
                Debug.Log("🔁 Restore complete: " + result);
                if (result) ValidateAllReceipts();
            });
        }
    }

    // ✅ Check all receipts for active subscriptions
    private void ValidateAllReceipts()
    {
        if (!initialized || storeController == null) return;

        foreach (var product in storeController.products.all)
        {
            if (product != null && product.hasReceipt)
            {
                ValidateReceipt(product);
            }
        }
    }

    private void ValidateReceipt(Product product)
    {
        try
        {
            var validator = new CrossPlatformValidator(null, AppleTangle.Data(), Application.identifier);
            var result = validator.Validate(product.receipt);

            foreach (IPurchaseReceipt receipt in result)
            {
#if UNITY_IOS
                if (receipt is AppleInAppPurchaseReceipt apple)
                {
                    Debug.Log("📄 Apple receipt: " + apple.productID);

                    if (apple.subscriptionExpirationDate > DateTime.UtcNow)
                    {
                        UnlockPremium();
                        Debug.Log("✅ iOS subscription active until " + apple.subscriptionExpirationDate);
                    }
                    else
                    {
                        RevokePremium();
                        Debug.Log("❌ iOS subscription expired: " + apple.subscriptionExpirationDate);
                    }
                }
#else
                // For Android – always assume valid for now (until server validation is added)
                UnlockPremium();
#endif
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("❌ Receipt validation error: " + e.Message);
        }
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
