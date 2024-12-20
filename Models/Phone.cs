using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Phone
{
    public Guid PhoneId { get; set; }

    public Guid ModelId { get; set; }

    public double CurrentPrice { get; set; }

    public int? Discont { get; set; }

    public double? OldPrice { get; set; }

    public string Color { get; set; } = null!;

    public int Rem { get; set; }

    public int BatteryCapacity { get; set; }

    public double ScreenDiagonal { get; set; }

    public virtual ICollection<CameraInPhone> CameraInPhones { get; set; } = new List<CameraInPhone>();

    public virtual Model Model { get; set; } = null!;

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();

    public virtual ICollection<MatrixType> MatrixTypes { get; set; } = new List<MatrixType>();

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
