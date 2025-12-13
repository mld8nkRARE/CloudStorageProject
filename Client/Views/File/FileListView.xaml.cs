using System.Windows;
using System.Windows.Controls;
using Client.Models.Folders;
using Client.ViewModels.Files;

namespace Client.Views.File
{
    public partial class FileListView : UserControl
    {
        public FileListView()
        {
            InitializeComponent();
        }

        // Этот метод будет вызываться когда начинаем перетаскивать файл
        private void FileItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is FileItemViewModel fileVm)
            {
                // Подготавливаем данные для перетаскивания
                var data = new DataObject();
                data.SetData("FileId", fileVm.Id);
                data.SetData("FileName", fileVm.FileName);

                // Начинаем перетаскивание
                DragDrop.DoDragDrop(element, data, DragDropEffects.Move);
            }
        }

        // Обработчик клика по папке
        private void FolderItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is FolderItem folder)
            {
                if (DataContext is FileListViewModel vm)
                {
                    vm.OpenFolderCommand.Execute(folder);
                }
            }
        }

        // Контекстное меню: Переименовать папку
        private void OnRenameFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.Tag is FolderItem folder &&
                DataContext is FileListViewModel vm)
            {
                vm.RenameFolderCommand.Execute(folder);
            }
        }

        // Контекстное меню: Удалить папку
        private void OnDeleteFolderClick(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem &&
                menuItem.Tag is FolderItem folder &&
                DataContext is FileListViewModel vm)
            {
                vm.DeleteFolderCommand.Execute(folder);
            }
        }

        // Когда файл "заносим" над папкой
        private void FolderItem_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                // Подсвечиваем папку
                border.Background = System.Windows.Media.Brushes.LightSkyBlue;
                border.BorderBrush = System.Windows.Media.Brushes.Blue;
            }
        }

        // Когда убираем файл с папки
        private void FolderItem_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                // Возвращаем обычный цвет
                border.Background = System.Windows.Media.Brushes.LightCyan;
                border.BorderBrush = System.Windows.Media.Brushes.LightBlue;
            }
        }

        // Когда "бросаем" файл на папку
        private async void FolderItem_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border && border.DataContext is FolderItem folder)
            {
                // Возвращаем цвет
                border.Background = System.Windows.Media.Brushes.LightCyan;
                border.BorderBrush = System.Windows.Media.Brushes.LightBlue;

                // Получаем ID файла из данных перетаскивания
                if (e.Data.GetDataPresent("FileId") &&
                    e.Data.GetData("FileId") is Guid fileId)
                {
                    // Запрашиваем подтверждение
                    if (e.Data.GetData("FileName") is string fileName)
                    {
                        var result = MessageBox.Show(
                            $"Переместить файл '{fileName}' в папку '{folder.Name}'?",
                            "Подтверждение",
                            MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            // Вызываем команду перемещения
                            if (DataContext is FileListViewModel vm)
                            {
                                // Вызываем метод напрямую, а не через команду
                                await vm.MoveFileToFolderAsync(fileId, folder.Id);
                            }
                        }
                    }
                }
            }
        }

        // Подсветка при перетаскивании в корень
        private void RootArea_DragEnter(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = System.Windows.Media.Brushes.LightGreen;
                border.BorderBrush = System.Windows.Media.Brushes.Green;
            }
        }

        private void RootArea_DragLeave(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                border.Background = System.Windows.Media.Brushes.White;
                border.BorderBrush = System.Windows.Media.Brushes.Gray;
            }
        }

        // Бросание файла в корень
        private async void RootArea_Drop(object sender, DragEventArgs e)
        {
            if (sender is Border border)
            {
                // Возвращаем цвет
                border.Background = System.Windows.Media.Brushes.White;
                border.BorderBrush = System.Windows.Media.Brushes.Gray;

                // Получаем ID файла
                if (e.Data.GetDataPresent("FileId") &&
                    e.Data.GetData("FileId") is Guid fileId)
                {
                    if (e.Data.GetData("FileName") is string fileName)
                    {
                        var result = MessageBox.Show(
                            $"Переместить файл '{fileName}' в корень?",
                            "Подтверждение",
                            MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.Yes)
                        {
                            if (DataContext is FileListViewModel vm)
                            {
                                // Перемещаем в корень (null = корень)
                                await vm.MoveFileToFolderAsync(fileId, null);
                            }
                        }
                    }
                }
            }
        }
    }
}
