using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class City
{
    public Guid CityId { get; set; }

    public string CityName { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
