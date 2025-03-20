using Core.Interfaces;
using Core.Models;

namespace Core.Commands;

public class GetBalanceDifferenceCommand(
    IAnalyticsService analyticsService,
    IEnumerable<Operation> operations,
    DateTime startDate,
    DateTime endDate) : ICommand<decimal>
{
    public decimal Execute()
    {
        return analyticsService.GetBalanceDifference(operations, startDate, endDate);
    }
}