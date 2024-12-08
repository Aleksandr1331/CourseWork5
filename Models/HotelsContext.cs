using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Models;

public partial class HotelsContext : DbContext
{
    public HotelsContext()
    {
    }

    public HotelsContext(DbContextOptions<HotelsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Amenity> Amenities { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<Hotel> Hotels { get; set; }

    public virtual DbSet<HotelAmenity> HotelAmenities { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TAWORI;Database=Hotels;Trusted_Connection=True;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Amenity>(entity =>
        {
            entity.HasKey(e => e.AmenityId).HasName("PK_Amenity_1");

            entity.ToTable("Amenity");

            entity.Property(e => e.AmenityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("AmenityID");
            entity.Property(e => e.AmenityName).HasMaxLength(20);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK_City_1");

            entity.ToTable("City");

            entity.Property(e => e.CityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("CItyID");
            entity.Property(e => e.CityName).HasMaxLength(20);
        });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilitiesId);

            entity.Property(e => e.FacilitiesId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("FacilitiesID");
            entity.Property(e => e.FacilitiesName).HasMaxLength(52);
        });

        modelBuilder.Entity<Hotel>(entity =>
        {
            entity.ToTable("Hotel");

            entity.Property(e => e.HotelId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("HotelID");
            entity.Property(e => e.Adress).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(40);
            entity.Property(e => e.RatingLocation).HasColumnName("Rating_Location");
            entity.Property(e => e.RatingPriceQuality).HasColumnName("Rating_PriceQuality");
            entity.Property(e => e.RatingPurity).HasColumnName("Rating_Purity");
            entity.Property(e => e.RatingRooms).HasColumnName("Rating_Rooms");
            entity.Property(e => e.RatingService).HasColumnName("Rating_Service");
            entity.Property(e => e.RatingSleepQuality).HasColumnName("Rating_SleepQuality");
            entity.Property(e => e.ReviewCount).HasColumnName("Review_Count");

            entity.HasMany(d => d.Facilities).WithMany(p => p.Hotels)
                .UsingEntity<Dictionary<string, object>>(
                    "RoomFacility",
                    r => r.HasOne<Facility>().WithMany()
                        .HasForeignKey("FacilitiesId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Room facilities_Facilities"),
                    l => l.HasOne<Hotel>().WithMany()
                        .HasForeignKey("HotelId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Room facilities_Hotel"),
                    j =>
                    {
                        j.HasKey("HotelId", "FacilitiesId");
                        j.ToTable("Room facilities");
                        j.IndexerProperty<Guid>("HotelId").HasColumnName("HotelID");
                        j.IndexerProperty<Guid>("FacilitiesId").HasColumnName("FacilitiesID");
                    });
        });

        modelBuilder.Entity<HotelAmenity>(entity =>
        {
            entity.HasKey(e => new { e.RoomId, e.AmenityId }).HasName("PK_Amenity_Room_1");

            entity.ToTable("Hotel Amenity");

            entity.Property(e => e.RoomId).HasColumnName("RoomID");
            entity.Property(e => e.AmenityId).HasColumnName("AmenityID");

            entity.HasOne(d => d.Amenity).WithMany(p => p.HotelAmenities)
                .HasForeignKey(d => d.AmenityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Amenity_Room_Amenity");

            entity.HasOne(d => d.Room).WithMany(p => p.HotelAmenities)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Hotel Amenity_Hotel");

            entity.HasOne(d => d.RoomNavigation).WithMany(p => p.HotelAmenities)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Amenity_Room_Room");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.ToTable("Review");

            entity.Property(e => e.ReviewId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("ReviewID");
            entity.Property(e => e.HotelId).HasColumnName("HotelID");
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Hotel).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_Hotel");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Review_User");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("Room");

            entity.Property(e => e.RoomId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("RoomID");
            entity.Property(e => e.HotelId).HasColumnName("HotelID");
            entity.Property(e => e.RoomType).HasMaxLength(50);

            entity.HasOne(d => d.Hotel).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.HotelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Room_Hotel");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("UserID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User_City");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
