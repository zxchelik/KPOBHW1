using Core.Enums;
using Core.Models;

namespace Core.Factories;

public static class EntityFactory
{
    public static BankAccount CreateBankAccount(int id, string name, decimal balance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название счета не может быть пустым");
        if (balance < 0)
            throw new ArgumentException("Баланс не может быть отрицательным");

        return new BankAccount { Id = id, Name = name, Balance = balance };
    }

    public static Category CreateCategory(int id, string name, CategoryType type)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Название категории не может быть пустым");

        return new Category { Id = id, Name = name, Type = type };
    }

    public static Operation CreateOperation(
        int id,
        BankAccount account,
        Category category,
        decimal amount,
        DateTime date,
        string? description = null
    )
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account), "Счет не может быть null");
        if (category == null)
            throw new ArgumentNullException(nameof(category), "Категория не может быть null");
        if (amount <= 0)
            throw new ArgumentException("Сумма операции должна быть положительной");

        switch (category.Type)
        {
            case CategoryType.Expense:
                account.Balance -= amount;
                break;
            case CategoryType.Income:
                account.Balance += amount;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        return new Operation
        {
            Id = id, 
            Type = category.Type,
            BankAccountId = account.Id, 
            CategoryId = category.Id, 
            Amount = amount,
            Date = date,
            Description = description
        };
    }
}