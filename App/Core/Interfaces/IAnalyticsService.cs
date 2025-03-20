using Core.Models;

namespace Core.Interfaces;

public interface IAnalyticsService
{
    decimal GetBalanceDifference(IEnumerable<Operation> operations, DateTime startDate, DateTime endDate);
    Dictionary<int, decimal> GetGroupedByCategory(IEnumerable<Operation> operations);
}