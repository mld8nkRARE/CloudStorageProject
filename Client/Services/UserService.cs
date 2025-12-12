using Client.Models.User;
using Client.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<UserProfileDto> GetProfileAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/profile");

                if (!response.IsSuccessStatusCode)
                    return null;

                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<UserProfileDto>(body);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<StorageInfoDto> GetStorageInfoAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users/storage");

                if (!response.IsSuccessStatusCode)
                    return new StorageInfoDto { MaxBytes = 100 * 1024 * 1024 };

                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<StorageInfoDto>(body);
            }
            catch (Exception)
            {
                return new StorageInfoDto { MaxBytes = 100 * 1024 * 1024 };
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/users/change-password", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
