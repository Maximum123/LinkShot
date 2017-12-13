using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CapturesQueue.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CapturesQueue.Services
{
    public class QueueService : IQueueService, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        private readonly MongoRepository _repository;
        private readonly IScreenShotService _screenShotService;
        private readonly ICaptureService _сaptureService;

        public QueueService(MongoRepository repository, IScreenShotService screenShotService, ICaptureService сaptureService)
        {
            _factory = new ConnectionFactory() {HostName = ConfigurationManager.AppSettings["RabbitMQ.Host"]};
            _queueName = ConfigurationManager.AppSettings["RabbitMQ.QueueName"];
            _repository = repository;
            _screenShotService = screenShotService;
            _сaptureService = сaptureService;
        }

        public void Dispose()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void Receive()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);

                //channel.ConfirmSelect();

                Semaphore semaphore = new Semaphore(5, 5);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    semaphore.WaitOne();
                    Task task = new Task(() =>
                    {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        HandleMessage(message, channel);
                        channel.BasicAck(ea.DeliveryTag, false);
                        semaphore.Release();
                    });  
                    task.Start();
                };

                channel.BasicConsume(queue: "linkShots",
                autoAck: false,
                consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
        }

        private void HandleMessage(string message, IModel channel)
        {
            Console.WriteLine(" [x] Received {0}", message);
            try
            {
                var bytes = _screenShotService.GetScreenBytesByUrl(message);
                var linkToScreen = _сaptureService.UploadScreenShot(bytes);
                _repository.Create(message, linkToScreen);
                Console.WriteLine("Screenshot done.");
            }
            catch (Exception ex)
            {
                _repository.Create(message, "", true);
                Console.WriteLine("Exeption is " + ex);
            }
        }
    }
}
