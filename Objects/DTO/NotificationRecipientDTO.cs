using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationRecipientDTO
    {
        [JsonProperty("UserId")]
        public int UserId { get; set; }

        [JsonProperty("ApplicationId")]
        public int ApplicationId { get; set; }

        [JsonProperty("LastName")]
        public string LastName { get; set; }

        [JsonProperty("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("EmailAddress")]
        public string EmailAddress { get; set; }

    }
}
