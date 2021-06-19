
namespace RealbizGames.Shopping
{
    public class ShoppingSuccessDTO
    {
        private ShoppingItemDTO _item;

        private string _actionIdentifer;

        public ShoppingSuccessDTO(ShoppingItemDTO item, string actionIdentifer)
        {
            Item = item;
            ActionIdentifer = actionIdentifer;
        }

        public ShoppingItemDTO Item { get => _item; set => _item = value; }
        public string ActionIdentifer { get => _actionIdentifer; set => _actionIdentifer = value; }
    }
}