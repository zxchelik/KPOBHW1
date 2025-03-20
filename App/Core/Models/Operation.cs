using Core.Enums;
using Core.Interfaces;

namespace Core.Models;

public class Operation: IEntity
{
    public int Id { get; init; }
    public CategoryType Type { get; set; }
    public int BankAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string? Description { get; set; }
    public int CategoryId { get; set; }

    public Category? GetCategory(IStore<Category> store)
    {
        return store.GetById(CategoryId);
    }

    public BankAccount? GetBankAccount(IStore<BankAccount> store)
    {
        return store.GetById(BankAccountId);
    }
}