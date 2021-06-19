using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RealbizGames.Shopping
{

    public class ShoppingUtils
    {
        public const string ASSIGN = ": ";
        public const string SEP = " ";

        public static string ToStringFor(object obj)
        {
            PropertyInfo[] _PropertyInfos = obj.GetType().GetProperties();
            var sb = new StringBuilder();

            sb.Append("[").Append(obj.GetType().Name).Append(" ");
            foreach (var info in _PropertyInfos)
            {
                var value = info.GetValue(obj, null) ?? "(null)";
                sb.Append(info.Name).Append(ASSIGN).Append(value.ToString()).Append(SEP);
            }
            sb.Append("]");

            return sb.ToString();
        }

        public static string ToStringForList<T>(List<T> l)
        {
            string message = "Total: " + l.Count + "\n";
            foreach (T o in l)
            {
                message += o.ToString() + "\n";
            }
            return message;
        }
    }
}