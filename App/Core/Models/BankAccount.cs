using Core.Interfaces;

namespace Core.Models;

public class BankAccount : IEntity
{
    public int Id { get; init; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
}