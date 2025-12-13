using Client.Models.Auth;
using Client.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Client.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        public Guid CurrentUserId { get; private set; } = Guid.Empty;
        public string CurrentToken { get; private set; } = string.Empty;

        public string AccessToken { get; private set; } = string.Empty;
        

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/auth/login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var loginResponse = JsonConvert.DeserializeObject<LoginResponse>(responseJson);

                if (loginResponse != null)
                {
                    // Сохраняем всё просто в поля
                    CurrentToken = loginResponse.Token;
                    CurrentUserId = loginResponse.UserId;

                    // Добавляем токен в заголовки
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", CurrentToken);
                }

                return loginResponse;
            }

            return null;
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

        public void SetToken(string token)
        {
            AccessToken = token;
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Не парсим токен, просто устанавливаем флаг
            // userId будет получен другим способом
        }

        public Guid GetUserId()
        {
            return CurrentUserId; // Просто возвращаем из поля
        }

        //public void ClearToken()
        //{
        //    AccessToken = null;
        //    _userId = Guid.Empty;
        //    _httpClient.DefaultRequestHeaders.Authorization = null;
        //}

        public void ClearToken()
        {
            CurrentToken = string.Empty;
            CurrentUserId = Guid.Empty;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

}
