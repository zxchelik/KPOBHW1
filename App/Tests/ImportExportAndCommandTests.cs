using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Xunit;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using Core.Models;
using Core.Enums;
using Core.Factories;
using Core.Facades;
using Core.Commands;
using Core.Interfaces;

namespace Tests
{
    public class ImportExportAndCommandTests : IDisposable
    {
        private readonly string tempFilePath;

        public ImportExportAndCommandTests()
        {
            tempFilePath = Path.GetTempFileName();
        }

        public void Dispose()
        {
            if (File.Exists(tempFilePath))
                File.Delete(tempFilePath);
        }

        private FinanceContext CreateTestContext()
        {
            var bankAccounts = new List<BankAccount>
            {
                EntityFactory.CreateBankAccount(1, "Acc1", 500m)
            };
            var categories = new List<Category>
            {
                EntityFactory.CreateCategory(1, "Salary", CategoryType.Income)
            };
            var operations = new List<Operation>
            {
                EntityFactory.CreateOperation(1, bankAccounts[0], categories[0], 500m, DateTime.Now, "Test")
            };
            return new FinanceContext(bankAccounts, categories, operations);
        }

        [Theory]
        [InlineData(FileFormat.Json)]
        [InlineData(FileFormat.Yaml)]
        // [InlineData(FileFormat.Excel)] // Если настроен EPPlus
        public void ExportThenImport_ReturnsEquivalentContext(FileFormat format)
        {
            var originalContext = CreateTestContext();
            // Export
            ExportFacade.Export(originalContext, format, tempFilePath);
            // Import
            var importedContext = ImportFacade.ImportAll(format, tempFilePath);

            Assert.Equal(originalContext.BankAccounts.Count(), importedContext.BankAccounts.Count());
            Assert.Equal(originalContext.Categories.Count(), importedContext.Categories.Count());
            Assert.Equal(originalContext.Operations.Count(), importedContext.Operations.Count());
        }

        [Fact]
        public void JsonExporterAndImporter_ExportsAndImportsCorrectly()
        {
            var context = CreateTestContext();
            var exporter = new Core.WorkWithFiles.Export.JsonExporter();
            exporter.Export(context, tempFilePath);
            var importer = new Core.WorkWithFiles.Import.JsonImporter();
            var importedContext = importer.Import(tempFilePath);
            Assert.Single(importedContext.BankAccounts);
        }

        [Fact]
        public void YamlExporterAndImporter_ExportsAndImportsCorrectly()
        {
            var context = CreateTestContext();
            var exporter = new Core.WorkWithFiles.Export.YamlExporter();
            exporter.Export(context, tempFilePath);
            var importer = new Core.WorkWithFiles.Import.YamlImporter();
            var importedContext = importer.Import(tempFilePath);
            Assert.Single(importedContext.Categories);
        }

        [Fact]
        public void JsonImporter_InvalidContent_ThrowsException()
        {
            File.WriteAllText(tempFilePath, "invalid json content");
            var importer = new Core.WorkWithFiles.Import.JsonImporter();
            Assert.ThrowsAny<Exception>(() => importer.Import(tempFilePath));
        }

        [Fact]
        public void YamlImporter_InvalidContent_ThrowsException()
        {
            File.WriteAllText(tempFilePath, "invalid: yaml: content:");
            var importer = new Core.WorkWithFiles.Import.YamlImporter();
            Assert.ThrowsAny<Exception>(() => importer.Import(tempFilePath));
        }

