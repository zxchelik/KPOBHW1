using System;
using Microsoft.Extensions.DependencyInjection;
using App;
using Core.Interfaces;
using Core.Models;
using Core.Services.MemoryStorage;
using Core.Services;
using Core.Enums;

namespace App
{
    internal abstract class Program
    {
        static void Main(string[] args)
        {
            // Настройка DI-контейнера
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Запуск приложения через разрешённый DI-контейнером Menu
            var menu = serviceProvider.GetRequiredService<Menu>();
            menu.Show();
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            // Регистрируем in-memory хранилища как синглтоны
            services.AddSingleton<IStore<BankAccount>, MemoryBankAccountStore>();
            services.AddSingleton<IStore<Category>, MemoryCategoryStore>();
            services.AddSingleton<IStore<Operation>, MemoryOperationStore>();

            // Регистрируем сервис аналитики
            services.AddSingleton<IAnalyticsService, AnalyticsService>();

            // Регистрируем фасады, если потребуется (их можно оставить статическими или также зарегистрировать)

            // Регистрируем сам класс Menu
            services.AddSingleton<Menu>();
        }
    }
}