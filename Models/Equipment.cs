using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Equipment
{
    public Guid EquipmentId { get; set; }

    public string EquipmentName { get; set; } = null!;

    public virtual ICollection<Phone> Phones { get; set; } = new List<Phone>();
}
