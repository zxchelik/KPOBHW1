using Core.DTO;
using Core.Interfaces;
using Core.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Core.WorkWithFiles.Import;

public class YamlImporter : IDataImporter
{
    public FinanceContext Import(string filePath)
    {
        var yamlString = File.ReadAllText(filePath);

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        // Аналогично, делаем промежуточный DTO
        var allData = deserializer.Deserialize<FinanceContextDto>(yamlString);

        if (allData == null)
            throw new Exception("Не удалось десериализовать YAML: " + filePath);

        return new FinanceContext(
            allData.BankAccounts ?? [],
            allData.Categories ?? [],
            allData.Operations ?? []
        );
    }
}