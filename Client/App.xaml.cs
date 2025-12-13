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
using System.Text.Json;
using System.IO;

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
            //services.AddHttpClient<IFileService, FileService>(c =>
            //{
            //    c.BaseAddress = new Uri("http://localhost:5000");
            //});

            //services.AddHttpClient<IAuthService, AuthService>(c =>
            //{
            //    c.BaseAddress = new Uri("http://localhost:5000");
            //});

            //services.AddHttpClient<IUserService, UserService>(c =>
            //{
            //    c.BaseAddress = new Uri("http://localhost:5000");
            //});

            //services.AddHttpClient<IFolderService, FolderService>(c =>
            //{
            //    c.BaseAddress = new Uri("http://localhost:5000");
            //});
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var baseUrl = File.ReadAllText(configPath).Trim(' ', '\r', '\n', '"');

            services.AddSingleton<HttpClient>(sp =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(baseUrl);
                return client;
            });
            // Services
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IFileService, FileService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IFolderService, FolderService>();
            services.AddSingleton<INavigationService, NavigationService>();

            
                
            // ViewModels
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<RegisterViewModel>();
            services.AddTransient<FileListViewModel>();
            services.AddTransient<ProfileViewModel>();
            services.AddSingleton<FileItemViewModel>();
            services.AddSingleton<FolderViewModel>();

            // Views
            services.AddSingleton<MainWindow>();
            services.AddSingleton<LoginView>();
            services.AddSingleton<RegisterView>();
            services.AddSingleton<ProfileView>();
            services.AddSingleton<FileListView>();
            services.AddSingleton<FolderView>();
        }
    }
}

