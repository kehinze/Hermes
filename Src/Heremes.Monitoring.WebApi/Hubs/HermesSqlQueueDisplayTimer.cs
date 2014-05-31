using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using Hermes.Monitoring.DataAcess;
using Hermes.Monitoring.Statistics;
using Hermes.Monitoring.WebApi.Dto;
using Hermes.Serialization.Json;
using Microsoft.AspNet.SignalR;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class HermesSqlQueueDisplayTimer
    {
        private static TableSchemaMonitor tableSchemaMonitor;
        private IHubContext hubContext;

        public HermesSqlQueueDisplayTimer()
        {
            tableSchemaMonitor = new TableSchemaMonitor(new JsonObjectSerializer());

            tableSchemaMonitor.OnTableSchemaMonitorComplete += timer_Elapsed;
            hubContext = GlobalHost.ConnectionManager.GetHubContext<HermesSqlQueueDisplayHub>();
        }

        private void timer_Elapsed(object sender, TableSchemaMonitorEventArgs e)
        {
            var tableSchemas = new
                {
                    TableSchemas = e.TableSchemas.Select(c => c.Value),
                    MonitoringPeriod = e.MonitorPeriod
                };
            
            hubContext.Clients.All.updateTableSchema(tableSchemas);
        }
    }

}