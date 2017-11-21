using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapturesQueue.Repositories;
using CapturesQueue.Services;

namespace CapturesQueue
{
    public class NinjectBindings: Ninject.Modules.NinjectModule
    {
        public override void Load()
        {
            Bind<ICaptureService>().To<CaptureService>();
            Bind<IScreenShotService>().To<PhantomJsCloudService>();
            Bind<MongoRepository>().To<MongoRepository>();
            Bind<IQueueService>().To<QueueService>();
        }
    }
}
