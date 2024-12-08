using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Amenity
{
    public Guid AmenityId { get; set; }

    public string AmenityName { get; set; } = null!;

    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();
}
