using Client.Models.Auth;
using Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public string AccessToken { get; private set; } = string.Empty;
        public Guid CurrentUserId { get; private set; } = Guid.Empty;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (!response.IsSuccessStatusCode)
                return null;

            var body = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<LoginResponse>(body);

            if (result != null)
                SetToken(result.Token, result.UserId);

            return result;
        }

        public async Task<bool> RegisterAsync(string nickname, string email, string password)
        {
            var request = new
            {
                Nickname = nickname,
                Email = email,
                Password = password
            };

            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/register", content);

            return response.IsSuccessStatusCode;
        }

        public void SetToken(string token, Guid userId)
        {
            AccessToken = token;
            CurrentUserId = userId;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        public void ClearToken()
        {
            AccessToken = string.Empty;
            CurrentUserId = Guid.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
