using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ChainLinkUtils.Utils.Objects.DTO;
using ChainLinkUtils.Utils.Objects.Responses;

namespace NotificationsService.Objects
{
    public class LoginResponse : Response
    {
        [JsonPropertyName("User")]
        public UsersDTO User { get; set; }

        [JsonPropertyName("MenuTree")]
        public List<UserMenuTreeDTO> MenuTree { get; set; }

        [JsonPropertyName("Favorites")]
        public List<FunctionsDTO> Favorites { get; set; }

        [JsonPropertyName("Token")]
        public string Token { get; set; }

        [JsonPropertyName("RefreshToken")]
        public RefreshTokenDTO RefreshToken { get; set; }
    }

    public class UserMenuTreeDTO
    {
        [JsonPropertyName("FunctionId")]
        public int FunctionId { get; set; }

        [JsonPropertyName("FunctionName")]
        public string FunctionName { get; set; }

        [JsonPropertyName("FunctionDescription")]
        public string FunctionDescription { get; set; }

        [JsonPropertyName("DisplayOrder")]
        public int DisplayOrder { get; set; }

        [JsonPropertyName("URL")]
        public string URL { get; set; }

        [JsonPropertyName("ActiveFlag")]
        public bool ActiveFlag { get; set; }

        [JsonPropertyName("Glyphicon")]
        public GlyphiconsDTO Glyphicon { get; set; }

        [JsonPropertyName("ChildFunctions")]
        public List<UserMenuTreeDTO> ChildFunctions { get; set; }
    }

}
