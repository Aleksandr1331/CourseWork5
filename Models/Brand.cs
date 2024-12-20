using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Brand
{
    public Guid BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public virtual ICollection<Model> Models { get; set; } = new List<Model>();
}
