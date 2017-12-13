using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapturesQueue.Model;

namespace CapturesQueue.Helpers
{
    public static class StringHelper
    {
        public static string AddCommasIfRequired(this string path)
        {
            return (path.Contains(" ")) ? "\"" + path + "\"" : path;
        }

        public static T ToEnum<T>(this string value, T defaultValue) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            T result;

            return Enum.TryParse(value, true, out result) ? result : defaultValue;
        }
    }
}
