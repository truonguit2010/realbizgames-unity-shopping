
namespace RealbizGames.Shopping
{
    public class IAPShoppingRule : IShoppingRule
    {
        private UnityShoppingProvider provider;

        public IAPShoppingRule(UnityShoppingProvider provider)
        {
            this.provider = provider;
        }

        public ShoppingItemResponseDTO Execute(ShoppingItemDTO expression)
        {
            return provider.Buy(expression);
        }

        public bool Valuate(ShoppingItemDTO expression)
        {
            return expression.Provider == ShoppingItemProvider.IAP;
        }
    }
}