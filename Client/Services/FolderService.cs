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

            var response = await _httpClient.PostAsync($"folders/create?userId={userId}", content);
            response.EnsureSuccessStatusCode();

            var resultJson = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Guid>(resultJson);
        }

        public async Task<FolderContent> GetContentAsync(Guid? folderId, Guid userId)
        {
            string url = folderId.HasValue
                ? $"folders/content/{folderId}?userId={userId}"
                : $"folders/content?userId={userId}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<FolderContent>(json);
        }

        //public async Task<FolderTree> GetTreeAsync(Guid userId, Guid? rootFolderId = null)
        //{
        //    string url = rootFolderId.HasValue
        //        ? $"folders/tree/{rootFolderId}?userId={userId}"
        //        : $"folders/tree?userId={userId}";

        //    var response = await _httpClient.GetAsync(url);
        //    if (!response.IsSuccessStatusCode) return null;

        //    var json = await response.Content.ReadAsStringAsync();
        //    return JsonConvert.DeserializeObject<FolderTree>(json);
        //}


        public async Task<bool> DeleteAsync(Guid folderId, Guid userId)
        {
            var response = await _httpClient.DeleteAsync($"folders/delete/{folderId}?userId={userId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MoveFolderAsync(Guid folderId, Guid? targetFolderId, Guid userId)
        {
            var request = new { FolderId = folderId, TargetFolderId = targetFolderId };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"folders/move-folder?userId={userId}", content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> MoveFileAsync(Guid fileId, Guid? targetFolderId, Guid userId)
        {
            var request = new { FileId = fileId, TargetFolderId = targetFolderId };
            var json = JsonConvert.SerializeObject(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"folders/move-file?userId={userId}", content);
            return response.IsSuccessStatusCode;
        }
    }

}

