using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Extensions
{
    public static class ReflectionExtension
    {
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            //---Gets the value of whatever property is passing
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
    }
}
