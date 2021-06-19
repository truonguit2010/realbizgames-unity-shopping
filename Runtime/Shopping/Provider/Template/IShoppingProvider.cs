using System.Collections.Generic;

namespace RealbizGames.Shopping
{
    public interface IShoppingProvider
    {
        void InitializePurchasing(List<ShoppingItemDTO> items);

        ShoppingProviderInitState InitState { get; }

        ShoppingItemResponseDTO Buy(ShoppingItemDTO item);

        void RestorePurchases();

        /// <summary>
        /// Check xem tôi đã mua production này hay chưa?
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        bool DidIBuy(string productId);

        ShoppingItemLocalizedMetadataDTO GetLocalizedMetadata(string productId);

        void NotifyMyOwnItemsBy(ShoppingItemType type, string actionIdentifer);

        void PrintDebug();
    }
}