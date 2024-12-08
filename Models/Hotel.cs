using System;
using System.Collections.Generic;

namespace CourseWork.Models;

public partial class Hotel
{
    public Guid HotelId { get; set; }

    public string Name { get; set; } = null!;

    public string Adress { get; set; } = null!;

    public double? Rating { get; set; }

    public int? ReviewCount { get; set; }

    public double? RatingLocation { get; set; }

    public double? RatingRooms { get; set; }

    public double? RatingPriceQuality { get; set; }

    public double? RatingPurity { get; set; }

    public double? RatingService { get; set; }

    public double? RatingSleepQuality { get; set; }

    public virtual ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
}
