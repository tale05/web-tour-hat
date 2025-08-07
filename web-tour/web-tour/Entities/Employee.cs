using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Employee
{
    public string EmployeeId { get; set; } = null!;

    public string? CompanyId { get; set; }

    public string? FirstNameEmployee { get; set; }

    public string? LastNameEmployee { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public virtual Company? Company { get; set; }
}
