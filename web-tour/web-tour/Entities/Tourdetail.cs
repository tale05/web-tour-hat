using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Tourdetail
{
    public int TourdetailId { get; set; }

    public string? ToursId { get; set; }

    public string? DescriptionTour { get; set; }

    public string? ContentTour { get; set; }

    public decimal? PriceBefore { get; set; }

    public decimal? PriceAfter { get; set; }

    public virtual Tour? Tours { get; set; }
}
