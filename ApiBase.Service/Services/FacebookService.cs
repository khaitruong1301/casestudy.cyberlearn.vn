using Newtonsoft.Json;
using ApiBase.Service.Helpers;
using ApiBase.Service.Socials.Facebook;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ApiBase.Service.Services
{
    public interface IFacebookService
    {
        Task<FacebookUserData> GetUserFacebookAsync(string facebookToken);
    }

    public class FacebookService : IFacebookService
    {
        private readonly HttpClient _httpClient;
        private readonly IFacebookSettings _facebookSettings;

        public FacebookService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FacebookUserData> GetUserFacebookAsync(string facebookToken)
        {
            FacebookUserAccessTokenData validateToken = await ValidateToken(facebookToken);
            if (!validateToken.IsValid)
                return null;
            return await GetUserInfo(facebookToken);
        }

        // Validate token
        private async Task<FacebookUserAccessTokenData> ValidateToken(string facebookToken)
        {
            // 1.generate an app access token
            string baseUri = "https://graph.facebook.com/oauth/access_token?client_id";
            var tokenResponse = await _httpClient.GetStringAsync($"{baseUri}={_facebookSettings.AppId }&client_secret={_facebookSettings.AppSecret}&grant_type=client_credentials");

            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(tokenResponse);

            // 2. validate the user access token
            var userAccessTokenValidationResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={facebookToken}&access_token={appAccessToken.AccessToken}");

            return JsonConvert.DeserializeObject<FacebookUserAccessTokenData>(userAccessTokenValidationResponse);
        }

        private async Task<FacebookUserData> GetUserInfo(string facebookToken)
        {
            // 3. we've got a valid token so we can request user data from fb
            var userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name,picture&access_token={facebookToken}");

            return JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);
        }
    }
}