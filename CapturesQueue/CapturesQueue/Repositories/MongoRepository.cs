using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Configuration;
using CapturesQueue.Model;

namespace CapturesQueue.Repositories
{
    public class MongoRepository
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public MongoRepository()
        {
            var connectionString = ConfigurationManager.AppSettings["Mongo.Connection"];
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(ConfigurationManager.AppSettings["Mongo.DabaseName"]);
            _collection = database.GetCollection<BsonDocument>(ConfigurationManager.AppSettings["Mongo.TableName"]);
        }

        public void Create(string message, string linkToScreen, bool isException = false)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Url", message);
            var document = _collection.Find(filter).FirstOrDefault();
            BsonValue linkStatus = null;
            
            if ((document == null || document.TryGetValue("Status", out linkStatus))  && linkStatus != null && linkStatus.ToString().Equals(Status.Failed.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                document = new BsonDocument
                {
                    { "Url", message},
                    { "LinkToScreenPreview", ""},
                    {"LinkToScreen", linkToScreen },
                    { "DateCreated", DateTime.UtcNow },
                    { "Status", isException ? Status.Failed.ToString() : string.IsNullOrEmpty(linkToScreen) ? Status.InProgress.ToString() : Status.Ready.ToString()},
                };
                _collection.InsertOne(document);
            }
        }

    }
}
