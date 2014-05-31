using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Hermes.Monitoring.DataAcess;
using Hermes.Serialization.Json;

namespace Hermes.Monitoring.WebApi.Controllers
{
    public class BrokerDataAccessController : ApiController
    {
        private readonly BrokerDataAccess brokerDataAccess;

        public BrokerDataAccessController()
        {
            brokerDataAccess = new BrokerDataAccess(new JsonObjectSerializer());
        }
        
        public object GetMessagesInQueue(string tableName, int pageNumber)
        {
            return brokerDataAccess.GetHermesMessages(tableName, pageNumber);
        }
        
    }
}
