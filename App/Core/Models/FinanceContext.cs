using System.Collections;


namespace Core.Models;

public class FinanceContext(
    IEnumerable<BankAccount> bankAccounts,
    IEnumerable<Category> categories,
    IEnumerable<Operation> operations)
{
    public IEnumerable<BankAccount> BankAccounts { get; set; } = bankAccounts;
    public IEnumerable<Category> Categories { get; set; } = categories;
    public IEnumerable<Operation> Operations { get; set; } = operations;
}