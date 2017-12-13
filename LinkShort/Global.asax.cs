using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Freezer.Core;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace LinkShort
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private IModel model;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HttpConfiguration config = GlobalConfiguration.Configuration;
            //config.Formatters.JsonFormatter.SerializerSettings.Converters.Add
            //    (new Newtonsoft.Json.Converters.StringEnumConverter());
            // DeclareToQueue();
        }

        private void DeclareToQueue()
        {
             ConnectToQueue();

            var consumer = new EventingBasicConsumer(model);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                Account account = new Account(
                    "dmlke7dxo",
                    "359384259728433",
                    "inMYO1qmZO4wqbXDZ8uPBacmXd0");

                Cloudinary cloudinary = new Cloudinary(account); 

              

            };
            model.BasicConsume(queue: "linkShots",
                autoAck: true,
                consumer: consumer);
        }

        private void ConnectToQueue()
        {
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = "crimcol.myqnapcloud.com";
            IConnection connection = connectionFactory.CreateConnection();
            model = connection.CreateModel();
            model.QueueDeclare("linkShots", true, true, false, null); 
        }
    }
}
