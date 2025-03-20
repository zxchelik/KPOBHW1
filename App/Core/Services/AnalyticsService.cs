using Core.Enums;
using Core.Interfaces;
using Core.Models;

namespace Core.Services;

public class AnalyticsService: IAnalyticsService
{
    public decimal GetBalanceDifference(IEnumerable<Operation> operations, DateTime startDate, DateTime endDate)
    {
        var filteredOperations = operations
            .Where(op => op.Date >= startDate && op.Date <= endDate)
            .ToList();

        var income = filteredOperations
            .Where(op => op.Type == CategoryType.Income)
            .Sum(op => op.Amount);

        var expense = filteredOperations
            .Where(op => op.Type == CategoryType.Expense)
            .Sum(op => op.Amount);

        return income - expense;
    }

    public Dictionary<int, decimal> GetGroupedByCategory(IEnumerable<Operation> operations)
    {
        return operations
            .GroupBy(op => op.CategoryId)
            .ToDictionary(group => group.Key, group => group.Sum(op => op.Amount));
    }
}