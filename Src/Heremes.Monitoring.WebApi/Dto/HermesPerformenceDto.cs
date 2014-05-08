using System.Runtime.Serialization;

namespace Hermes.Monitoring.WebApi.Dto
{
    [DataContract(Name = "HermesPerformenceDto")]
    public class HermesPerformenceDto
    {
        [DataMember(Name = "MessagesPerSecond")]
        public double MessagesPerSedond { get; set; }

        [DataMember(Name = "AverageTimeToDeliverMessagesPerSecond")]
        public double AverageTimeToDeliverMessagesPerSecond { get; set; }

        [DataMember(Name = "AverageTimeToProcessMessagesPerSecond")]
        public double AverageTimeToProcessMessagesPerSecond { get; set; }
    }
}