using Core.Interfaces;
using Core.Models;
using Newtonsoft.Json;

namespace Core.WorkWithFiles.Export;

public class JsonExporter : IDataExporter
{
    public void Export(FinanceContext context, string filePath)
    {
        var jsonString = JsonConvert.SerializeObject(
            context,
            Formatting.Indented
        );

        File.WriteAllText(filePath, jsonString);
    }
}