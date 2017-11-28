using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using LinkShort.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;

namespace LinkShort.Services
{
    public class QueueService
    {
        private static IModel model;
        private static IMongoCollection<BsonDocument> _collection;

        static QueueService()
        {
            MongoDBConnection();
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "crimcol.myqnapcloud.com";
            //connectionFactory.UserName = "guest";
            //connectionFactory.Password = "guest";
            IConnection connection = connectionFactory.CreateConnection();
            model = connection.CreateModel();
            model.QueueDeclare(queue: "linkShots",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            model.ConfirmSelect();
        }

        public LinkModel AddToQueue(string link)
        {
            var screenUrl = GetLinkToScreen(link);
            if (!string.IsNullOrEmpty(screenUrl))
            {
                return FillLinkModel(link, screenUrl, Status.Ready);
            }
            try
            {
                var body = Encoding.UTF8.GetBytes(link);
                IBasicProperties bp = model.CreateBasicProperties();
                bp.DeliveryMode = 2;

                model.BasicPublish(exchange: "",
                    routingKey: "linkShots",
                    basicProperties: bp,
                    body: body);

                for (var i = 0; i < 15; i++)
                {
                    Thread.Sleep(1000);
                    screenUrl = GetLinkToScreen(link);
                    if (!string.IsNullOrEmpty(screenUrl))
                    {
                        return FillLinkModel(link, screenUrl, Status.Ready);
                    }
                }
            }
            catch (Exception ex)
            {
                FillLinkModel(link, screenUrl, Status.Failed);
            }
            return FillLinkModel(link, screenUrl, Status.InProgress);
        }

        private LinkModel FillLinkModel(string url, string screenUrl, Status status)
        {
            var result = new LinkModel
            {
                Url = url,
                Status = status,
                ImageUrl = screenUrl
            };
            return result;
        }

        private static string GetLinkToScreen(string link)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Url", link);
            var document = _collection.Find(filter).FirstOrDefault();
            if (document != null)
            {
                return document["LinkToScreen"].AsString;
            }
            return string.Empty;
        }

        private static void MongoDBConnection()
        {
            var client = new MongoClient("mongodb://crimcol.myqnapcloud.com:27017");
            var database = client.GetDatabase("LinkCapture");
            _collection = database.GetCollection<BsonDocument>("urls");
        }
    }
}