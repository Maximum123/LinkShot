using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CapturesQueue.Services
{
    public class PhantomJsCloudService : IScreenShotService
    {
        private const string ApiKey = "ak-k359b-fvths-s94nj-bkerc-xkmbm";
        public byte[] GetScreenBytesByUrl(string url)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.ExpectContinue = false; //REQUIRED! or you will get 502 Bad Gateway errors
                                                                     //you should look at the HTTP Endpoint docs, section about "userRequest" and "pageRequest" 
                                                                     //for a listing of all the parameters you can pass via the "pageRequestJson" variable.

                var pageRequestJson = new StringContent($"{{'url':'{url}','renderType':'jpeg', renderSettings:{{viewport:{{width:1024,height:768}},clipRectangle:{{width:1024,height:768}} }} }}");
                var response = client.PostAsync($"https://PhantomJScloud.com/api/browser/v2/{ApiKey}/", pageRequestJson).Result;
                var responseByte = response.Content.ReadAsByteArrayAsync().Result;
                return responseByte;
            }
        }
    }
}
