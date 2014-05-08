using System.Collections.Generic;
using Hermes.Monitoring.Statistics;
using Hermes.Monitoring.WebApi.Dto;
using Hermes.Serialization.Json;
using Microsoft.AspNet.SignalR;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class HermesPerformenceTimer
    {
        private SqlTransportPerfomanceMonitor sqlTransportPerfomanceMonitor;
        private Queue<HermesPerformenceDto>  performenceDtos = new Queue<HermesPerformenceDto>();
        private IHubContext hubContext;

        public HermesPerformenceTimer()
        {
            sqlTransportPerfomanceMonitor = new SqlTransportPerfomanceMonitor(new JsonObjectSerializer());
            sqlTransportPerfomanceMonitor.OnPerformancePeriodCompleted += timer_Elapsed;
            hubContext = GlobalHost.ConnectionManager.GetHubContext<HermesPerformenceHub>();
        }

        void timer_Elapsed(object sender, PerformanceMetricEventArgs e)
        {
            performenceDtos.Enqueue(new HermesPerformenceDto
            {
                AverageTimeToDeliverMessagesPerSecond = e.PerformanceMetric.Count,
                AverageTimeToProcessMessagesPerSecond = e.PerformanceMetric.AverageTimeToDelivery.Milliseconds,
                MessagesPerSedond = e.PerformanceMetric.AverageTimeToProcess.Milliseconds
            });

            if (performenceDtos.Count > 100)
            {
                performenceDtos.Dequeue();
            }

            hubContext.Clients.All.updateHermesPerformenceMonitor(performenceDtos.ToArray());
        }
    }
}