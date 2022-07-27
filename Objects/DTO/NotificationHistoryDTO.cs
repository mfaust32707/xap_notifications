using System;
using Newtonsoft.Json;

namespace NotificationsService.Objects.DTO
{
    public class NotificationHistoryDTO
    {
       [JsonProperty("Id")]
        public int? Id { get; set; }

        [JsonProperty("NotificationId")]
        public int? NotificationId { get; set; }

        [JsonProperty("RecipientUserId")]
        public int? RecipientUserId { get; set; }

        [JsonProperty("Subject")]
        public string Subject { get; set; }

        [JsonProperty("NotificationMethodId")]
        public int? NotificationMethodId { get; set; }

        [JsonProperty("MethodDescription")]
        public string MethodDescription { get; set; }

        [JsonProperty("ApplicationId")]
        public int? ApplicationId { get; set; }

        [JsonProperty("ReadFlag")]
        public bool ReadFlag { get; set; }

        [JsonProperty("MessageBody")]
        public string MessageBody { get; set; }

        [JsonProperty("ExpiredFlag")]
        public bool ExpiredFlag { get; set; }

        private DateTime expirationDateTime { get; set; }

        [JsonProperty("ExpirationDateTime")]
        public DateTime ExpirationDateTime

        {
            get
            {
                return this.expirationDateTime;
            }
            set
            {
                if (value < (DateTime)System.Data.SqlTypes.SqlDateTime.MinValue ||
                    value > (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue)
                {
                    expirationDateTime = (DateTime)System.Data.SqlTypes.SqlDateTime.MaxValue;
                }
                else
                {
                    expirationDateTime = value;
                }
            }
            
          }
        [JsonProperty("SentDateTime")]
        public DateTime SentDateTime { get; set; }

    }
}
