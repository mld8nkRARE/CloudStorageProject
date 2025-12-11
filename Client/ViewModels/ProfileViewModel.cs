using Client.Services;
using Client.Services.Interfaces;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Input;

namespace Client.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly INavigationService _navigation;
        private readonly IAuthService _authService;

        public ICommand BackCommand { get; }

        public ProfileViewModel(INavigationService navigation, IAuthService authService)
        {
            _navigation = navigation;
            _authService = authService;

            BackCommand = new RelayCommand(() =>
            {
                // ✅ Получаем FileListViewModel через DI
                var fileListVm = App.Services.GetRequiredService<Client.ViewModels.Files.FileListViewModel>();
                _navigation.NavigateTo(fileListVm);
            });
        }
    }
}


