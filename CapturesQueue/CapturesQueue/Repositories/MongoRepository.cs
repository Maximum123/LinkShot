using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;

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

        public void Create(string message, string linkToScreen)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("Url", message);
            var document = _collection.Find(filter).FirstOrDefault();
            if (document == null)
            {
                document = new BsonDocument
                {
                    { "Url", message},
                    { "LinkToScreenPreview", ""},
                    {"LinkToScreen", linkToScreen }
                };
                _collection.InsertOne(document);
            }
        }

    }
}
