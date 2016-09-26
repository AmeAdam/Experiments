using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmeWeb
{
    public interface IAmeMainService : IDisposable
    {
        void Start();
        void Stop();
    }

    public class AmeMainService : IAmeMainService
    {
        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            
        }
    }
}
