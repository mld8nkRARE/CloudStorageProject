using Client.Models.Folders;
using Client.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services
{
    public class FolderService : IFolderService
    {
        private readonly HttpClient _httpClient;

        public FolderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Guid> CreateAsync(CreateFolderRequest dto, Guid userId)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"api/folders", content);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Guid>(resultJson);
        }

        public async Task<FolderContent> GetContentAsync(Guid? folderId, Guid userId)
        {
            string url = folderId.HasValue
                ? $"api/folders/{folderId}"
                : $"api/folders";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FolderContent>(json);
        }

        public async Task<bool> DeleteAsync(Guid folderId, Guid userId)
        {
            var response = await _httpClient.DeleteAsync($"api/folders/{folderId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId, Guid userId)
        {
            var request = new { TargetFolderId = targetFolderId };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"api/folders/{folderId}/move", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId, Guid userId)
        {
            var request = new { TargetFolderId = targetFolderId };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"api/folders/file/{fileId}/move", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<FolderPathDto>> GetFolderPathAsync(Guid folderId, Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/folders/{folderId}/path");
            if (!response.IsSuccessStatusCode) return new List<FolderPathDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FolderPathDto>>(json);
        }

        public async Task<bool> RenameAsync(Guid folderId, string newName, Guid userId)
        {
            var request = new { NewName = newName };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PatchAsync($"api/folders/{folderId}/rename", content);
            return response.IsSuccessStatusCode;
        }
    }

}

