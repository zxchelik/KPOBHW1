using System;
using System.Linq;
using Core.Models;
using Core.Factories;
using Core.Facades;
using Core.Enums;
using Core.Interfaces;

namespace App
{
    public class Menu
    {
        private readonly IStore<BankAccount> bankAccountStore;
        private readonly IStore<Category> categoryStore;
        private readonly IStore<Operation> operationStore;
        private readonly IAnalyticsService analyticsService;

        // Все зависимости внедряются через конструктор
        public Menu(
            IStore<BankAccount> bankAccountStore,
            IStore<Category> categoryStore,
            IStore<Operation> operationStore,
            IAnalyticsService analyticsService)
        {
            this.bankAccountStore = bankAccountStore;
            this.categoryStore = categoryStore;
            this.operationStore = operationStore;
            this.analyticsService = analyticsService;
        }

        public void Show()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Финансовый учет ===");
                Console.WriteLine("1. Управление счетами");
                Console.WriteLine("2. Управление категориями");
                Console.WriteLine("3. Управление операциями");
                Console.WriteLine("4. Аналитика");
                Console.WriteLine("5. Импорт данных");
                Console.WriteLine("6. Экспорт данных");
                Console.WriteLine("7. Выход");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ManageBankAccounts();
                        break;
                    case "2":
                        ManageCategories();
                        break;
                    case "3":
                        ManageOperations();
                        break;
                    case "4":
                        Analytics();
                        break;
                    case "5":
                        ImportData();
                        break;
                    case "6":
                        ExportData();
                        break;
                    case "7":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        #region Управление счетами

