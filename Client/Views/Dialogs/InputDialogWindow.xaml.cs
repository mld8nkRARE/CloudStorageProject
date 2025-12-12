using System.Windows;
using System.Windows.Controls;

namespace Client.Views.Dialogs
{
    public partial class InputDialogWindow : Window
    {
        public string CurrentPassword { get; private set; } = string.Empty;
        public string NewPassword { get; private set; } = string.Empty;
        public string ConfirmPassword { get; private set; } = string.Empty;

        public InputDialogWindow()
        {
            InitializeComponent();

            // Подписываемся на изменение паролей
            CurrentPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            NewPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            ConfirmPasswordBox.PasswordChanged += PasswordBox_PasswordChanged;

            Loaded += (s, e) => CurrentPasswordBox.Focus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Получаем значения
            CurrentPassword = CurrentPasswordBox.Password;
            NewPassword = NewPasswordBox.Password;
            ConfirmPassword = ConfirmPasswordBox.Password;

            // Валидация
            if (string.IsNullOrWhiteSpace(CurrentPassword))
            {
                ShowError("Введите текущий пароль");
                CurrentPasswordBox.Focus();
                return;
            }

            if (NewPassword.Length < 6)
            {
                ShowError("Новый пароль должен быть минимум 6 символов");
                NewPasswordBox.Focus();
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                ShowError("Новый пароль и подтверждение не совпадают");
                ConfirmPasswordBox.Focus();
                return;
            }

            // Всё ок
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

        // Скрыть ошибку при изменении текста
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;
        }
    }
}
