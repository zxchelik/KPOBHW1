using System.Transactions;
using Core.Models;

namespace Core.Interfaces;

public interface IStore<T>
{
    void Add(T item);
    void Update(T item);
    void DeleteById(int id);
    T? GetById(int id);
    IEnumerable<T> GetAll();
    void RewriteAllData(IEnumerable<T> items);
}