        [Fact]
        public void GetBalanceDifferenceCommand_ReturnsCorrectResult()
        {
            var analyticsService = new Core.Services.AnalyticsService();
            var operations = new List<Operation>
            {
                EntityFactory.CreateOperation(1,
                    EntityFactory.CreateBankAccount(1, "Acc", 100m),
                    EntityFactory.CreateCategory(1, "Income", CategoryType.Income),
                    100m, DateTime.Now, null),
                EntityFactory.CreateOperation(2,
                    EntityFactory.CreateBankAccount(1, "Acc", 100m),
                    EntityFactory.CreateCategory(2, "Expense", CategoryType.Expense),
                    30m, DateTime.Now, null)
            };
            DateTime startDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now.AddDays(1);

            var command = new GetBalanceDifferenceCommand(analyticsService, operations, startDate, endDate);
            var result = command.Execute();
            Assert.Equal(70m, result);
        }

        [Fact]
        public void GetGroupedByCategoryCommand_ReturnsCorrectGrouping()
        {
            var analyticsService = new Core.Services.AnalyticsService();
            var operations = new List<Operation>
            {
                EntityFactory.CreateOperation(1,
                    EntityFactory.CreateBankAccount(1, "Acc", 100m),
                    EntityFactory.CreateCategory(1, "Income", CategoryType.Income),
                    100m, DateTime.Now, null),
                EntityFactory.CreateOperation(2,
                    EntityFactory.CreateBankAccount(1, "Acc", 100m),
                    EntityFactory.CreateCategory(2, "Expense", CategoryType.Expense),
                    40m, DateTime.Now, null),
                EntityFactory.CreateOperation(3,
                    EntityFactory.CreateBankAccount(1, "Acc", 100m),
                    EntityFactory.CreateCategory(2, "Expense", CategoryType.Expense),
                    10m, DateTime.Now, null)
            };
            var command = new GetGroupedByCategoryCommand(analyticsService, operations);
            var result = command.Execute();

            Assert.Equal(100m, result[1]);
            Assert.Equal(50m, result[2]); // 40 + 10
        }

        [Fact]
        public void ExportDataCommand_ExportsDataSuccessfully()
        {
            var bankStore = new TestStore<BankAccount>(new List<BankAccount>
            {
                EntityFactory.CreateBankAccount(1, "Acc1", 500m)
            });
            var catStore = new TestStore<Category>(new List<Category>
            {
                EntityFactory.CreateCategory(1, "Salary", CategoryType.Income)
            });
            var opStore = new TestStore<Operation>(new List<Operation>
            {
                EntityFactory.CreateOperation(1, bankStore.GetAll().First(), catStore.GetAll().First(), 500m,
                    DateTime.Now, "Test")
            });

            var exportCommand = new ExportDataCommand(catStore, opStore, bankStore, tempFilePath, FileFormat.Json);
            exportCommand.Execute();

            Assert.True(File.Exists(tempFilePath));
            var content = File.ReadAllText(tempFilePath);
            Assert.Contains("Acc1", content);
            Assert.Contains("Salary", content);
        }

        [Fact]
        public void ImportDataCommand_ImportsDataSuccessfully()
        {
            var contextDto = new
            {
                bankAccounts = new List<object> { new { Id = 1, Name = "Acc1", Balance = 500m } },
                categories = new List<object> { new { Id = 1, Type = "Income", Name = "Salary" } },
                operations = new List<object>
                {
                    new
                    {
                        Id = 1, Type = "Income", BankAccountId = 1, Amount = 500m,
                        Date = DateTime.Now.ToString("yyyy-MM-dd"), Description = "Test", CategoryId = 1
                    }
                }
            };
            var json = JsonConvert.SerializeObject(contextDto, Formatting.Indented);
            File.WriteAllText(tempFilePath, json);

            var bankStore = new TestStore<BankAccount>(Enumerable.Empty<BankAccount>());
            var catStore = new TestStore<Category>(Enumerable.Empty<Category>());
            var opStore = new TestStore<Operation>(Enumerable.Empty<Operation>());

            var importCommand = new ImportDataCommand(catStore, opStore, bankStore, FileFormat.Json, tempFilePath);
            importCommand.Execute();

            Assert.Single(bankStore.GetAll());
            Assert.Single(catStore.GetAll());
            Assert.Single(opStore.GetAll());
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
    }
}