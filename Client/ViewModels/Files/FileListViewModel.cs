using Client.Models.File;
using Client.Models.Folders;
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
        private readonly IFolderService _folderService;
        private readonly INavigationService _navigation;
        public ObservableCollection<FolderItem> Items { get; } = new();
        //public ObservableCollection<FileItemViewModel> Files { get; } = new ObservableCollection<FileItemViewModel>();
        [ObservableProperty]
        private FolderItem? selectedItem;
        [ObservableProperty]
        private string currentFolderName = "Корень";

        private Guid? _currentFolderId;

        public IAsyncRelayCommand CreateFolderCommand { get; }
        public IAsyncRelayCommand DeleteCommand { get; }
        public IAsyncRelayCommand<FolderItem> ItemDoubleClickCommand { get; }
        public IAsyncRelayCommand GoBackCommand { get; }
        public FileListViewModel(IFileService fileService, IFolderService folderService, INavigationService navigation)
        {
            _fileService = fileService;
            _folderService = folderService;
            _navigation = navigation;
            CreateFolderCommand = new AsyncRelayCommand(CreateAsync);
            DeleteCommand = new AsyncRelayCommand(DeleteSelected, () => SelectedItem != null);
            ItemDoubleClickCommand = new AsyncRelayCommand<FolderItem>(OnItemDoubleClick);
            GoBackCommand = new AsyncRelayCommand(GoBack, () => _currentFolderId != null);
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
            var loginVm = App.Services.GetRequiredService<Client.ViewModels.Auth.LoginViewModel>();
            _navigation.NavigateTo(loginVm);
        }

        [RelayCommand]
        private void OpenProfile()
        {
            var profileVm = App.Services.GetRequiredService<ProfileViewModel>();
            _navigation.NavigateTo(profileVm);
        }
    }
}



