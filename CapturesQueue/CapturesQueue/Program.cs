using System.Reflection;
using CapturesQueue.Services;
using Ninject;

namespace CapturesQueue
{
    class Program
    {
        private static IKernel kernel;
        static void Main(string[] args)
        {
            kernel = new StandardKernel();
            kernel.Load(Assembly.GetExecutingAssembly());
            var _queue = kernel.Get<IQueueService>();
            _queue.Receive();
        }
    }
}
