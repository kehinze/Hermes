using System.Linq;
using System.Timers;
using System.Web;
using Hermes.Messaging.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class HermesPerformenceHub : Hub
    {
        private string hello;

        public HermesPerformenceHub()
        {
            hello = "hello";
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            return base.OnConnected();
        }
    }
}