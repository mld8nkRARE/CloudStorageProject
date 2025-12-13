using Client.Models.File;
using Client.Models.Folders;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Files
{
    public partial class FileListViewModel : ObservableObject
    {
        private readonly IFileService _fileService;
        private readonly IAuthService _authService;
        private readonly INavigationService _navigation;

        private Guid? _currentFolderId = null;
        private string _currentFolderName = string.Empty;

        public class BreadcrumbItem
        {
            public string Name { get; set; } = string.Empty;
            public Guid? FolderId { get; set; }
        }

        private ObservableCollection<BreadcrumbItem> _breadcrumbs = new ObservableCollection<BreadcrumbItem>();
        public ObservableCollection<BreadcrumbItem> Breadcrumbs
        {
            get => _breadcrumbs;
            set => SetProperty(ref _breadcrumbs, value);
        }

        private string _currentPath = "Мои файлы";
        public string CurrentPath
        {
            get => _currentPath;
            set => SetProperty(ref _currentPath, value);
        }
        
        public ObservableCollection<FileItemViewModel> Files { get; } = new ObservableCollection<FileItemViewModel>();

        private async Task UpdateBreadcrumbsAsync(Guid? folderId)
        {
            try
            {
                var newBreadcrumbs = new ObservableCollection<BreadcrumbItem>();

                // Всегда добавляем корень
                newBreadcrumbs.Add(new BreadcrumbItem { Name = "Мои файлы", FolderId = null });

                if (folderId.HasValue)
                {
                    var folderService = App.Services.GetRequiredService<IFolderService>();
                    var userId = _authService.GetUserId();

                    // Получаем полный путь папки
                    var path = await folderService.GetFolderPathAsync(folderId.Value, userId);

                    foreach (var item in path)
                    {
                        newBreadcrumbs.Add(new BreadcrumbItem
                        {
                            Name = item.Name,
                            FolderId = item.Id
                        });
                    }
                }

                Breadcrumbs = newBreadcrumbs;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обновления хлебных крошек: {ex.Message}");
                // Если ошибка, просто показываем корень
                if (Breadcrumbs.Count == 0)
                {
                    Breadcrumbs.Add(new BreadcrumbItem { Name = "Мои файлы", FolderId = null });
                }
            }
        }

        [RelayCommand]
        public async Task NavigateToBreadcrumbAsync(BreadcrumbItem breadcrumb)
        {
            if (breadcrumb == null) return;

            try
            {
                if (breadcrumb.FolderId == null)
                {
                    // Переход в корень
                    await GoBackAsync();
                }
                else
                {
                    // Переход к конкретной папке в пути
                    await OpenFolderByIdAsync(breadcrumb.FolderId.Value, breadcrumb.Name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка навигации: {ex.Message}");
            }
        }

        // НОВОЕ: Добавляем коллекцию папок
        public ObservableCollection<FolderItem> Folders { get; } = new ObservableCollection<FolderItem>();
        //
        public FileListViewModel(IFileService fileService, INavigationService navigation)
        {
            _fileService = fileService;
            _navigation = navigation;
            _authService = App.Services.GetRequiredService<IAuthService>();

            Breadcrumbs.Add(new BreadcrumbItem { Name = "Мои файлы", FolderId = null });

            // ЗАГРУЖАЕМ ТОЛЬКО ФАЙЛЫ, папки загрузим позже
            _ = LoadFilesAsync();

        }

        //метод для загрузки папок
        public async Task LoadFoldersAfterLoginAsync()
        {
            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();

                // Проверяем, что userId валидный
                if (userId == Guid.Empty)
                {
                    Console.WriteLine("Ошибка: userId не установлен");
                    return;
                }

                // Всегда загружаем корневые папки при инициализации
                var content = await folderService.GetContentAsync(null, userId);

                if (content?.Items != null)
                {
                    Folders.Clear();
                    foreach (var folder in content.Items.Where(i => i.IsFolder))
                    {
                        Folders.Add(folder);
                    }

                    Console.WriteLine($"Загружено папок: {Folders.Count}");
                }
                else
                {
                    Console.WriteLine("Content или Items равны null");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки папок: {ex.Message}");
            }
        }

        [RelayCommand]
        //public async Task LoadFilesAsync()
        //{
        //    var list = await _fileService.GetFilesAsync();
        //    Files.Clear();
        //    foreach (var f in list)
        //    {
        //        var itemVm = new FileItemViewModel(f, _fileService);
        //        itemVm.FileDeleted += async () => await LoadFilesAsync();
        //        Files.Add(itemVm);
        //    }
        //}
        public async Task LoadFilesAsync()
        {
            try
            {
                var list = await _fileService.GetFilesAsync();
                Files.Clear();
                foreach (var f in list)
                {
                    var itemVm = new FileItemViewModel(f, _fileService);
                    itemVm.FileDeleted += async () => await LoadFilesAsync();
                    Files.Add(itemVm);
                }

                CurrentPath = "Мои файлы";
                _currentFolderId = null;
                _currentFolderName = string.Empty;

                // Обновляем хлебные крошки
                await UpdateBreadcrumbsAsync(null);

                // Загружаем корневые папки
                await LoadFoldersAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки файлов: {ex.Message}");
            }
        }

        // НОВОЕ: Загрузка папок
        private async Task LoadFoldersAsync()
        {
            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();
                var content = await folderService.GetContentAsync(_currentFolderId, userId);

                if (content?.Items != null)
                {
                    Folders.Clear();
                    foreach (var folder in content.Items.Where(i => i.IsFolder))
                    {
                        Folders.Add(folder);
                    }
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки загрузки папок
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

                // Обновляем файлы в зависимости от текущей папки
                if (_currentFolderId == null)
                    await LoadFilesAsync();
                else
                    await LoadFolderContentAsync(_currentFolderId);
            }
        }


        // НОВОЕ: Команда для создания папки
        [RelayCommand]
        public async Task CreateFolderAsync()
        {
            try
            {
                var dialog = new Views.Dialogs.CreateFolderDialog();
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    var folderService = App.Services.GetRequiredService<IFolderService>();
                    var userId = _authService.GetUserId();

                    var request = new CreateFolderRequest
                    {
                        Name = dialog.FolderName,
                        ParentFolderId = _currentFolderId //корневая папка
                    };

                    var folderId = await folderService.CreateAsync(request, userId);

                    if (folderId != Guid.Empty)
                    {
                        MessageBox.Show("Папка создана");
                        await LoadFoldersAsync();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при создании папки");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task RenameFolderAsync(FolderItem folder)
        {
            if (folder == null) return;

            try
            {
                var dialog = new Views.Dialogs.RenameFolderDialog(folder.Name);
                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true && !string.IsNullOrWhiteSpace(dialog.NewFolderName))
                {
                    var folderService = App.Services.GetRequiredService<IFolderService>();
                    var userId = _authService.GetUserId();

                    var success = await folderService.RenameAsync(folder.Id, dialog.NewFolderName, userId);

                    if (success)
                    {
                        MessageBox.Show("Папка переименована");

                        // Обновляем список
                        if (_currentFolderId == null)
                            await LoadFoldersAsync();
                        else
                            await LoadFolderContentAsync(_currentFolderId);

                        // Обновляем хлебные крошки если переименовали текущую папку
                        if (_currentFolderId == folder.Id)
                        {
                            await UpdateBreadcrumbsAsync(_currentFolderId);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при переименовании");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        [RelayCommand]
        public async Task DeleteFolderAsync(FolderItem folder)
        {
            if (folder == null) return;

            var result = MessageBox.Show(
                $"Удалить папку '{folder.Name}' со всем содержимым?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes) return;

            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();

                var success = await folderService.DeleteAsync(folder.Id, userId);

                if (success)
                {
                    MessageBox.Show("Папка удалена");

                    // Обновляем список
                    if (_currentFolderId == null)
                        await LoadFoldersAsync();
                    else
                        await LoadFolderContentAsync(_currentFolderId);
                }
                else
                {
                    MessageBox.Show("Ошибка при удалении папки");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        // НОВОЕ: Команда для перемещения файла в папку
        public async Task MoveFileToFolderAsync(Guid fileId, Guid? folderId)
        {
            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();
                var success = await folderService.MoveFileAsync(fileId, folderId, userId);

                if (success)
                {
                    MessageBox.Show("Файл перемещен");

                    // Обновляем файлы в зависимости от текущей папки
                    if (_currentFolderId == null)
                        await LoadFilesAsync();
                    else
                        await LoadFolderContentAsync(_currentFolderId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка перемещения: {ex.Message}");
            }
        }


        [RelayCommand]
        private void Logout()
        {
            _authService.ClearToken();
            Files.Clear();
            //Folders.Clear();
            Breadcrumbs.Clear();
            var loginVm = App.Services.GetRequiredService<Client.ViewModels.Auth.LoginViewModel>();
            _navigation.NavigateTo(loginVm);
        }

        [RelayCommand]
        private void OpenProfile()
        {
            var profileVm = App.Services.GetRequiredService<ProfileViewModel>();
            _navigation.NavigateTo(profileVm);
        }


        [RelayCommand]
        public async Task OpenFolderAsync(FolderItem folder)
        {
            if (folder == null) return;
            await OpenFolderByIdAsync(folder.Id, folder.Name);
        }

        private async Task OpenFolderByIdAsync(Guid folderId, string folderName)
        {
            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();

                var content = await folderService.GetContentAsync(folderId, userId);

                if (content?.Items != null)
                {
                    _currentFolderId = folderId;
                    _currentFolderName = folderName;
                    CurrentPath = $"Мои файлы / {folderName}";

                    // Обновляем хлебные крошки
                    await UpdateBreadcrumbsAsync(folderId);

                    // Очищаем и заполняем файлы
                    Files.Clear();
                    foreach (var item in content.Items.Where(i => !i.IsFolder))
                    {
                        var fileDto = new FileDto
                        {
                            Id = item.Id,
                            FileName = item.Name,
                            Size = item.Size ?? 0,
                            UploadedAt = item.CreatedAt
                        };

                        var fileItem = new FileItemViewModel(fileDto, _fileService);
                        fileItem.FileDeleted += async () => await LoadFolderContentAsync(folderId);
                        Files.Add(fileItem);
                    }

                    // Обновляем подпапки
                    Folders.Clear();
                    foreach (var subFolder in content.Items.Where(i => i.IsFolder))
                    {
                        Folders.Add(subFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки папки: {ex.Message}");
            }
        }

        // Вспомогательный метод для загрузки содержимого папки
        private async Task LoadFolderContentAsync(Guid? folderId)
        {
            try
            {
                var folderService = App.Services.GetRequiredService<IFolderService>();
                var userId = _authService.GetUserId();
                var content = await folderService.GetContentAsync(folderId, userId);

                if (content?.Items != null)
                {
                    Files.Clear();
                    foreach (var item in content.Items.Where(i => !i.IsFolder))
                    {
                        var fileDto = new FileDto
                        {
                            Id = item.Id,
                            FileName = item.Name,
                            Size = item.Size ?? 0,
                            UploadedAt = item.CreatedAt
                        };

                        var fileItem = new FileItemViewModel(fileDto, _fileService);
                        fileItem.FileDeleted += async () => await LoadFolderContentAsync(folderId);
                        Files.Add(fileItem);
                    }

                    Folders.Clear();
                    foreach (var subFolder in content.Items.Where(i => i.IsFolder))
                    {
                        Folders.Add(subFolder);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }


        [RelayCommand]
        public async Task GoBackAsync()
        {
            if (_currentFolderId == null)
            {
                // Уже в корне, просто обновляем
                await LoadFilesAsync();
                //await LoadFoldersAsync();
                return;
            }

            try
            {
                // Возвращаемся к корню
                await LoadFilesAsync();
                //await LoadFoldersAsync();
                CurrentPath = "Мои файлы";
                _currentFolderId = null;
                await UpdateBreadcrumbsAsync(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}



