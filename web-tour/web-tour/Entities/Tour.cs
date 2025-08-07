using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Tour
{
    public string ToursId { get; set; } = null!;

    public string? CategoryId { get; set; }

    public string? Title { get; set; }

    public string? ImgTitle { get; set; }

    public bool? StatusTour { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Tourdetail> Tourdetails { get; set; } = new List<Tourdetail>();
}
