using System.Collections.Generic;

namespace RealbizGames.Shopping
{
    public interface IShoppingGateWay
    {
        void Initialize(List<ShoppingItemDTO> items);

        ShoppingItemResponseDTO Buy(ShoppingItemDTO item);

        void RestorePurchases();

        bool DidIBuy(string productId);

        ShoppingItemLocalizedMetadataDTO GetLocalizedMetadata(string productId);

        ShoppingProviderInitState InitState { get; }

        // Check các item mình đã mua để call lại flow purchase success.
        void NotifyMyOwnItemsBy(ShoppingItemProvider provider, ShoppingItemType type, string actionIdentifer);

    }
}

