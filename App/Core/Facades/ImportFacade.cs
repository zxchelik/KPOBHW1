using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.WorkWithFiles.Import;

namespace Core.Facades;

public static class ImportFacade
{
    public static FinanceContext ImportAll(FileFormat format, string filePath)
    {
        var importer = CreateImporter(format);
        return importer.Import(filePath);
    }

    private static IDataImporter CreateImporter(FileFormat format)
    {
        return format switch
        {
            FileFormat.Json  => new JsonImporter(),
            FileFormat.Yaml  => new YamlImporter(),
            FileFormat.Excel => new ExcelImporter(),
            _ => throw new NotImplementedException($"Неизвестный формат: {format}")
        };
    }
}