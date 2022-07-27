using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NotificationsService.Objects
{
    public class ReportRunDetails
    {
        [JsonPropertyName("ReportId")]
        public int ReportId { get; set; }

        [JsonPropertyName("DownloadFileName")]
        public string DownloadFileName { get; set; }

        [JsonPropertyName("Parameters")]
        public List<ReportParameter> Parameters { get; set; }
    }

    public class ReportParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
