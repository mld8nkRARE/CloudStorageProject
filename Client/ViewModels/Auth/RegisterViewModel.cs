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
    public partial class RegisterViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigation;

        private string _email = "";
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string _nickname = "";
        public string Nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public RegisterViewModel(IAuthService authService, INavigationService navigation)
        {
            _authService = authService;
            _navigation = navigation;
        }

        [RelayCommand]
        public async Task Register()
        {
            var result = await _authService.RegisterAsync(
                Nickname, Email, Password
            );

            if (!result)
            {
                MessageBox.Show("Ошибка регистрации");
                return;
            }

            MessageBox.Show("Успешная регистрация!");

            // После регистрации — переход на FileListView
            var fileListVm = App.Services.GetRequiredService<FileListViewModel>();
            _navigation.NavigateTo(fileListVm);
            await fileListVm.LoadFilesAsync();
        }

        [RelayCommand]
        public void BackToLogin()
        {
            var loginVm = App.Services.GetRequiredService<LoginViewModel>();
            _navigation.NavigateTo(loginVm);
        }
    }
}



