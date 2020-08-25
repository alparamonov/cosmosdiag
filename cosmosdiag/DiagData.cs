using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace cosmosdiag
{
    public class DiagData
    {
        public string DiagnosticVersion { get; set; }
        public Summary Summary { get; set; }
        public Context[] Context { get; set; }
    }

    public class Summary
    {
        public DateTime StartUtc { get; set; }
        public float TotalElapsedTimeInMs { get; set; }
        public string UserAgent { get; set; }
        public int TotalRequestCount { get; set; }
        public int FailedRequestCount { get; set; }
    }

    public class Context
    {
        public string Id { get; set; }
        public float ElapsedTimeInMs { get; set; }
        public string PKRangeId { get; set; }
        public DateTime StartUtc { get; set; }
        public string QueryMetric { get; set; }
        public string IndexUtilization { get; set; }
        public string ClientCorrelationId { get; set; }
        [JsonProperty("Context")]
        public ContextDetailed[] ContextDetailed { get; set; }
    }

    public class ContextDetailed
    {
        public string Id { get; set; }
        public object[] ContactedReplicas { get; set; }
        public string[] RegionsContacted { get; set; }
        public object[] FailedReplicas { get; set; }
        public float HandlerElapsedTimeInMs { get; set; }
        public string CpuHistory { get; set; }
        public float ElapsedTimeInMs { get; set; }
        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }
        public string TargetEndpoint { get; set; }
        public DateTime ResponseTimeUtc { get; set; }
        public string ResourceType { get; set; }
        public string OperationType { get; set; }
        public string LocationEndpoint { get; set; }
        public string ActivityId { get; set; }
        public string StoreResult { get; set; }
    }
}
