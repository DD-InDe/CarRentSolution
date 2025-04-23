using System;
using System.Collections.Generic;

namespace CarRentSolution.Entity;

public partial class Company
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public string Boss { get; set; } = null!;

    public string Document { get; set; } = null!;

    public string LegalAddress { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Inn { get; set; } = null!;

    public string Kpp { get; set; } = null!;

    public string Bank { get; set; } = null!;

    public string CurrentAccount { get; set; } = null!;

    public string CorrespondentAccount { get; set; } = null!;

    public string Bik { get; set; } = null!;
}
