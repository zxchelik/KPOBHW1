using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Core.Models;
using Core.Enums;
using Core.Factories;
using Core.Interfaces;
using Core.Services.MemoryStorage;

namespace Tests
{
    public class MemoryStoreAndContextTests
    {
        // Тест для базовых операций в in-memory хранилищах
        [Fact]
        public void AddAndGetById_WorksCorrectly()
        {
            var store = new MemoryBankAccountStore();
            var account = EntityFactory.CreateBankAccount(1, "Test Account", 100m);
            store.Add(account);
            var retrieved = store.GetById(1);
            Assert.NotNull(retrieved);
            Assert.Equal(account.Name, retrieved.Name);
        }

        [Fact]
        public void Update_WorksCorrectly()
        {
            var store = new MemoryCategoryStore();
            var category = EntityFactory.CreateCategory(1, "Food", CategoryType.Expense);
            store.Add(category);
            category.Name = "Groceries";
            store.Update(category);
            var retrieved = store.GetById(1);
            Assert.Equal("Groceries", retrieved.Name);
        }

        [Fact]
        public void DeleteById_WorksCorrectly()
        {
            var store = new MemoryOperationStore();
            var op = EntityFactory.CreateOperation(1, 
                EntityFactory.CreateBankAccount(1, "Acc", 100m),
                EntityFactory.CreateCategory(1, "Income", CategoryType.Income),
                50m, DateTime.Now, "Test");
            store.Add(op);
            store.DeleteById(1);
            Assert.Null(store.GetById(1));
        }

        [Fact]
        public void RewriteAllData_WorksCorrectly()
        {
            var store = new MemoryBankAccountStore();
            var accounts = new List<BankAccount>
            {
                EntityFactory.CreateBankAccount(1, "A1", 100m),
                EntityFactory.CreateBankAccount(2, "A2", 200m)
            };
            store.RewriteAllData(accounts);
            var all = store.GetAll().ToList();
            Assert.Equal(2, all.Count);
            Assert.Contains(all, a => a.Id == 1);
            Assert.Contains(all, a => a.Id == 2);
        }

        [Fact]
        public void GetNextId_ReturnsCorrectValue()
        {
            // Используем вспомогательную реализацию IStore<T> для теста GetNextId
            var store = new TestStore<BankAccount>(Enumerable.Empty<BankAccount>());
            Assert.Equal(1, store.GetNextId());
            store.Add(EntityFactory.CreateBankAccount(1, "Acc1", 100m));
            Assert.Equal(2, store.GetNextId());
            store.Add(EntityFactory.CreateBankAccount(3, "Acc3", 100m)); // сейчас id: 1 и 3
            Assert.Equal(4, store.GetNextId());
        }

        // Вспомогательный класс для тестирования IStore<T>
        public class TestStore<T> : IStore<T> where T : IEntity
        {
            private readonly Dictionary<int, T> _items = new();

            // 1. Параметрless-конструктор, если где-то нужен вызов без параметров
            public TestStore() 
            {
            }

            // 2. Конструктор, принимающий коллекцию, 
            //    чтобы сразу наполнить хранилище
            public TestStore(IEnumerable<T> items)
            {
                foreach (var item in items)
                {
                    _items[item.Id] = item;
                }
            }

            public void Add(T item) => _items[item.Id] = item;

            public void Update(T item) => _items[item.Id] = item;

            public void DeleteById(int id) => _items.Remove(id);

            public T? GetById(int id) => _items.TryGetValue(id, out var entity) ? entity : default;

            public IEnumerable<T> GetAll() => _items.Values;

            public int GetNextId() => _items.Keys.Any() ? _items.Keys.Max() + 1 : 1;

            public void RewriteAllData(IEnumerable<T> items)
            {
                _items.Clear();
                foreach (var item in items)
                {
                    _items[item.Id] = item;
                }
            }
        }

        [Fact]
        public void FinanceContextFactory_ReturnsContextWithCorrectData()
        {
            var bankAccounts = new List<BankAccount>
            {
                EntityFactory.CreateBankAccount(1, "Account1", 500m)
            };
            var categories = new List<Category>
            {
                EntityFactory.CreateCategory(1, "Salary", CategoryType.Income)
            };
            var operations = new List<Operation>
            {
                EntityFactory.CreateOperation(1, bankAccounts[0], categories[0], 500m, DateTime.Now)
            };

            var bankStore = new TestStore<BankAccount>(bankAccounts);
            var catStore = new TestStore<Category>(categories);
            var opStore = new TestStore<Operation>(operations);

            var context = Core.Factories.FinanceContextFactory.CreateFinanceContext(bankStore, catStore, opStore);

            Assert.Equal(bankAccounts, context.BankAccounts);
            Assert.Equal(categories, context.Categories);
            Assert.Equal(operations, context.Operations);
        }
    }
}
