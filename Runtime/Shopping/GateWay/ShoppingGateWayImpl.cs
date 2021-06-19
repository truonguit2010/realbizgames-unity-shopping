using System.Collections.Generic;
using RealbizGames.RulePattern;

namespace RealbizGames.Shopping
{
    public class ShoppingGateWayImpl : IShoppingGateWay
    {
        private static ShoppingGateWayImpl _instance;

        public static ShoppingGateWayImpl DefaultInstance {
            get {
                if (_instance == null) {
                    _instance = new ShoppingGateWayImpl();
                }
                return _instance;
            }
        }

        public ShoppingProviderInitState InitState => unityShoppingProvider.InitState;

        private UnityShoppingProvider unityShoppingProvider = new UnityShoppingProvider();

        private IRuleEngine<ShoppingItemResponseDTO, ShoppingItemDTO> buyRuleEngine;

        public ShoppingGateWayImpl() {

        }

        private void InitRuleEngine() {
            buyRuleEngine = new GenericRuleEngine<ShoppingItemResponseDTO, ShoppingItemDTO>();
            buyRuleEngine.AddRule(new IAPShoppingRule(unityShoppingProvider));
        }

        public ShoppingItemResponseDTO Buy(ShoppingItemDTO item)
        {
            return buyRuleEngine.Execute(item);
        }

        public void Initialize(List<ShoppingItemDTO> items)
        {
            unityShoppingProvider.InitializePurchasing(items);
        }

        public void RestorePurchases()
        {
            unityShoppingProvider.RestorePurchases();
        }

        public bool DidIBuy(string productId)
        {
            return unityShoppingProvider.DidIBuy(productId: productId);
        }

        public ShoppingItemLocalizedMetadataDTO GetLocalizedMetadata(string productId)
        {
            return unityShoppingProvider.GetLocalizedMetadata(productId: productId);
        }

        public void NotifyMyOwnItemsBy(ShoppingItemProvider provider, ShoppingItemType type, string actionIdentifer)
        {
            throw new System.NotImplementedException();
        }
    }
}
