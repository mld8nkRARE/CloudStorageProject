using Client.Services;
using Client.Services.Interfaces;
using Client.ViewModels;
using Client.ViewModels.Auth;
using Client.ViewModels.Files;
using Client.Views;
using Client.Views.Auth;
using Client.Views.File;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Windows;

namespace Client
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            // стартуем MainWindow
            var mainWindow = Services.GetRequiredService<MainWindow>();
            var mainVm = Services.GetRequiredService<MainViewModel>();

            // текущий view — LoginViewModel
            //mainVm.CurrentView = Services.GetRequiredService<LoginViewModel>();
            mainWindow.DataContext = mainVm;

            mainWindow.Show();
            base.OnStartup(e);
        }


        private void ConfigureServices(IServiceCollection services)
        {
            // HTTP
            //services.AddHttpClient();
            /*services.AddHttpClient<IFileService, FileService>(c =>
            {
                c.BaseAddress = new Uri("http://localhost:5187");
            });
            services.AddHttpClient<IAuthService, AuthService>(c =>
            {
                c.BaseAddress = new Uri("http://localhost:5187");
            });
            */
            services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:5187");
                return client;
            });
            // Services
            //services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IFolderService, FolderService>();
            services.AddSingleton<INavigationService, NavigationService>();
            

            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddSingleton<FileListViewModel>();
            services.AddSingleton<ProfileViewModel>();
            services.AddSingleton<FileItemViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginView>();
            services.AddSingleton<RegisterView>();
            services.AddSingleton<ProfileView>();
            services.AddSingleton<FileListView>();
        }
    }
}

