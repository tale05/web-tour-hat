using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Category
{
    public string CategoryId { get; set; } = null!;

    public string? NameCategory { get; set; }

    public string? ImgCategory { get; set; }

    public bool? StatusCategory { get; set; }

    public virtual ICollection<Tour> Tours { get; set; } = new List<Tour>();
}
