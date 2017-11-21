using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapturesQueue.Services
{
    public interface IScreenShotService
    {
        byte[] GetScreenBytesByUrl(string url);
    }
}
