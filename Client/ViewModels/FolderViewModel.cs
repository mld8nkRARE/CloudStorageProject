using Client.Models.Folders;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Client.ViewModels
{
    public partial class FolderViewModel : ObservableObject
    {
        private readonly IFolderService _folderService;

        public ObservableCollection<FolderItem> Items { get; set; } = new ObservableCollection<FolderItem>();
        [ObservableProperty]
        private FolderItem? _selectedItem;

        [ObservableProperty]
        private Guid? _currentFolderId;

        [ObservableProperty]
        private string _currentFolderName = "Корень";


        public ICommand CreateFolderCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand OpenFolderCommand { get; }

        public FolderViewModel(IFolderService folderService)
        {
            _folderService = folderService;
            CreateFolderCommand = new AsyncRelayCommand(CreateFolder);
            DeleteItemCommand = new AsyncRelayCommand(DeleteItem, () => SelectedItem != null);
            OpenFolderCommand = new AsyncRelayCommand<FolderItem>(OpenFolder);

            //Task.Run(async () => await LoadFolder(null));
            _ = LoadFolder(null);
        }

        public async Task LoadFolder(Guid? folderId)
        {
            var content = await _folderService.GetContentAsync(folderId);
            if (content == null) return;

            CurrentFolderName = content.FolderName;
            OnPropertyChanged(nameof(CurrentFolderName));

            await App.Current.Dispatcher.InvokeAsync(() =>
            {
                Items.Clear();

                foreach (var item in content.Items)
                {
                    Items.Add(item);
                }
            });
            OnPropertyChanged(nameof(CurrentFolderName));
        }

        private async Task CreateFolder()
        {
            var name = Interaction.InputBox("Введите имя папки:", "Создать папку", "Новая папка");
            if (string.IsNullOrWhiteSpace(name)) return;

            await _folderService.CreateAsync(new Client.Models.Folders.CreateFolderRequest
            {
                Name = name,
                ParentFolderId = _currentFolderId
            });

            await LoadFolder(_currentFolderId);
        }

        private async Task DeleteItem()
        {
            if (SelectedItem == null) return;

            if (SelectedItem.IsFolder)
                await _folderService.DeleteAsync(SelectedItem.Id);
            else
                await _folderService.MoveFileAsync(SelectedItem.Id, null); // заглушка, можно заменить на DeleteFile

            await LoadFolder(_currentFolderId);
        }

        private async Task OpenFolder(FolderItem item)
        {
            if (item == null || !item.IsFolder) return;
            await LoadFolder(item.Id);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

