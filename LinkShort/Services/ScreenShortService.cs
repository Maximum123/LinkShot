using Freezer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkShort.Services
{
    public class ScreenShortService
    {
        public ScreenShortService()
        {
            FreezerGlobal.Initialize();
        }
        public byte[] GetScreenShortByUrl(string url)
        {
            var screenshotJob = ScreenshotJobBuilder.Create(url)
              .SetBrowserSize(1366, 768)
              .SetCaptureZone(CaptureZone.FullPage) // Set what should be captured
              .SetTrigger(new WindowLoadTrigger(50)); // Set when the picture is taken

            var screen =   screenshotJob.Freeze();
            return screen;
        }
    }
}