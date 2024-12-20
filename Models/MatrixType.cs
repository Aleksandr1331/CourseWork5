using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class MatrixType
{
    public Guid MatrixTypeId { get; set; }

    public string MatrixType1 { get; set; } = null!;

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
