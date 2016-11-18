using System.ServiceProcess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GiadaServer
{
    public class GiadaMainService : ServiceBase, IGiadaMainService
    {
        private IWebHost host;
        private bool stopRequestedByWindows;

        public void Run(IWebHost webHost, bool isDebug, string[] args)
        {
            host = webHost;

            if (isDebug)
            {
                OnStarted();
                using (host)
                {
                    host.Run();
                    OnStopping();
                    OnStopped();
                }
            }
            else
                Run(this);
        }

        protected sealed override void OnStart(string[] args)
        {
            OnStarting(args);
            host.Services.GetService<IApplicationLifetime>().ApplicationStopped.Register(() =>
            {
                if (stopRequestedByWindows)
                    return;
                Stop();
            });
            host.Start();
        }

        protected sealed override void OnStop()
        {
            stopRequestedByWindows = true;
            OnStopping();
            host?.Dispose();
            OnStopped();
        }

        /// <summary>Executes before ASP.NET Core starts.</summary>
        /// <param name="args">The command line arguments passed to the service.</param>
        protected virtual void OnStarting(string[] args)
        {
        }

        /// <summary>Executes after ASP.NET Core starts.</summary>
        protected virtual void OnStarted()
        {
        }

        /// <summary>Executes before ASP.NET Core shuts down.</summary>
        protected virtual void OnStopping()
        {
        }

        /// <summary>Executes after ASP.NET Core shuts down.</summary>
        protected virtual void OnStopped()
        {
        }
    }
}