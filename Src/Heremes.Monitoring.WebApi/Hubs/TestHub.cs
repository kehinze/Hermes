using Microsoft.AspNet.SignalR;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class TestHub : Hub
    {
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
           
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}