using Core.Enums;
using Core.Interfaces;
using Core.Models;
using OfficeOpenXml;

namespace Core.WorkWithFiles.Import;

public class ExcelImporter : IDataImporter
{
    public FinanceContext Import(string filePath)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var package = new ExcelPackage(new FileInfo(filePath));

        var bankAccounts = ReadBankAccounts(package);
        var categories = ReadCategories(package);
        var operations = ReadOperations(package);

        return new FinanceContext(bankAccounts, categories, operations);
    }

    protected virtual IEnumerable<BankAccount> ReadBankAccounts(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets["BankAccounts"];
        var list = new List<BankAccount>();

        // Предположим, что первая строка — заголовки, а данные начинаются со 2-й.
        var rowCount = sheet.Dimension.End.Row;
        for (var row = 2; row <= rowCount; row++)
        {
            var id = Convert.ToInt32(sheet.Cells[row, 1].Value);
            var name = sheet.Cells[row, 2].Value?.ToString() ?? "";
            var balance = Convert.ToDecimal(sheet.Cells[row, 3].Value);

            list.Add(new BankAccount
            {
                Id = id,
                Name = name,
                Balance = balance
            });
        }

        return list;
    }

    protected virtual IEnumerable<Category> ReadCategories(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets["Categories"];
        var list = new List<Category>();

        var rowCount = sheet.Dimension.End.Row;
        for (var row = 2; row <= rowCount; row++)
        {
            var id = Convert.ToInt32(sheet.Cells[row, 1].Value);
            var type = sheet.Cells[row, 2].Value?.ToString();
            var name = sheet.Cells[row, 3].Value?.ToString() ?? "";

            list.Add(new Category
            {
                Id = id,
                Type = Enum.Parse<CategoryType>(type ?? "Expense", true),
                Name = name
            });
        }

        return list;
    }

    protected virtual IEnumerable<Operation> ReadOperations(ExcelPackage package)
    {
        var sheet = package.Workbook.Worksheets["Operations"];
        var list = new List<Operation>();

        var rowCount = sheet.Dimension.End.Row;
        for (var row = 2; row <= rowCount; row++)
        {
            var id = Convert.ToInt32(sheet.Cells[row, 1].Value);
            var type = sheet.Cells[row, 2].Value?.ToString();
            var baId = Convert.ToInt32(sheet.Cells[row, 3].Value);
            var amount = Convert.ToDecimal(sheet.Cells[row, 4].Value);
            var dateStr = sheet.Cells[row, 5].Value?.ToString() ?? "";
            var desc = sheet.Cells[row, 6].Value?.ToString();
            var catId = Convert.ToInt32(sheet.Cells[row, 7].Value);

            list.Add(new Operation
            {
                Id = id,
                Type = Enum.Parse<CategoryType>(type ?? "Expense", true),
                BankAccountId = baId,
                Amount = amount,
                Date = DateTime.Parse(dateStr), // или TryParse
                Description = desc,
                CategoryId = catId
            });
        }

        return list;
    }
}