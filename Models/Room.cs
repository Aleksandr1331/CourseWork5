using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Room
{
    public Guid RoomId { get; set; }

    public Guid HotelId { get; set; }

    public string RoomType { get; set; } = null!;

    public double Price { get; set; }

    public virtual Hotel Hotel { get; set; } = null!;

    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
}
