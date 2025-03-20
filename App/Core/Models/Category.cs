using Core.Enums;
using Core.Interfaces;

namespace Core.Models;

public class Category: IEntity
{
    public int Id { get; init; }
    public CategoryType Type { get; set; }
    public string Name { get; set; } = "";
}