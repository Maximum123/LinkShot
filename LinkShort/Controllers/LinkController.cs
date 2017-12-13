using LinkShort.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using LinkShort.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LinkShort.Controllers
{
   // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LinkController : ApiController
    {
        // GET api/<controller>
        public object Get(string link)   
        {
            var queue = new QueueService(); 
            var linkToShot = queue.AddToQueue(link);
            //var screenService = new ScreenShortService();
            //var screenSort = screenService.GetScreenShortByUrl(link);
            //Stream stream = new MemoryStream(screenSort);
            //HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            //result.Content = new StreamContent(stream);
            //result.Content.Headers.ContentType =
            //    new MediaTypeHeaderValue("application/octet-stream");
            //return result;
            return linkToShot;
        }

     //   [System.Web.Http.HttpPost]
        public object Post([FromBody]string[] links)
        {
            
            var queue = new QueueService();
            var result = new List<LinkModel>();
            foreach (var link in links)
            {
                if (!link.Contains("http:") && !link.Contains("https:"))
                {
                    continue;
                }
                result.Add(queue.AddToQueue(link));
            }

            //JsonConvert.DefaultSettings = (() =>
            //{
            //    var settings = new JsonSerializerSettings();
            //    settings.Converters.Add(new StringEnumConverter { CamelCaseText = true });
            //    return settings;
            //});
            //var converted = JsonConvert.SerializeObject(result);
            return result;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }
        // POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}