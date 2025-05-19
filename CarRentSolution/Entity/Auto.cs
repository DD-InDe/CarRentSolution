using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Auto
{
    public string Vin { get; set; } = null!;

    public int? Year { get; set; }

    public string Engine { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string Color { get; set; } = null!;

    public string GovNumber { get; set; } = null!;

    public string Passport { get; set; } = null!;

    public DateOnly PassportDated { get; set; }

    public decimal FullPrice { get; set; }

    public decimal RentPrice { get; set; }

    public long ModelId { get; set; }

    public long? StatusId { get; set; }

    public string? Photo { get; set; }

    public string? PassportIssued { get; set; }

    public virtual Model Model { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Rent> Rents { get; set; } = new List<Rent>();

    public virtual AutoStatus? Status { get; set; }
}
