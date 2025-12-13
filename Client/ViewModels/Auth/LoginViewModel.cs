using Client.Services;
using Client.Services.Interfaces;
using Client.ViewModels.Files;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels.Auth
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigation;

        private string _email = "";
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public LoginViewModel(IAuthService authService, INavigationService navigation)
        {
            _authService = authService;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task Login()
        {
            _authService.ClearToken();
            var result = await _authService.LoginAsync(new Models.Auth.LoginRequest
            {
                Email = Email,
                Password = Password
            });

            if (result == null)
            {
                MessageBox.Show("Неверный email или пароль");
                return;
            }
            _authService.SetToken(result.Token, result.UserId);
            MessageBox.Show("Успешный вход!");

            // Создаем FileListViewModel
            var fileListVm = App.Services.GetRequiredService<FileListViewModel>();

            // ЗАГРУЖАЕМ ПАПКИ ПОСЛЕ ЛОГИНА
            await fileListVm.LoadFoldersAfterLoginAsync();

            // Переходим
            _navigation.NavigateTo(fileListVm);
            await fileListVm.LoadFilesAsync();
        }

        [RelayCommand]
        public void OpenRegister()
        {
            var registerVm = App.Services.GetRequiredService<RegisterViewModel>();
            _navigation.NavigateTo(registerVm);
        }
    }
}


