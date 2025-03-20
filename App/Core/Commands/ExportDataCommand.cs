using System.Transactions;
using Core.Enums;
using Core.Facades;
using Core.Factories;
using Core.Interfaces;
using Core.Models;

namespace Core.Commands;

public class ExportDataCommand(
    IStore<Category> categories,
    IStore<Operation> operations,
    IStore<BankAccount> bankAccounts,
    string filePath,
    FileFormat fileFormat
    ) : ICommand
{
    
    public void Execute()
    {
        var context =  FinanceContextFactory.CreateFinanceContext(bankAccounts, categories,operations);
        ExportFacade.Export(context, fileFormat, filePath);
    }
}