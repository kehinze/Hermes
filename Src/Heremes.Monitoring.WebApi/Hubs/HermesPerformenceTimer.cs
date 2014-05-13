using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

            var endpointPerformences = e.PerformanceMetric.GetEndpointPerformance()
                .ToDtoArray(e.MonitorPeriod);

            hubContext.Clients.All.updateHermesPerformenceMonitor(performence);
            hubContext.Clients.All.updateEndpointPerformence(endpointPerformences);
        }
    }

    [DataContract(Name = "EndpointDto")]
    public class EndpointDto
    {
        [DataMember(Name = "Machine")]
        public string Machine { get; set; }

        [DataMember(Name = "Queue")]
        public string Queue { get; set; }
    }

    public static class MapToDto
    {
        public static EndpointPerformenceDto[] ToDtoArray(this IEnumerable<EndpointPerformanceMetric> endpointPerformanceMetric, TimeSpan monitoringPeriod)
        {
            return endpointPerformanceMetric.Select(endpointPerformenceDto => endpointPerformenceDto.ToDto(monitoringPeriod)).ToArray();
        }

        public static EndpointPerformenceDto ToDto(this EndpointPerformanceMetric endpointPerformanceMetric, TimeSpan monitoringPeriod)
        {
            return new EndpointPerformenceDto
                {
                    AverageTimeToDeliver = endpointPerformanceMetric.AverageTimeToDeliver.TotalSeconds,
                    AverageTimeToProcess = endpointPerformanceMetric.AverageTimeToProcess.TotalSeconds,
                    Endpoint = new EndpointDto
                        {
                            Machine = endpointPerformanceMetric.Endpoint.Machine,
                            Queue = endpointPerformanceMetric.Endpoint.Queue
                        },
                    TotalMessagesProcessed = endpointPerformanceMetric.TotalMessagesProcessed,
                    AverageMessagesPerSecond = endpointPerformanceMetric.TotalMessagesProcessed / monitoringPeriod.TotalSeconds,
                    TimeIntervalInSeconds =  monitoringPeriod.TotalSeconds
                };
        }
    }

    [DataContract(Name = "EndpointPerformenceDto")]
    public class EndpointPerformenceDto
    {
        [DataMember(Name = "AverageTimeToDeliver")]
        public double AverageTimeToDeliver { get; set; }

        [DataMember(Name = "AverageTimeToProcess")]
        public double AverageTimeToProcess { get; set; }

        [DataMember(Name = "TotalMessagesProcessed")]
        public int TotalMessagesProcessed { get; set; }

        [DataMember(Name = "AverageMessagesPerSecond")]
        public double AverageMessagesPerSecond { get; set; }

        [DataMember(Name = "TimeIntervalInSeconds")]
        public double TimeIntervalInSeconds { get; set; }

        [DataMember(Name = "Endpoint")]
        public EndpointDto Endpoint { get; set; } 
    }

}