using Newtonsoft.Json;
using System.Globalization;

namespace NitesInstitution.Models
{
    public class AccessTokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public string ExpiresIn { get; set; }
        [JsonProperty(".expires")]
        public string ExpiresAt { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonIgnore]
        public string ObtainedAt { get; set; }

        public bool IsValid()
        {
            DateTime obtainedAt = DateTime.ParseExact(this.ObtainedAt, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime now = DateTime.Now;
            if (obtainedAt.AddSeconds(Convert.ToInt32(this.ExpiresIn)) > now.AddMinutes(1))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }

    public class TokenValidation
    {
        [JsonProperty("valid")]
        public bool Valid { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("user")]
        public string User { get; set; }
    }

}
