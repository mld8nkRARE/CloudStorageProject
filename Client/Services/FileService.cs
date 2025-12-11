using Client.Models.File;
using Client.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Client.Services
{
    public class FileService : IFileService
    {
        private readonly HttpClient _httpClient;

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<FileDto>> GetFilesAsync()
        {
            var response = await _httpClient.GetAsync("/api/files/list");

            if (!response.IsSuccessStatusCode)
                return new List<FileDto>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<FileDto>>(json);
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"/api/files/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadFileAsync(string path)
        {
            using (var content = new MultipartFormDataContent())
            {
                var fileBytes = File.ReadAllBytes(path);
                var fileContent = new ByteArrayContent(fileBytes);

                content.Add(fileContent, "file", Path.GetFileName(path));

                var response = await _httpClient.PostAsync("/api/files/upload", content);
                return response.IsSuccessStatusCode;
            }
        }

        public async Task<bool> DownloadFileAsync(Guid id, string savePath)
        {
            var response = await _httpClient.GetAsync($"/api/files/download/{id}");

            if (!response.IsSuccessStatusCode)
                return false;

            var stream = await response.Content.ReadAsStreamAsync();

            // Сохраняем файл на диск
            using (var fileStream = File.Create(savePath))
            {
                await stream.CopyToAsync(fileStream);
            }

            return true;
        }
    }
}
