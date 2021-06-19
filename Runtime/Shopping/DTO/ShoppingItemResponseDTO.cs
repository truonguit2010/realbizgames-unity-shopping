
namespace RealbizGames.Shopping
{
    public class ShoppingItemResponseDTO
    {
        public BuyItemStatus status;

        public ShoppingItemResponseDTO(BuyItemStatus status)
        {
            this.status = status;
        }
    }
}