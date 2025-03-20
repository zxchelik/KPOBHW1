using Core.Enums;
using Core.Facades;
using Core.Interfaces;
using Core.Models;

namespace Core.Commands;

public class ImportDataCommand(
    IStore<Category> categories,
    IStore<Operation> operations,
    IStore<BankAccount> bankAccounts,
    FileFormat format,
    string filePath
    ): ICommand
{
    public void Execute()
    {
        var fc = ImportFacade.ImportAll(format, filePath);
        
        categories.RewriteAllData(fc.Categories);
        operations.RewriteAllData(fc.Operations);
        bankAccounts.RewriteAllData(fc.BankAccounts);
    }
}