using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace RealbizGames.Shopping
{

    public class UnityShoppingProvider : IShoppingProvider, IStoreListener
    {

        public const string TAG = "UnityShoppingProvider";

        // --------------------------------------------------------------------------
        // --------------------------------------------------------------------------
        // IShoppingProvider
        // --------------------------------------------------------------------------

        private ShoppingProviderInitState _InitState = ShoppingProviderInitState.None;
        public ShoppingProviderInitState InitState => _InitState;

        private Dictionary<string, ShoppingItemDTO> _itemDictionary = new Dictionary<string, ShoppingItemDTO>();

        private void addItems(List<ShoppingItemDTO> items)
        {
            if (_itemDictionary == null)
            {
                _itemDictionary = new Dictionary<string, ShoppingItemDTO>();
            }
            foreach (ShoppingItemDTO item in items)
            {
                _itemDictionary[item.ItemId] = item;
            }
        }

        public ShoppingItemResponseDTO Buy(ShoppingItemDTO item)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(item.ItemId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                    return new ShoppingItemResponseDTO(BuyItemStatus.Processing);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    return new ShoppingItemResponseDTO(BuyItemStatus.Fail);
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
                return new ShoppingItemResponseDTO(BuyItemStatus.Fail);
            }
        }

        public bool DidIBuy(string productId)
        {
            Product product = m_StoreController.products.WithID(productId);
            return product != null && product.hasReceipt;
        }

        public ShoppingItemLocalizedMetadataDTO GetLocalizedMetadata(string productId)
        {
            Product product = m_StoreController.products.WithID(productId);
            if (product != null) {
                ShoppingItemLocalizedMetadataDTO dto = new ShoppingItemLocalizedMetadataDTO();

                dto.localizedTitle = product.metadata.localizedTitle;
                dto.localizedDescription = product.metadata.localizedDescription;
                dto.localizedPrice = product.metadata.localizedPrice;
                dto.localizedPriceString = product.metadata.localizedPriceString;

                return dto;
            } else {
                return null;
            }
        }

        public void InitializePurchasing(List<ShoppingItemDTO> items)
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                Debug.Log("InitializePurchasing has already!");
                return;
            }
            else
            {
                addItems(items);

                // Create a builder, first passing in a suite of Unity provided stores.
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

                // Add a product to sell / restore by way of its identifier, associating the general identifier
                // with its store-specific identifiers.
                /*builder.AddProduct(kProductIDConsumable, ProductType.Consumable);*/

                // Continue adding the non-consumable product.
                /*builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);*/

                // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
                // if the Product ID was configured differently between Apple and Google stores. Also note that
                // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
                // must only be referenced here. 

                foreach (ShoppingItemDTO item in items)
                {
                    string itemId = item.ItemId;

                    if (!string.IsNullOrEmpty(itemId))
                    {
                        if (item.Type == ShoppingItemType.Consumable)
                        {
                            builder.AddProduct(itemId, ProductType.Consumable);
                        }
                        else if (item.Type == ShoppingItemType.Non_Consumable)
                        {
                            builder.AddProduct(itemId, ProductType.NonConsumable);
                        }
                        else if (item.Type == ShoppingItemType.Subscription)
                        {
                            builder.AddProduct(itemId, ProductType.Subscription);
                        }
                    }

                }
                _InitState = ShoppingProviderInitState.Processing;
                // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
                // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
                UnityPurchasing.Initialize(this, builder);
            }
        }

        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) =>
                {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        private ProductType convert(ShoppingItemType type) {
            if (type == ShoppingItemType.Non_Consumable) {
                return ProductType.NonConsumable;
            } else if (type == ShoppingItemType.Consumable) {
                return ProductType.Consumable;
            } else if (type == ShoppingItemType.Subscription) {
                return ProductType.Subscription;
            } else {
                throw new System.Exception("Unity do not have this type: " + type.ToString());
            }
        }

        public void NotifyMyOwnItemsBy(ShoppingItemType type, string actionIdentifer)
        {
            ProductType productType = convert(type);

            foreach(Product product in m_StoreController.products.all) {
                if (product.definition.type == productType) {
                    if (didIBuyProduct(product)) {
                        ShoppingItemDTO dto = _itemDictionary[product.definition.id];
                        ShoppingSuccessDTO successDTO = new ShoppingSuccessDTO(item: dto, actionIdentifer: actionIdentifer);
                        ShoppingNotificationCenter.DefaultInstance.NotifySuccess(successDTO);
                    }
                }
            }
        }

        private bool didIBuyProduct(Product p) {
            return p.hasReceipt;
        }

        // --------------------------------------------------------------------------
        // --------------------------------------------------------------------------
        // IStoreListener
        // --------------------------------------------------------------------------

        // The Unity Purchasing system.
        private IStoreController m_StoreController;
        // // The store-specific Purchasing subsystems.
        private IExtensionProvider m_StoreExtensionProvider;
        // private IAppleExtensions m_AppleExtensions;

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            if (Config.DefaultInstance.enableLog) {
                Debug.LogFormat("{0} - OnInitialized", TAG);
            }
            // Purchasing has succeeded initializing. Collect our Purchasing references.

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;
            // m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();

            _InitState = ShoppingProviderInitState.Success;

            if (Config.DefaultInstance.enableLog) {
                PrintDebug();
            }

            ShoppingNotificationCenter.DefaultInstance.NotifyInitializeSuccess();
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            if (Config.DefaultInstance.enableLog) {
                Debug.LogFormat("{0} - OnInitializeFailed", TAG);
            }
            _InitState = ShoppingProviderInitState.Failed;
            ShoppingNotificationCenter.DefaultInstance.NotifyInitializeFailed();
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            if (Config.DefaultInstance.enableLog) {
                Debug.LogFormat("{0} - OnPurchaseFailed {1}", TAG, product.definition.id);
            }
            ShoppingFailDTO dto = new ShoppingFailDTO();
            dto.Item = _itemDictionary[product.definition.id];
            dto.FailType = ShoppingFailType.Unknown;
            dto.FailCode = failureReason.ToString();

            ShoppingNotificationCenter.DefaultInstance.NotifyFail(dto);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            if (Config.DefaultInstance.enableLog) {
                Debug.Log(string.Format("ProcessPurchase: Begin. Product: '{0}'", purchaseEvent.purchasedProduct.definition.id));
            }

            CrossPlatformValidator validator = Config.DefaultInstance.validator;//  new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(purchaseEvent.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                //Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        // This is Google's Order ID.
                        // Note that it is null when testing in the sandbox
                        // because Google's sandbox does not provide Order IDs.
                        ShoppingItemDTO dto = _itemDictionary[productReceipt.productID];
                        ShoppingSuccessDTO successDTO = new ShoppingSuccessDTO(item: dto, actionIdentifer: null);
                        ShoppingNotificationCenter.DefaultInstance.NotifySuccess(successDTO);
                    }
                    else
                    {
                        AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                        if (null != apple)
                        {
                            ShoppingItemDTO dto = _itemDictionary[productReceipt.productID];
                            ShoppingSuccessDTO successDTO = new ShoppingSuccessDTO(item: dto, actionIdentifer: null);
                            ShoppingNotificationCenter.DefaultInstance.NotifySuccess(successDTO);
                        }
                    }
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed.

            return Config.DefaultInstance.enableAutoCompleteConsumableProduct ? PurchaseProcessingResult.Complete : PurchaseProcessingResult.Pending;
        }

        // --------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------
        // --------------------------------------------------------------------------------
        public void PrintDebug()
        {
            List<Product> all = new List<Product>(m_StoreController.products.all);

            string str = ToStringForProducts(all);
            Debug.LogFormat("{0} - Debug {1}\n{2} ", TAG, all.Count, str);
        }

        private string ToStringForProducts(List<Product> products) {
            string str = string.Empty;
            foreach(Product p in products) {
                str += ToStringForProduct(p) + "\n";
            }
            return str;
        }

        private string ToStringForProduct(Product product) {
            return string.Format("[Product id: {0} hasReceipt: {1} receipt: {2}]", product.definition.id, product.hasReceipt, product.receipt);
        }
    }
}