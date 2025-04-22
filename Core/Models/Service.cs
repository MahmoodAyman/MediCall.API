using System;

namespace Core.Models;

public class Service : BaseEntity
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal BasePrice { get; set; }
    public virtual List<Visit> Visits { get; set; } = [];
    public virtual List<Nurse> Nurses { get; set; } = [];
}
