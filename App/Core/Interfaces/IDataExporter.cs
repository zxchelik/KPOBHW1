using Core.Models;

namespace Core.Interfaces;

public interface IDataExporter
{
    void Export(
        FinanceContext context,
        string filePath
    );
}