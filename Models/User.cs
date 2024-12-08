using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public Guid CityId { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
