using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationDTO
    {
      
        [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("Messages")]
        public List<NotificationDetailDTO> Messages { get; set; }

        [JsonProperty("Methods")]
        public List<NotificationMethodDTO> Methods { get; set; }

    }
}
