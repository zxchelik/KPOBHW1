using System.Collections;
using Core.Interfaces;
using Core.Models;

namespace Core.Factories;

public static class FinanceContextFactory
{
    public static FinanceContext CreateFinanceContext(
        IStore<BankAccount> baStore,
        IStore<Category> cStore,
        IStore<Operation> oStore
    )
    {
        return new FinanceContext(baStore.GetAll(), cStore.GetAll(), oStore.GetAll());
    }
}