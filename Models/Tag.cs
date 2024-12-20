using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Tag
{
    public Guid TagId { get; set; }

    public string TagName { get; set; } = null!;

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
