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
    public class QueueService : IQueueService
    {
        private readonly ConnectionFactory _factory;
        private readonly string _queueName;

        private readonly MongoRepository _repository;
        private readonly CasperJsService _casperJsService;
        private readonly ICaptureService _сaptureService;

        public QueueService(MongoRepository repository, CasperJsService casperJsService, ICaptureService сaptureService)
        {
            _factory = new ConnectionFactory() {HostName = ConfigurationManager.AppSettings["RabbitMQ.Host"]};
            _queueName = ConfigurationManager.AppSettings["RabbitMQ.QueueName"];
            _repository = repository;
            _casperJsService = casperJsService;
            _сaptureService = сaptureService;
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

                var consumer = new EventingBasicConsumer(channel);
                Semaphore semaphore = new Semaphore(5, 5);
                consumer.Received += (model, ea) =>
                {

                    //Task task = Task.Run(() =>
                        {
                            semaphore.WaitOne();
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);
                            try
                            {
                                var bytes = _casperJsService.GetScreenBytesByUrl(message);
                                var linkToScreen = _сaptureService.UploadScreenShot(bytes);
                                _repository.Create(message, linkToScreen);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exeption is " + ex);
                            }
                            channel.BasicAck(ea.DeliveryTag, false);
                            semaphore.Release();
                        }
                   // );
                    //task.Wait();

                };
                channel.BasicConsume(queue: "linkShots",
                    autoAck: false,
                    consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();

            }
        }
    }
}
