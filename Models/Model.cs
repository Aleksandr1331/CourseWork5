using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Model
{
    public Guid ModelId { get; set; }

    public Guid BrandId { get; set; }

    public string Model1 { get; set; } = null!;

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
