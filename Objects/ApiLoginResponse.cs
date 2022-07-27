using System.Collections.Generic;
using ChainLinkUtils.Utils.Objects.DTO;
using ChainLinkUtils.Utils.Objects.Responses;
using Newtonsoft.Json;

namespace UserAuthentication.Objects.Responses
{
    public class ApiLoginResponse : Response
    {
        [JsonProperty("ApiName")]
        public string ApiName { get; set; }

        [JsonProperty("Permissions")]
        public List<PermissionsDTO> Permissions { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("RefreshToken")]
        public RefreshTokenDTO RefreshToken { get; set; }
    }
}

