
namespace RealbizGames.Shopping
{
    [System.Serializable]
    public class PurchaseSucceedEvent : UnityEngine.Events.UnityEvent<ShoppingSuccessDTO> { };

    [System.Serializable]
    public class PurchaseFailedEvent : UnityEngine.Events.UnityEvent<ShoppingFailDTO> { };

    [System.Serializable]
    public class OnInitializeFailedEvent : UnityEngine.Events.UnityEvent { };

    [System.Serializable]
    public class OnInitializeSuccessEvent : UnityEngine.Events.UnityEvent { };

    public class ShoppingNotificationCenter
    {
        private static ShoppingNotificationCenter _instance;

        public static ShoppingNotificationCenter DefaultInstance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ShoppingNotificationCenter();
                }
                return _instance;
            }
        }

        public readonly PurchaseSucceedEvent purchaseSucceedEvent = new PurchaseSucceedEvent();

        public readonly PurchaseFailedEvent purchaseFailedEvent = new PurchaseFailedEvent();

        public readonly OnInitializeFailedEvent onInitializeFailedEvent = new OnInitializeFailedEvent();

        public readonly OnInitializeSuccessEvent onInitializeSuccessEvent = new OnInitializeSuccessEvent();

        public void NotifySuccess(ShoppingSuccessDTO item)
        {
            purchaseSucceedEvent.Invoke(item);
        }

        public void NotifyFail(ShoppingFailDTO dto)
        {
            purchaseFailedEvent.Invoke(dto);
        }

        public void NotifyInitializeSuccess()
        {
            onInitializeSuccessEvent.Invoke();
        }

        public void NotifyInitializeFailed()
        {
            onInitializeFailedEvent.Invoke();
        }
    }

}
