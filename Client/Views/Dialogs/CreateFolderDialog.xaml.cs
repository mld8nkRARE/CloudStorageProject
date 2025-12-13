using System.Windows;

namespace Client.Views.Dialogs
{
    public partial class CreateFolderDialog : Window
    {
        public string FolderName { get; private set; } = string.Empty;

        public CreateFolderDialog()
        {
            InitializeComponent();
            Loaded += (s, e) => FolderNameTextBox.Focus();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
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

            FolderName = name;
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