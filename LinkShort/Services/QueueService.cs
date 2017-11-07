using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using RabbitMQ.Client;

namespace LinkShort.Services
{
    public class QueueService
    {
        private  static  IModel model;
        static  QueueService()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "crimcol.myqnapcloud.com";
            //connectionFactory.UserName = "guest";
            //connectionFactory.Password = "guest";
            IConnection connection = connectionFactory.CreateConnection(); 
            model = connection.CreateModel();
            model.QueueDeclare("linkShots", true, true, false, null);
        }

        public void AddToQueue(string link)
        {
            var body = Encoding.UTF8.GetBytes(link);
            model.BasicPublish(exchange: "",
                routingKey: "linkShots",
                basicProperties: null,
                body: body);
        }
    }
}