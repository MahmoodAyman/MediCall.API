using System;

namespace Core.Models;

public class Service : BaseEntity
{
    public required string Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
}
