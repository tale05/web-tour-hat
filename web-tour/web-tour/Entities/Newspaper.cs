using System;
using System.Collections.Generic;

namespace web_tour.Entities;

public partial class Newspaper
{
    public int NewspaperId { get; set; }

    public string? CompanyId { get; set; }

    public string? Title { get; set; }

    public string? ImgTitle { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedTime { get; set; }

    public virtual Company? Company { get; set; }
}
