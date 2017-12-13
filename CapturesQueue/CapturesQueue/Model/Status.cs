using MongoDB.Bson;
using MongoDB.Driver;
using System.Configuration;

namespace CapturesQueue.Model
{
    public enum Status
    {
        Ready,
        InProgress,
        Failed,
    }
}
