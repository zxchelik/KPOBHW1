using Core.Models;

namespace Core.DTO;

public class FinanceContextDto
{
    public List<BankAccount>? BankAccounts { get; set; }
    public List<Category>? Categories     { get; set; }
    public List<Operation>? Operations    { get; set; }
}