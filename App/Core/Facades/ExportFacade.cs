using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.WorkWithFiles.Export;

namespace Core.Facades;

public static class ExportFacade
{
    public static void Export(FinanceContext context, FileFormat format, string filePath)
    {
        var exporter = CreateExporter(format);

        exporter.Export(context, filePath);
    }


    private static IDataExporter CreateExporter(FileFormat format)
    {
        return format switch
        {
            FileFormat.Json => new JsonExporter(),
            FileFormat.Yaml => new YamlExporter(),
            FileFormat.Excel => new ExcelExporter(),
            _ => throw new NotImplementedException($"Неизвестный формат: {format}")
        };
    }
}