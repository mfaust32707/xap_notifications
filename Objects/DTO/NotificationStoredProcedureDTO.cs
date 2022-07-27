
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationStoredProcedureDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("NotificationId")]
        public int NotificationId { get; set; }

        [JsonProperty("StoredProcedureName")]
        public string StoredProcedureName { get; set; }

    }
}

