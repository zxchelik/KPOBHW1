using Core.Interfaces;
using Core.Models;

namespace Core.Commands;

public class GetGroupedByCategoryCommand(IAnalyticsService analyticsService, IEnumerable<Operation> operations)
    : ICommand<Dictionary<int, decimal>>
{
    public Dictionary<int, decimal> Execute()
    {
        return analyticsService.GetGroupedByCategory(operations);
    }
}