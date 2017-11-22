using System;
using System.Reflection;
using CapturesQueue.Services;
using Ninject;
using Topshelf;

namespace CapturesQueue
{
    class Program
    {
        static void Main(string[] args)
        {
            IoC.Init();
            HostFactory.Run(x =>
            {
                x.Service<IQueueService>(s =>
                {
                    s.ConstructUsing(name => IoC.Get<IQueueService>());
                    s.WhenStarted(q => q.Receive());
                    s.WhenStopped(q => (q as IDisposable).Dispose());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Capture Service");
                x.SetDisplayName("Capture Service");
                x.SetServiceName("CaptureQueue");
            });

        }
    }
}
