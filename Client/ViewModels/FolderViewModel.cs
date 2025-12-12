using Client.Models.Folders;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.VisualBasic;


namespace Client.ViewModels
{
    public class FolderViewModel : INotifyPropertyChanged
    {
        private readonly IFolderService _folderService;
        private readonly Guid _userId;

        public ObservableCollection<FolderItem> Items { get; set; } = new ObservableCollection<FolderItem>();
        private FolderItem _selectedItem;
        public FolderItem SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(nameof(SelectedItem)); }
        }

        private Guid? _currentFolderId;
        public string CurrentFolderName { get; set; } = "Корень";

        public ICommand CreateFolderCommand { get; }
        public ICommand DeleteItemCommand { get; }
        public ICommand OpenFolderCommand { get; }

        public FolderViewModel(IFolderService folderService, Guid userId)
        {
            _folderService = folderService;
            _userId = userId;

            CreateFolderCommand = new AsyncRelayCommand(CreateFolder);
            DeleteItemCommand = new AsyncRelayCommand(DeleteItem, () => SelectedItem != null);
            OpenFolderCommand = new AsyncRelayCommand<FolderItem>(OpenFolder);

            Task.Run(async () => await LoadFolder(null));
        }

        public async Task LoadFolder(Guid? folderId)
        {
            var content = await _folderService.GetContentAsync(folderId, _userId);
            if (content == null) return;

            _currentFolderId = content.FolderId;
            CurrentFolderName = content.FolderName;
            OnPropertyChanged(nameof(CurrentFolderName));

            App.Current.Dispatcher.Invoke(() =>
            {
                Items.Clear();
                foreach (var i in content.Items)
                {
                    Items.Add(new FolderItem
                    {
                        Id = i.Id,
                        Name = i.Name,
                        Type = i.Type,
                        Size = i.Size,
                        CreatedAt = i.CreatedAt
                    });
                }
            });
        }

        private async Task CreateFolder()
        {
            string name = Interaction.InputBox("Введите имя папки:", "Создать папку", "Новая папка");
            if (string.IsNullOrWhiteSpace(name)) return;

            await _folderService.CreateAsync(new Client.Models.Folders.CreateFolderRequest
            {
                Name = name,
                ParentFolderId = _currentFolderId
            }, _userId);

            await LoadFolder(_currentFolderId);
        }

        private async Task DeleteItem()
        {
            if (SelectedItem == null) return;

            if (SelectedItem.IsFolder)
                await _folderService.DeleteAsync(SelectedItem.Id, _userId);
            else
                await _folderService.MoveFileAsync(SelectedItem.Id, null, _userId); // заглушка, можно заменить на DeleteFile

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

