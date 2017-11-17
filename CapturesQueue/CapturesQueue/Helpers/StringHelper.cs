using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapturesQueue.Helpers
{
    public static class StringHelper
    {
        public static string AddCommasIfRequired(this string path)
        {
            return (path.Contains(" ")) ? "\"" + path + "\"" : path;
        }
    }
}
