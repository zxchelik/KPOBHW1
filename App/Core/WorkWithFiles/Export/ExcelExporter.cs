using Core.Interfaces;
using Core.Models;
using OfficeOpenXml;

namespace Core.WorkWithFiles.Export;

public class ExcelExporter : IDataExporter
{
    public void Export(FinanceContext context, string filePath)
    {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage();

            // 1) Лист с BankAccounts
            var bankSheet = package.Workbook.Worksheets.Add("BankAccounts");
            bankSheet.Cells[1, 1].Value = "Id";
            bankSheet.Cells[1, 2].Value = "Name";
            bankSheet.Cells[1, 3].Value = "Balance";

            var row = 2;
            foreach (var ba in context.BankAccounts)
            {
                bankSheet.Cells[row, 1].Value = ba.Id;
                bankSheet.Cells[row, 2].Value = ba.Name;
                bankSheet.Cells[row, 3].Value = ba.Balance;
                row++;
            }

            // 2) Лист с Categories
            var categorySheet = package.Workbook.Worksheets.Add("Categories");
            categorySheet.Cells[1, 1].Value = "Id";
            categorySheet.Cells[1, 2].Value = "Type";
            categorySheet.Cells[1, 3].Value = "Name";

            row = 2;
            foreach (var cat in context.Categories)
            {
                categorySheet.Cells[row, 1].Value = cat.Id;
                categorySheet.Cells[row, 2].Value = cat.Type.ToString();
                categorySheet.Cells[row, 3].Value = cat.Name;
                row++;
            }

            // 3) Лист с Operations
            var operationSheet = package.Workbook.Worksheets.Add("Operations");
            operationSheet.Cells[1, 1].Value = "Id";
            operationSheet.Cells[1, 2].Value = "Type";
            operationSheet.Cells[1, 3].Value = "BankAccountId";
            operationSheet.Cells[1, 4].Value = "Amount";
            operationSheet.Cells[1, 5].Value = "Date";
            operationSheet.Cells[1, 6].Value = "Description";
            operationSheet.Cells[1, 7].Value = "CategoryId";

            row = 2;
            foreach (var op in context.Operations)
            {
                operationSheet.Cells[row, 1].Value = op.Id;
                operationSheet.Cells[row, 2].Value = op.Type.ToString();
                operationSheet.Cells[row, 3].Value = op.BankAccountId;
                operationSheet.Cells[row, 4].Value = op.Amount;
                operationSheet.Cells[row, 5].Value = op.Date.ToString("yyyy-MM-dd");
                operationSheet.Cells[row, 6].Value = op.Description;
                operationSheet.Cells[row, 7].Value = op.CategoryId;
                row++;
            }

            // Сохраняем
            package.SaveAs(new FileInfo(filePath));
        }
}