using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Company
{
    public string CompanyId { get; set; } = null!;

    public string? NameVie { get; set; }

    public string? NameEng { get; set; }

    public string? NameAbbr { get; set; }

    public string? CompanyEmail { get; set; }

    public string? CompanyPhone { get; set; }

    public string? CompanyAddress { get; set; }

    public string? CompanyDescription { get; set; }

    public string? BusinessLicenseNo { get; set; }

    public DateOnly? BusinessLicenseDate { get; set; }

    public string? IssuedBy { get; set; }

    public string? InternationalTravelLicenseNo { get; set; }

    public DateOnly? InternationalTravelLicenseDate { get; set; }

    public string? FacebookUrl { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Newspaper> Newspapers { get; set; } = new List<Newspaper>();
}