        private void ManageBankAccounts()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление счетами ===");
                Console.WriteLine("1. Список счетов");
                Console.WriteLine("2. Добавить счет");
                Console.WriteLine("3. Редактировать счет");
                Console.WriteLine("4. Удалить счет");
                Console.WriteLine("5. Назад");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListBankAccounts();
                        break;
                    case "2":
                        AddBankAccount();
                        break;
                    case "3":
                        EditBankAccount();
                        break;
                    case "4":
                        DeleteBankAccount();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListBankAccounts()
        {
            Console.Clear();
            Console.WriteLine("=== Список счетов ===");
            foreach (var account in bankAccountStore.GetAll())
            {
                Console.WriteLine($"ID: {account.Id}, Название: {account.Name}, Баланс: {account.Balance}");
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void AddBankAccount()
        {
            Console.Clear();
            Console.WriteLine("=== Добавить счет ===");
            Console.Write("Введите название счета: ");
            var name = Console.ReadLine();
            Console.Write("Введите начальный баланс: ");
            decimal balance;
            while (!decimal.TryParse(Console.ReadLine(), out balance))
            {
                Console.Write("Неверное значение. Введите баланс ещё раз: ");
            }
            int newId = bankAccountStore.GetAll().Any() ? bankAccountStore.GetAll().Max(a => a.Id) + 1 : 1;
            var account = EntityFactory.CreateBankAccount(newId, name, balance);
            bankAccountStore.Add(account);
            Console.WriteLine("Счет добавлен. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void EditBankAccount()
        {
            Console.Clear();
            Console.WriteLine("=== Редактировать счет ===");
            ListBankAccounts();
            Console.Write("Введите ID счета для редактирования: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID ещё раз: ");
            }
            var account = bankAccountStore.GetById(id);
            if (account == null)
            {
                Console.WriteLine("Счет не найден. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            Console.Write("Введите новое название счета (оставьте пустым, чтобы не менять): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                account.Name = newName;
            }
            Console.Write("Введите новый баланс (оставьте пустым, чтобы не менять): ");
            var balanceInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(balanceInput))
            {
                if (decimal.TryParse(balanceInput, out decimal newBalance))
                {
                    account.Balance = newBalance;
                }
                else
                {
                    Console.WriteLine("Неверное значение баланса. Изменения не сохранены.");
                }
            }
            bankAccountStore.Update(account);
            Console.WriteLine("Счет обновлён. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void DeleteBankAccount()
        {
            Console.Clear();
            Console.WriteLine("=== Удалить счет ===");
            ListBankAccounts();
            Console.Write("Введите ID счета для удаления: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID ещё раз: ");
            }
            bankAccountStore.DeleteById(id);
            Console.WriteLine("Счет удалён (если существовал). Нажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion

        #region Управление категориями

        private void ManageCategories()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление категориями ===");
                Console.WriteLine("1. Список категорий");
                Console.WriteLine("2. Добавить категорию");
                Console.WriteLine("3. Редактировать категорию");
                Console.WriteLine("4. Удалить категорию");
                Console.WriteLine("5. Назад");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListCategories();
                        break;
                    case "2":
                        AddCategory();
                        break;
                    case "3":
                        EditCategory();
                        break;
                    case "4":
                        DeleteCategory();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListCategories()
        {
            Console.Clear();
            Console.WriteLine("=== Список категорий ===");
            foreach (var category in categoryStore.GetAll())
            {
                Console.WriteLine($"ID: {category.Id}, Название: {category.Name}, Тип: {category.Type}");
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void AddCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Добавить категорию ===");
            Console.Write("Введите название категории: ");
            var name = Console.ReadLine();
            Console.Write("Введите тип категории (Income/Expense): ");
            string typeStr = Console.ReadLine();
            if (!Enum.TryParse<CategoryType>(typeStr, true, out var type))
            {
                Console.WriteLine("Неверный тип категории. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            int newId = categoryStore.GetAll().Any() ? categoryStore.GetAll().Max(c => c.Id) + 1 : 1;
            var category = EntityFactory.CreateCategory(newId, name, type);
            categoryStore.Add(category);
            Console.WriteLine("Категория добавлена. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void EditCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Редактировать категорию ===");
            ListCategories();
            Console.Write("Введите ID категории для редактирования: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID ещё раз: ");
            }
            var category = categoryStore.GetById(id);
            if (category == null)
            {
                Console.WriteLine("Категория не найдена. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            Console.Write("Введите новое название категории (оставьте пустым, чтобы не менять): ");
            var newName = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newName))
            {
                category.Name = newName;
            }
            Console.Write("Введите новый тип категории (Income/Expense, оставьте пустым, чтобы не менять): ");
            var newTypeStr = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newTypeStr))
            {
                if (Enum.TryParse<CategoryType>(newTypeStr, true, out var newType))
                {
                    category.Type = newType;
                }
                else
                {
                    Console.WriteLine("Неверный тип. Изменения не сохранены.");
                }
            }
            categoryStore.Update(category);
            Console.WriteLine("Категория обновлена. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void DeleteCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Удалить категорию ===");
            ListCategories();
            Console.Write("Введите ID категории для удаления: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID ещё раз: ");
            }
            categoryStore.DeleteById(id);
            Console.WriteLine("Категория удалена (если существовала). Нажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion

        #region Управление операциями

        private void ManageOperations()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управление операциями ===");
                Console.WriteLine("1. Список операций");
                Console.WriteLine("2. Добавить операцию");
                Console.WriteLine("3. Редактировать операцию");
                Console.WriteLine("4. Удалить операцию");
                Console.WriteLine("5. Назад");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ListOperations();
                        break;
                    case "2":
                        AddOperation();
                        break;
                    case "3":
                        EditOperation();
                        break;
                    case "4":
                        DeleteOperation();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ListOperations()
        {
            Console.Clear();
            Console.WriteLine("=== Список операций ===");
            foreach (var op in operationStore.GetAll())
            {
                Console.WriteLine($"ID: {op.Id}, Тип: {op.Type}, Счет ID: {op.BankAccountId}, " +
                    $"Категория ID: {op.CategoryId}, Сумма: {op.Amount}, Дата: {op.Date.ToShortDateString()}, " +
                    $"Описание: {op.Description}");
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void AddOperation()
        {
            Console.Clear();
            Console.WriteLine("=== Добавить операцию ===");
            Console.Write("Введите ID счета: ");
            int accountId;
            while (!int.TryParse(Console.ReadLine(), out accountId))
            {
                Console.Write("Неверное значение. Введите ID счета: ");
            }
            var account = bankAccountStore.GetById(accountId);
            if (account == null)
            {
                Console.WriteLine("Счет не найден. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            Console.Write("Введите ID категории: ");
            int categoryId;
            while (!int.TryParse(Console.ReadLine(), out categoryId))
            {
                Console.Write("Неверное значение. Введите ID категории: ");
            }
            var category = categoryStore.GetById(categoryId);
            if (category == null)
            {
                Console.WriteLine("Категория не найдена. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            Console.Write("Введите сумму операции: ");
            decimal amount;
            while (!decimal.TryParse(Console.ReadLine(), out amount))
            {
                Console.Write("Неверное значение. Введите сумму: ");
            }
            Console.Write("Введите дату операции (например, 2025-03-20): ");
            DateTime date;
            while (!DateTime.TryParse(Console.ReadLine(), out date))
            {
                Console.Write("Неверный формат даты. Введите дату ещё раз: ");
            }
            Console.Write("Введите описание операции (необязательно): ");
            var description = Console.ReadLine();
            int newId = operationStore.GetAll().Any() ? operationStore.GetAll().Max(o => o.Id) + 1 : 1;
            try
            {
                var operation = EntityFactory.CreateOperation(newId, account, category, amount, date, description);
                operationStore.Add(operation);
                Console.WriteLine("Операция добавлена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при создании операции: " + ex.Message);
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void EditOperation()
        {
            Console.Clear();
            Console.WriteLine("=== Редактировать операцию ===");
            ListOperations();
            Console.Write("Введите ID операции для редактирования: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID операции: ");
            }
            var op = operationStore.GetAll().FirstOrDefault(o => o.Id == id);
            if (op == null)
            {
                Console.WriteLine("Операция не найдена. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            Console.Write("Введите новый ID счета (оставьте пустым, чтобы не менять): ");
            var accountInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(accountInput) && int.TryParse(accountInput, out int newAccountId))
            {
                var newAccount = bankAccountStore.GetById(newAccountId);
                if (newAccount != null)
                {
                    op.BankAccountId = newAccountId;
                }
                else
                {
                    Console.WriteLine("Счет не найден. Изменения не применены.");
                }
            }
            Console.Write("Введите новый ID категории (оставьте пустым, чтобы не менять): ");
            var categoryInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(categoryInput) && int.TryParse(categoryInput, out int newCategoryId))
            {
                var newCategory = categoryStore.GetById(newCategoryId);
                if (newCategory != null)
                {
                    op.CategoryId = newCategoryId;
                    op.Type = newCategory.Type;
                }
                else
                {
                    Console.WriteLine("Категория не найдена. Изменения не применены.");
                }
            }
            Console.Write("Введите новую сумму операции (оставьте пустым, чтобы не менять): ");
            var amountInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(amountInput) && decimal.TryParse(amountInput, out decimal newAmount))
            {
                op.Amount = newAmount;
            }
            Console.Write("Введите новую дату операции (например, 2025-03-20, оставьте пустым): ");
            var dateInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParse(dateInput, out DateTime newDate))
            {
                op.Date = newDate;
            }
            Console.Write("Введите новое описание (оставьте пустым, чтобы не менять): ");
            var desc = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(desc))
            {
                op.Description = desc;
            }
            operationStore.Update(op);
            Console.WriteLine("Операция обновлена. Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void DeleteOperation()
        {
            Console.Clear();
            Console.WriteLine("=== Удалить операцию ===");
            ListOperations();
            Console.Write("Введите ID операции для удаления: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Неверное значение. Введите ID операции: ");
            }
            operationStore.DeleteById(id);
            Console.WriteLine("Операция удалена (если существовала). Нажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion

        #region Аналитика

        private void Analytics()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Аналитика ===");
                Console.WriteLine("1. Разница доходов и расходов за период");
                Console.WriteLine("2. Группировка операций по категориям");
                Console.WriteLine("3. Назад");
                Console.Write("Выберите опцию: ");
                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ShowBalanceDifference();
                        break;
                    case "2":
                        ShowGroupedByCategory();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор. Нажмите любую клавишу...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void ShowBalanceDifference()
        {
            Console.Clear();
            Console.WriteLine("=== Разница доходов и расходов ===");
            Console.Write("Введите начальную дату (yyyy-MM-dd): ");
            DateTime startDate;
            while (!DateTime.TryParse(Console.ReadLine(), out startDate))
            {
                Console.Write("Неверный формат. Введите дату ещё раз: ");
            }
            Console.Write("Введите конечную дату (yyyy-MM-dd): ");
            DateTime endDate;
            while (!DateTime.TryParse(Console.ReadLine(), out endDate))
            {
                Console.Write("Неверный формат. Введите дату ещё раз: ");
            }
            var balanceDiff = analyticsService.GetBalanceDifference(operationStore.GetAll(), startDate, endDate);
            Console.WriteLine($"Разница между доходами и расходами: {balanceDiff}");
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void ShowGroupedByCategory()
        {
            Console.Clear();
            Console.WriteLine("=== Группировка операций по категориям ===");
            var grouped = analyticsService.GetGroupedByCategory(operationStore.GetAll());
            foreach (var group in grouped)
            {
                var cat = categoryStore.GetById(group.Key);
                string catName = cat != null ? cat.Name : "Неизвестная категория";
                Console.WriteLine($"Категория: {catName} (ID: {group.Key}), Сумма: {group.Value}");
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion

        #region Импорт/Экспорт данных

        private void ImportData()
        {
            Console.Clear();
            Console.WriteLine("=== Импорт данных ===");
            Console.Write("Введите путь к файлу: ");
            var filePath = Console.ReadLine();
            Console.Write("Введите формат файла (Json, Yaml, Excel): ");
            var formatStr = Console.ReadLine();
            if (!Enum.TryParse<FileFormat>(formatStr, true, out var format))
            {
                Console.WriteLine("Неверный формат. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            try
            {
                var context = ImportFacade.ImportAll(format, filePath);
                bankAccountStore.RewriteAllData(context.BankAccounts);
                categoryStore.RewriteAllData(context.Categories);
                operationStore.RewriteAllData(context.Operations);
                Console.WriteLine("Данные успешно импортированы.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка импорта: " + ex.Message);
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        private void ExportData()
        {
            Console.Clear();
            Console.WriteLine("=== Экспорт данных ===");
            Console.Write("Введите путь к файлу для экспорта: ");
            var filePath = Console.ReadLine();
            Console.Write("Введите формат файла (Json, Yaml, Excel): ");
            var formatStr = Console.ReadLine();
            if (!Enum.TryParse<FileFormat>(formatStr, true, out var format))
            {
                Console.WriteLine("Неверный формат. Нажмите любую клавишу...");
                Console.ReadKey();
                return;
            }
            try
            {
                var context = Core.Factories.FinanceContextFactory.CreateFinanceContext(
                    bankAccountStore,
                    categoryStore,
                    operationStore
                );
                ExportFacade.Export(context, format, filePath);
                Console.WriteLine("Данные успешно экспортированы.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка экспорта: " + ex.Message);
            }
            Console.WriteLine("Нажмите любую клавишу...");
            Console.ReadKey();
        }

        #endregion
    }
}
