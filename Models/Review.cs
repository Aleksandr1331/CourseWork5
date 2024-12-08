using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Review
{
    public Guid ReviewId { get; set; }

    public Guid HotelId { get; set; }

    public Guid UserId { get; set; }

    public double? Rating { get; set; }

    public string Title { get; set; } = null!;

    public DateOnly DatePosted { get; set; }

    public DateOnly DateCheckIn { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
