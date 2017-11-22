using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CapturesQueue.Repositories;
using CapturesQueue.Services;
using Ninject;

namespace CapturesQueue
{
    public class IoC
    {
        private static StandardKernel _kernel;

        public static void Init()
        {
            _kernel = new StandardKernel();
            _kernel.Load(new NinjectBindings());
        }

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        class NinjectBindings : Ninject.Modules.NinjectModule
        {
            public override void Load()
            {
                Bind<ICaptureService>().To<CaptureService>();
                Bind<IScreenShotService>().To<CasperJsService>();
                Bind<MongoRepository>().To<MongoRepository>();
                Bind<IQueueService>().To<QueueService>();
            }
        }
    }
}
