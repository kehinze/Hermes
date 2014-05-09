using System.Runtime.Serialization;

namespace Hermes.Monitoring.WebApi.Dto
{
    [DataContract(Name = "HermesPerformenceDto")]
    public class HermesPerformenceDto
    {
        [DataMember(Name = "TotalTimeIntervalInSeconds")]
        public double TotalTimeIntervalInSeconds { get; set; }

        [DataMember(Name = "MessagesPerSecond")]
        public double MessagesPerSecond { get; set; }

        [DataMember(Name = "AverageTimeToDeliverMessages")]
        public double AverageTimeToDeliverMessages { get; set; }

        [DataMember(Name = "AverageTimeToProcessMessages")]
        public double AverageTimeToProcessMessages { get; set; }
    }
}