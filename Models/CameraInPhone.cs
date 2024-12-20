using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class CameraInPhone
{
    public Guid CameraId { get; set; }

    public Guid PhoneId { get; set; }

    public string Specifications { get; set; } = null!;

    public Guid TypeId { get; set; }

    public virtual Phone Phone { get; set; } = null!;

    public virtual CameraType Type { get; set; } = null!;
}
