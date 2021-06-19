using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;

namespace RealbizGames.Shopping
{
    public class Config
    {
        private static Config _DefaultInstance;
        public static Config DefaultInstance
        {
            get
            {
                if (_DefaultInstance == null)
                {
                    _DefaultInstance = new Config();
                }
                return _DefaultInstance;
            }
        }

        private Config()
        {

        }
        public bool enableAutoCompleteConsumableProduct = true;

        public bool enableLog = true;

        public CrossPlatformValidator validator;
    }
}