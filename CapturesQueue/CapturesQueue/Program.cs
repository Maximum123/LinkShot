using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Freezer.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Screenshot = Freezer.Core.Screenshot;

namespace CapturesQueue
{
    class Program
    {
        private static IMongoCollection<BsonDocument> _collection;
        private static ChromeDriver _chromDeriver;
      
        static void Main(string[] args)
        {
           // FreezerInit();
          //  ChromeDriverInit();
            MongoDBConnection();
            Receive();
            //var factory = new ConnectionFactory() { HostName = "crimcol.myqnapcloud.com" };
            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "testQueue",
            //        durable: false,
            //        exclusive: false,
            //        autoDelete: false,
            //        arguments: null);

            //    string message = "Hello World!";
            //    var body = Encoding.UTF8.GetBytes(message);

            //    channel.BasicPublish(exchange: "",
            //        routingKey: "testQueue",
            //        basicProperties: null,
            //        body: body);
            //    Console.WriteLine(" [x] Sent {0}", message);
            //}
     
        }

        private static void ChromeDriverInit()
        {

            _chromDeriver = new ChromeDriver();
            _chromDeriver.Manage().Window.Size = new System.Drawing.Size(1024, 768);
        }

        private static void MongoDBConnection()
        {
            var client = new MongoClient("mongodb://crimcol.myqnapcloud.com:27017");
            var database = client.GetDatabase("LinkCapture");
            _collection = database.GetCollection<BsonDocument>("urls");
        }

        private static void Receive()
        {
            var factory = new ConnectionFactory() { HostName = "crimcol.myqnapcloud.com" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "linkShots",
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //channel.ConfirmSelect();

                var consumer = new EventingBasicConsumer(channel);
                Semaphore semaphore = new Semaphore(5, 5);
                consumer.Received += (model, ea) =>
                {
                   
                   Task task = Task.Run(() =>
                    {
                        semaphore.WaitOne();
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                        try
                        {
                            AddToMongoIfNeed(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exeption is " + ex);
                        }
                        channel.BasicAck(ea.DeliveryTag, false);
                        semaphore.Release();
                    }
                    );
                    //task.Wait();
                   
                };
                channel.BasicConsume(queue: "linkShots",
                    autoAck: false,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
        }

        private static void AddToMongoIfNeed(string message)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Url", message);
            var document = _collection.Find(filter).FirstOrDefault();
            if (document == null)
            {
                byte[] screenshotBytes = null;
                try
                {
                    screenshotBytes = GetScreenShotCasperJSByUrl(message);
                }
                catch (Exception ex)
                {

                }
                var linkToScreen = GetScreenshotLink(screenshotBytes);
                document = new BsonDocument
                {
                    { "Url", message},
                    { "LinkToScreenPreview", ""},
                    {"LinkToScreen", linkToScreen }
                };
                _collection.InsertOne(document);

            }
        }

        private static string GetScreenshotLink(byte[] screenshotBytes)
        {
            if (screenshotBytes == null)
                return String.Empty;

            Account account = new Account(
                "dmlke7dxo",
                "359384259728433",
                "inMYO1qmZO4wqbXDZ8uPBacmXd0");

            Cloudinary cloudinary = new Cloudinary(account);

            using (Stream ms = new MemoryStream(screenshotBytes))
            {

                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(Guid.NewGuid().ToString(), ms)
                };
                var uploadResult = cloudinary.Upload(uploadParams);

                return uploadResult.Uri.ToString();
            }
        }

        private static void FreezerInit()
        {
            FreezerGlobal.Initialize();
        }


        private static byte[] GetScreenShotCasperJSByUrl(string url)
        {
            CasperJsHelper.CasperJsHelper cjs = new CasperJsHelper.CasperJsHelper("casperjs-1.1.3");
            var scriptPath =Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scripts", "TakeScreenshot.js");
            var tempFileName = Guid.NewGuid() +".png";
            cjs.Run(AddCommasIfRequired(scriptPath), new string[] {$"--url={url}", $"--fileName={tempFileName}"});

            var screenPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "casperjs-1.1.3",  "temp",
                tempFileName);
            var result = File.ReadAllBytes(screenPath);
            File.Delete(screenPath);

            return result;
        }

        private static byte[] GetScreenShotSeleniumGCByUrl(string url)
        {
            _chromDeriver.Navigate().GoToUrl(url);
            OpenQA.Selenium.Screenshot ss = ((ITakesScreenshot)_chromDeriver).GetScreenshot();
            byte[] screenshotAsByteArray = ss.AsByteArray;
            return screenshotAsByteArray;
        }

        private static byte[] GetScreenShotFreezerByUrl(string url)
        {
      
                var screenshotJob = ScreenshotJobBuilder.Create(url)
                    .SetBrowserSize(1024, 768)
                    .SetCaptureZone(CaptureZone.VisibleScreen) // Set what should be captured
                    .SetTrigger(new WindowLoadTrigger(50)); // Set when the picture is taken

                var screen = screenshotJob.Freeze();
                Console.WriteLine("screenshot is done");
            

            return screen;
        }

        private static string AddCommasIfRequired(string path)
        {
            return (path.Contains(" ")) ? "\"" + path + "\"" : path;
        }
    }
}
