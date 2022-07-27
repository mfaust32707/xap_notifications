using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationRequestDTO
    {
        [JsonProperty("RecipientIds")]
        public List<int> RecipientIds { get; set; }

        [JsonProperty("ApplicationIds")]
        public List<int> ApplicationIds { get; set; }

        [JsonProperty("Parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty("ExpirationDateTime")]
        public DateTime ExpirationDateTime { get; set; }

    }
}
