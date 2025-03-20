using Core.Models;

namespace Core.Interfaces;

public interface IDataImporter
{
    public FinanceContext Import(string filePath);
}