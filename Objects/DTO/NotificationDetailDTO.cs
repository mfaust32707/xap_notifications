
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationDetailDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("MessageFormat")]
        public string MessageFormat { get; set; }

        [JsonProperty("MessageTemplate")]
        public string MessageTemplate { get; set; }

        [JsonProperty("NotificationId")]
        public int NotificationId { get; set; }

    }
}

