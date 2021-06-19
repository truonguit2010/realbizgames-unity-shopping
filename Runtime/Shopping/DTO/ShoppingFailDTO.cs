
namespace RealbizGames.Shopping
{
    public class ShoppingFailDTO
    {
        private ShoppingItemDTO _item;
        private ShoppingFailType _failType;

        private string _failCode;

        public ShoppingFailType FailType { get => _failType; set => _failType = value; }
        public ShoppingItemDTO Item { get => _item; set => _item = value; }
        public string FailCode { get => _failCode; set => _failCode = value; }
    }
}