
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class HeaderMethodRefDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("HeaderId")]
        public int HeaderId { get; set; }

        [JsonProperty("MethodId")]
        public int MethodId { get; set; }
    }
}
