using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Core.Models;
using Core.Enums;
using Core.Factories;
using Core.Services;

namespace Tests
{
    public class AnalyticsAndFactoriesTests
    {
        [Fact]
        public void GetBalanceDifference_ReturnsCorrectDifference()
        {
            // Arrange: создаем операции с доходами и расходами
            var operations = new List<Operation>
            {
                new Operation 
                { 
                    Id = 1,
                    Type = CategoryType.Income,
                    BankAccountId = 1,
                    Amount = 1000m,
                    Date = new DateTime(2025, 03, 10),
                    CategoryId = 1
                },
                new Operation 
                { 
                    Id = 2,
                    Type = CategoryType.Expense,
                    BankAccountId = 1,
                    Amount = 300m,
                    Date = new DateTime(2025, 03, 11),
                    CategoryId = 2
                },
                new Operation 
                { 
                    Id = 3,
                    Type = CategoryType.Income,
                    BankAccountId = 1,
                    Amount = 500m,
                    Date = new DateTime(2025, 03, 15),
                    CategoryId = 1
                }
            };
            var analyticsService = new AnalyticsService();
            DateTime startDate = new DateTime(2025, 03, 09);
            DateTime endDate = new DateTime(2025, 03, 12);

            // Act
            var result = analyticsService.GetBalanceDifference(operations, startDate, endDate);

            // Assert (учитываются операции с Id 1 и 2: 1000 - 300 = 700)
            Assert.Equal(700m, result);
        }

        [Fact]
        public void GetGroupedByCategory_ReturnsCorrectGrouping()
        {
            // Arrange: операции с разными категориями
            var operations = new List<Operation>
            {
                new Operation 
                { 
                    Id = 1,
                    Type = CategoryType.Income,
                    BankAccountId = 1,
                    Amount = 1000m,
                    Date = DateTime.Now,
                    CategoryId = 1
                },
                new Operation 
                { 
                    Id = 2,
                    Type = CategoryType.Expense,
                    BankAccountId = 1,
                    Amount = 300m,
                    Date = DateTime.Now,
                    CategoryId = 2
                },
                new Operation 
                { 
                    Id = 3,
                    Type = CategoryType.Expense,
                    BankAccountId = 1,
                    Amount = 200m,
                    Date = DateTime.Now,
                    CategoryId = 2
                }
            };
            var analyticsService = new AnalyticsService();

            // Act
            var result = analyticsService.GetGroupedByCategory(operations);

            // Assert: для категории 1 сумма должна равняться 1000, для категории 2 – 500
            Assert.True(result.ContainsKey(1));
            Assert.True(result.ContainsKey(2));
            Assert.Equal(1000m, result[1]);
            Assert.Equal(500m, result[2]);
        }

        [Fact]
        public void CreateBankAccount_ValidInput_ReturnsBankAccount()
        {
            int id = 1;
            string name = "Main Account";
            decimal balance = 1000m;

            var account = EntityFactory.CreateBankAccount(id, name, balance);

            Assert.NotNull(account);
            Assert.Equal(id, account.Id);
            Assert.Equal(name, account.Name);
            Assert.Equal(balance, account.Balance);
        }

        [Fact]
        public void CreateBankAccount_EmptyName_ThrowsArgumentException()
        {
            int id = 1;
            string name = "";
            decimal balance = 1000m;

            Assert.Throws<ArgumentException>(() => EntityFactory.CreateBankAccount(id, name, balance));
        }

        [Fact]
        public void CreateCategory_ValidInput_ReturnsCategory()
        {
            int id = 1;
            string name = "Salary";
            var type = CategoryType.Income;

            var category = EntityFactory.CreateCategory(id, name, type);

            Assert.NotNull(category);
            Assert.Equal(id, category.Id);
            Assert.Equal(name, category.Name);
            Assert.Equal(type, category.Type);
        }

        [Fact]
        public void CreateCategory_EmptyName_ThrowsArgumentException()
        {
            int id = 1;
            string name = "";
            var type = CategoryType.Expense;

            Assert.Throws<ArgumentException>(() => EntityFactory.CreateCategory(id, name, type));
        }

        [Fact]
        public void CreateOperation_ValidInput_ReturnsOperation()
        {
            int id = 1;
            var account = EntityFactory.CreateBankAccount(1, "Main Account", 1000m);
            var category = EntityFactory.CreateCategory(1, "Salary", CategoryType.Income);
            decimal amount = 500m;
            DateTime date = DateTime.Now;
            string description = "Monthly salary";

            var operation = EntityFactory.CreateOperation(id, account, category, amount, date, description);

            Assert.NotNull(operation);
            Assert.Equal(id, operation.Id);
            Assert.Equal(account.Id, operation.BankAccountId);
            Assert.Equal(category.Id, operation.CategoryId);
            Assert.Equal(amount, operation.Amount);
            Assert.Equal(date, operation.Date);
            Assert.Equal(description, operation.Description);
            Assert.Equal(category.Type, operation.Type);
        }

        [Fact]
        public void CreateOperation_InvalidAmount_ThrowsArgumentException()
        {
            int id = 1;
            var account = EntityFactory.CreateBankAccount(1, "Main Account", 1000m);
            var category = EntityFactory.CreateCategory(1, "Salary", CategoryType.Income);
            decimal amount = -100m;
            DateTime date = DateTime.Now;

            Assert.Throws<ArgumentException>(() => EntityFactory.CreateOperation(id, account, category, amount, date));
        }
    }
}
