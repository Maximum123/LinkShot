using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RabbitMQ.Client;

namespace LinkShort.Services
{
    public class QueueService
    {
        private  IModel model;
        public QueueService()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "crimcol.myqnapcloud.com";
            connectionFactory.UserName = "guest";
            connectionFactory.Password = "guest";
            connectionFactory.Port = 15672;

            IConnection connection = connectionFactory.CreateConnection();
            model = connection.CreateModel();
        }

        public void AddToQueue(string link)
        {
            var linkToSite = new Dictionary<string, object>();
            linkToSite.Add(link, null);
            model.QueueDeclare("linkShots", true, true, true, linkToSite);
        }
    }
}