using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkShort.Models
{
    public class LinkModel
    {
        public string Url { get; set; }
        public Status Status { get; set; }
        public string ImageUrl{ get; set; }
    }

    public enum Status
    {
        Ready,
        InProgress,
        Failed,
    }
}