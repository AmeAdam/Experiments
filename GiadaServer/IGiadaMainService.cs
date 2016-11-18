using System;
using Microsoft.AspNetCore.Hosting;

namespace GiadaServer
{
    public interface IGiadaMainService : IDisposable
    {
        void Run(IWebHost webHost, bool isDebug, string[] args);
    }
}
