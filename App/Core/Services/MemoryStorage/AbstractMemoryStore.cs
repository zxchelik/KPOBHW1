using Core.Interfaces;

namespace Core.Services.MemoryStorage;

public abstract class AbstractMemoryStore<T> : IStore<T> where T : IEntity
{
    private readonly Dictionary<int, T> _items = new();

    public void Add(T item)
    {
        _items[item.Id] = item;
    }

    public void Update(T item)
    {
        _items[item.Id] = item;
    }

    public void DeleteById(int id)
    {
        _items.Remove(id);
    }

    public T? GetById(int id)
    {
        return _items.GetValueOrDefault(id);
    }

    public IEnumerable<T> GetAll()
    {
        return _items.Values;
    }

    public int GetNextId() => _items.Keys.Max() + 1;

    public void RewriteAllData(IEnumerable<T> items)
    {
        _items.Clear();
        foreach (var item in items)
        {
            _items[item.Id] = item;
        }
    }
}