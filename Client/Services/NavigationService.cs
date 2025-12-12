using Client.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

//namespace Client.Services
//{
//    public class NavigationService : INavigationService
//    {
//        private readonly IServiceProvider _serviceProvider;

//        public NavigationService(IServiceProvider serviceProvider)
//        {
//            _serviceProvider = serviceProvider;
//        }

//        public void NavigateTo(object viewModel)
//        {
//            var mainVm = _serviceProvider.GetRequiredService<MainViewModel>();
//            mainVm.CurrentView = viewModel;
//        }
//    }
//}
namespace Client.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private MainViewModel _mainVm;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo(object viewModel)
        {
            if (_mainVm == null)
            {
                _mainVm = _serviceProvider.GetRequiredService<MainViewModel>();
            }

            _mainVm.CurrentView = viewModel;
        }
    }
}