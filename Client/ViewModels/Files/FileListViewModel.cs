using Client.Models.File;
using Client.Services;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Files
{
    public partial class FileListViewModel : ObservableObject
    {
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;
        private readonly INavigationService _navigation;

        public ObservableCollection<FileItemViewModel> Files { get; } = new ObservableCollection<FileItemViewModel>();

        public FileListViewModel(IFileService fileService, INavigationService navigation)
        {
            _fileService = fileService;
            _navigation = navigation;
            _ = LoadFilesAsync();
        }

        [RelayCommand]
        public async Task LoadFilesAsync()
        {
            var list = await _fileService.GetFilesAsync();
            Files.Clear();
            foreach (var f in list)
            {
                var itemVm = new FileItemViewModel(f, _fileService);
                itemVm.FileDeleted += async () => await LoadFilesAsync();
                Files.Add(itemVm);
            }
        }

        [RelayCommand]
        public async Task UploadAsync()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            if (dlg.ShowDialog() != true) return;

            var success = await _fileService.UploadFileAsync(dlg.FileName);
            if (!success) MessageBox.Show("Ошибка при загрузке");
            else
            {
                MessageBox.Show("Файл загружен");
                await LoadFilesAsync();
            }
        }

        [RelayCommand]
        private void Logout()
        {
            _authService.ClearToken();
            Files.Clear();
            var loginVm = App.Services.GetRequiredService<Client.ViewModels.Auth.LoginViewModel>();
            _navigation.NavigateTo(loginVm);
        }

        [RelayCommand]
        private void OpenProfile()
        {
            var profileVm = App.Services.GetRequiredService<ProfileViewModel>();
            _navigation.NavigateTo(profileVm);
            //profileVm.LoadProfileCommand.Execute(null);
        }
    }
}



