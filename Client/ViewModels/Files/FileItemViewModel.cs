using Client.Models.File;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Files
{
    public partial class FileItemViewModel : ObservableObject
    {
        private readonly IFileService _fileService;

        public FileItemViewModel(FileDto dto, IFileService fileService)
        {
            _fileService = fileService;
            Id = dto.Id;
            FileName = dto.FileName;
            Size = dto.Size;
            UploadedAt = dto.UploadedAt;
        }
        public event Action? FileDeleted;
        public Guid Id { get; }
        public string FileName { get; }
        public long Size { get; }
        public DateTime UploadedAt { get; }

        [RelayCommand]
        private async Task DownloadAsync()
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = FileName
            };
            if (dialog.ShowDialog() == true)
            {
                var success = await _fileService.DownloadFileAsync(Id, dialog.FileName);
                if (success) MessageBox.Show("Скачивание завершено");
                else MessageBox.Show("Ошибка при скачивании");
            }
        }

        [RelayCommand]
        private async Task DeleteAsync()
        {
            if (MessageBox.Show($"Удалить {FileName}?", "Подтвердите", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
            var ok = await _fileService.DeleteFileAsync(Id);
            if (ok)
            {
                MessageBox.Show("Файл удалён");
                FileDeleted?.Invoke();
            }

            else MessageBox.Show("Ошибка при удалении");
        }
    }
}
