using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Client.Views.Dialogs
{
    public partial class RenameFolderDialog : Window
    {
        public string NewFolderName { get; private set; } = string.Empty;
        public string CurrentName { get; set; }

        public RenameFolderDialog(string currentName)
        {
            InitializeComponent();
            CurrentName = currentName;
            DataContext = this;
            Loaded += (s, e) => FolderNameTextBox.Focus();
            FolderNameTextBox.SelectAll();
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            var name = FolderNameTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Введите название папки");
                return;
            }

            if (name.Length > 50)
            {
                ShowError("Название слишком длинное (макс. 50 символов)");
                return;
            }

            NewFolderName = name;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}
