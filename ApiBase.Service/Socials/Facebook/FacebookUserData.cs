using Newtonsoft.Json;

namespace ApiBase.Service.Socials.Facebook
{
    public class FacebookUserData
    {
        [JsonProperty("id")]
        public string FacebookId { set; get; }

        [JsonProperty("email")]
        public string Email { set; get; }

        [JsonProperty("first_name")]
        public string FirstName { set; get; }

        [JsonProperty("last_name")]
        public string LastName { set; get; }

        [JsonProperty("picture")]
        public string Avatar { set; get; }
    }
}