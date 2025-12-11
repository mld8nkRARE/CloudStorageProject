using Client.Models.User;
using Client.Services.Interfaces;
using Client.ViewModels.Auth;
using Client.Views.Dialogs;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client.ViewModels
{
    public partial class ProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public ICommand BackCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        // Временная замена [ObservableProperty]
        private UserProfileDto _userProfile;
        public UserProfileDto UserProfile
        {
            get => _userProfile;
            set => SetProperty(ref _userProfile, value);
        }

        private Models.User.StorageInfoDto _storageInfo;
        public Models.User.StorageInfoDto StorageInfo
        {
            get => _storageInfo;
            set => SetProperty(ref _storageInfo, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string UsedStorageFormatted => FormatBytes(StorageInfo?.UsedBytes ?? 0);
        public string MaxStorageFormatted => FormatBytes(StorageInfo?.MaxBytes ?? 100 * 1024 * 1024);
        public double StoragePercentage => StorageInfo?.Percentage ?? 0;

        public ProfileViewModel(
            INavigationService navigation,
            IAuthService authService,
            IUserService userService)
        {
            _navigation = navigation;
            _authService = authService;
            _userService = userService;

            BackCommand = new RelayCommand(GoBack);
            RefreshCommand = new AsyncRelayCommand(LoadProfileData);
            ChangePasswordCommand = new AsyncRelayCommand(ChangePassword);
            LogoutCommand = new RelayCommand(Logout);

            LoadProfileData();
        }

        private async Task LoadProfileData()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                UserProfile = await _userService.GetProfileAsync();
                StorageInfo = await _userService.GetStorageInfoAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GoBack()
        {
            var fileListVm = App.Services.GetRequiredService<Files.FileListViewModel>();
            _navigation.NavigateTo(fileListVm);
        }

        private async Task ChangePassword()
        {
            try
            {
                var dialog = new InputDialogWindow();
                dialog.Owner = Application.Current.MainWindow; // Делаем окно модальным к главному

                if (dialog.ShowDialog() == true)
                {
                    var dto = new ChangePasswordDto
                    {
                        CurrentPassword = dialog.CurrentPassword,
                        NewPassword = dialog.NewPassword,
                        ConfirmPassword = dialog.ConfirmPassword
                    };

                    var result = await _userService.ChangePasswordAsync(dto);

                    if (result)
                    {
                        MessageBox.Show(
                            "✅ Пароль успешно изменен",
                            "Успех",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show(
                            "❌ Неверный текущий пароль",
                            "Ошибка",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при смене пароля: {ex.Message}";
            }
        }

        private void Logout()
        {
            _authService.ClearToken();
            var loginVm = App.Services.GetRequiredService<LoginViewModel>();
            _navigation.NavigateTo(loginVm);
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "Б", "КБ", "МБ", "ГБ", "ТБ" };
            int counter = 0;
            decimal number = bytes;

            while (Math.Round(number / 1024) >= 1 && counter < suffixes.Length - 1)
            {
                number /= 1024;
                counter++;
            }

            return $"{number:n1} {suffixes[counter]}";
        }
    }
}


