using Core.DTO;
using Core.Interfaces;
using Core.Models;
using Newtonsoft.Json;

namespace Core.WorkWithFiles.Import;

public class JsonImporter: IDataImporter
{
    
    public FinanceContext Import(string filePath)
    {
        var jsonString = File.ReadAllText(filePath);

        var allData = JsonConvert.DeserializeObject<FinanceContextDto>(jsonString);

        if (allData == null)
        {
            throw new Exception("Не удалось десериализовать файл: " + filePath);
        }

        // Создаём и возвращаем FinanceContext
        return new FinanceContext(
            bankAccounts: allData.BankAccounts ?? [],
            categories:   allData.Categories   ?? [],
            operations:   allData.Operations   ?? []
        );
    }
}