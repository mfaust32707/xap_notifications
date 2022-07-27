
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationMethodDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }
    
    }
}
