using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace CapturesQueue
{
    public class LinkData: BsonDocument
    {
        public string Url { get; set; }
        public string LinkToScreenPreview { get; set; }
        public string LinkToScreen { get; set; }
    }
}
