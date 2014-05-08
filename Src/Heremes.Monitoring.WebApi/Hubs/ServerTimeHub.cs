using System;
using Microsoft.AspNet.SignalR;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class ServerTimeHub : Hub
    {
        public string GetServerTime()
        {
            return DateTime.UtcNow.ToString();
        }
    }
}