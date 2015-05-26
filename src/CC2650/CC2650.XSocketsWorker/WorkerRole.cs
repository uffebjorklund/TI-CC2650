using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using XSockets.Core.Common.Socket;
using XSockets.Plugin.Framework;

namespace CC2650.XSocketsWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        private IXSocketServerContainer _container;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            try
            {
                this.RunAsync(this._cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this._runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            bool result = base.OnStart();

            //Get the server container
            _container = Composable.GetExport<IXSocketServerContainer>();                       
            //Start in worker role
            _container.StartOnAzure();
            
            return result;
        }

        public override void OnStop()
        {
            _container.Stop();

            this._cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(30000);
            }
        }
    }
}
