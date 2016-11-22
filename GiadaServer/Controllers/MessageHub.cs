using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace GiadaServer.Controllers
{
    public class MessageHub : Hub
    {
        public void Send(string from, string to, string text)
        {
            Clients.All.broadcastMessage(from, to, text);
        }
    }
}
