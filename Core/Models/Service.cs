using System;

namespace Core.Models;

public class Service
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal BasePrice { get; set; }
}
