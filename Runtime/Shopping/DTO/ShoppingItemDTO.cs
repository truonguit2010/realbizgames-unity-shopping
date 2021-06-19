
namespace RealbizGames.Shopping
{
    public class ShoppingItemDTO
    {
        private string _itemName;
        private string _itemId;

        private ShoppingItemType _type;

        private ShoppingItemProvider _provider;

        private object definition;

        public string ItemName { get => _itemName; set => _itemName = value; }
        public string ItemId { get => _itemId; set => _itemId = value; }
        public ShoppingItemType Type { get => _type; set => _type = value; }
        public ShoppingItemProvider Provider { get => _provider; set => _provider = value; }
        public object Definition { get => definition; set => definition = value; }
    }

}
