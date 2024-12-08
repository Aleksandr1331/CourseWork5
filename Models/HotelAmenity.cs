using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class HotelAmenity
{
    public Guid RoomId { get; set; }

    public Guid AmenityId { get; set; }

    public virtual Amenity Amenity { get; set; } = null!;

    public virtual Hotel Room { get; set; } = null!;

    public virtual Room RoomNavigation { get; set; } = null!;
}
