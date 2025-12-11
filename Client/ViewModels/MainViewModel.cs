using Client.Services;
using Client.Services.Interfaces;
using Client.ViewModels.Auth;
using Client.ViewModels.Files;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        //public INavigationService Navigation { get; }

        private string _title = "Облачноe хранилище";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private object _currentView;
        public object CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

        // Можно хранить ссылки на VM для удобства
        public FileListViewModel FileListVm { get; }
        public LoginViewModel LoginVm { get; }
        public RegisterViewModel RegisterVm { get; }

        public MainViewModel(IAuthService authService,
                             LoginViewModel loginVm,
                             RegisterViewModel registerVm,
                             FileListViewModel fileListVm)
        {
            _authService = authService;
            LoginVm = loginVm;
            RegisterVm = registerVm;
            FileListVm = fileListVm;

            // стартовый экран — логин
            CurrentView = LoginVm;
        }

        public void Logout()
        {
            _authService.ClearToken();
            MessageBox.Show("Вы вышли из аккаунта");

            // Возвращаем на LoginView через глобальный сервис
            var navigation = App.Services.GetRequiredService<INavigationService>();
            navigation.NavigateTo(LoginVm);
        }

        public async Task InitializeAsync()
        {
            if (FileListVm != null)
                await FileListVm.LoadFilesAsync();
        }
    }
}






