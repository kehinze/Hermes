using System;
using System.Collections.Generic;
using Hermes.Monitoring.Statistics;
using Hermes.Monitoring.WebApi.Dto;
using Hermes.Serialization.Json;
using Microsoft.AspNet.SignalR;

namespace Hermes.Monitoring.WebApi.Hubs
{
    public class HermesPerformenceTimer
    {
        private static SqlTransportPerfomanceMonitor sqlTransportPerfomanceMonitor;
        private IHubContext hubContext;

        public HermesPerformenceTimer()
        {
            sqlTransportPerfomanceMonitor = new SqlTransportPerfomanceMonitor(new JsonObjectSerializer());
            sqlTransportPerfomanceMonitor.OnPerformancePeriodCompleted += timer_Elapsed;
            hubContext = GlobalHost.ConnectionManager.GetHubContext<HermesPerformenceHub>();
        }

        void timer_Elapsed(object sender, PerformanceMetricEventArgs e)
        {
            var performence = new HermesPerformenceDto
            {
                AverageTimeToDeliverMessages = e.PerformanceMetric.AverageTimeToDelivery.TotalSeconds,
                AverageTimeToProcessMessages = e.PerformanceMetric.AverageTimeToProcess.TotalSeconds,
                MessagesPerSecond = e.PerformanceMetric.Count / e.MonitorPeriod.TotalSeconds,
                TotalTimeIntervalInSeconds = e.MonitorPeriod.TotalSeconds
            };

            hubContext.Clients.All.updateHermesPerformenceMonitor(performence);
        }
    }
}