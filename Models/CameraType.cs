using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class CameraType
{
    public Guid TypeId { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<CameraInPhone> CameraInPhones { get; set; } = new List<CameraInPhone>();
}
