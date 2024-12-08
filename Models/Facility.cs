using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Facility
{
    public Guid FacilitiesId { get; set; }

    public string FacilitiesName { get; set; } = null!;

    public virtual ICollection<Hotel> Hotels { get; set; } = new List<Hotel>();
}
