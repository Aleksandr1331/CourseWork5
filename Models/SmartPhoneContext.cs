using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CourseWork.Models;

public partial class SmartPhoneContext : DbContext
{
    public SmartPhoneContext()
    {
    }

    public SmartPhoneContext(DbContextOptions<SmartPhoneContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CameraInPhone> CameraInPhones { get; set; }

    public virtual DbSet<CameraType> CameraTypes { get; set; }

    public virtual DbSet<Equipment> Equipment { get; set; }

    public virtual DbSet<MatrixType> MatrixTypes { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Phone> Phones { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=TAWORI;Database=SmartPhone;User Id=Smartphone_Visual;Password=Smartphone_Visual;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.ToTable("Brand");

            entity.Property(e => e.BrandId)
                .ValueGeneratedNever()
                .HasColumnName("Brand ID");
            entity.Property(e => e.BrandName)
                .HasMaxLength(50)
                .HasColumnName("Brand_Name");
        });

        modelBuilder.Entity<CameraInPhone>(entity =>
        {
            entity.HasKey(e => e.CameraId);

            entity.ToTable("CameraInPhone");

            entity.Property(e => e.CameraId)
                .ValueGeneratedNever()
                .HasColumnName("Camera ID");
            entity.Property(e => e.PhoneId).HasColumnName("Phone ID");
            entity.Property(e => e.Specifications).HasMaxLength(50);
            entity.Property(e => e.TypeId).HasColumnName("Type ID");

            entity.HasOne(d => d.Phone).WithMany(p => p.CameraInPhones)
                .HasForeignKey(d => d.PhoneId)
                .HasConstraintName("FK_CameraInPhone_Phone");

            entity.HasOne(d => d.Type).WithMany(p => p.CameraInPhones)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CameraInPhone_CameraType");
        });

        modelBuilder.Entity<CameraType>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("CameraType");

            entity.Property(e => e.TypeId)
                .ValueGeneratedNever()
                .HasColumnName("Type ID");
            entity.Property(e => e.Type).HasMaxLength(50);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.Property(e => e.EquipmentId)
                .ValueGeneratedNever()
                .HasColumnName("Equipment ID");
            entity.Property(e => e.EquipmentName)
                .HasMaxLength(50)
                .HasColumnName("Equipment_Name");
        });

        modelBuilder.Entity<MatrixType>(entity =>
        {
            entity.ToTable("MatrixType");

            entity.Property(e => e.MatrixTypeId)
                .ValueGeneratedNever()
                .HasColumnName("MatrixType ID");
            entity.Property(e => e.MatrixType1)
                .HasMaxLength(50)
                .HasColumnName("MatrixType");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.ToTable("Model");

            entity.Property(e => e.ModelId)
                .ValueGeneratedNever()
                .HasColumnName("Model ID");
            entity.Property(e => e.BrandId).HasColumnName("Brand ID");
            entity.Property(e => e.Model1)
                .HasMaxLength(50)
                .HasColumnName("Model");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Model_Brand");
        });

        modelBuilder.Entity<Phone>(entity =>
        {
            entity.ToTable("Phone");

            entity.Property(e => e.PhoneId)
                .ValueGeneratedNever()
                .HasColumnName("Phone ID");
            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.ModelId).HasColumnName("Model ID");
            entity.Property(e => e.Rem).HasColumnName("REM");

            entity.HasOne(d => d.Model).WithMany(p => p.Phones)
                .HasForeignKey(d => d.ModelId)
                .HasConstraintName("FK_Phone_Model");

            entity.HasMany(d => d.Equipment).WithMany(p => p.Phones)
                .UsingEntity<Dictionary<string, object>>(
                    "EquipmentPhone",
                    r => r.HasOne<Equipment>().WithMany()
                        .HasForeignKey("EquipmentId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Equipment_Phone_Equipment"),
                    l => l.HasOne<Phone>().WithMany()
                        .HasForeignKey("PhoneId")
                        .HasConstraintName("FK_Equipment_Phone_Phone"),
                    j =>
                    {
                        j.HasKey("PhoneId", "EquipmentId");
                        j.ToTable("Equipment_Phone");
                        j.IndexerProperty<Guid>("PhoneId").HasColumnName("Phone ID");
                        j.IndexerProperty<Guid>("EquipmentId").HasColumnName("Equipment ID");
                    });

            entity.HasMany(d => d.MatrixTypes).WithMany(p => p.Phones)
                .UsingEntity<Dictionary<string, object>>(
                    "PhoneMatrix",
                    r => r.HasOne<MatrixType>().WithMany()
                        .HasForeignKey("MatrixTypeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Phone_Matrix_MatrixType"),
                    l => l.HasOne<Phone>().WithMany()
                        .HasForeignKey("PhoneId")
                        .HasConstraintName("FK_Phone_Matrix_Phone"),
                    j =>
                    {
                        j.HasKey("PhoneId", "MatrixTypeId");
                        j.ToTable("Phone_Matrix");
                        j.IndexerProperty<Guid>("PhoneId").HasColumnName("Phone ID");
                        j.IndexerProperty<Guid>("MatrixTypeId").HasColumnName("MatrixType ID");
                    });

            entity.HasMany(d => d.Tags).WithMany(p => p.Phones)
                .UsingEntity<Dictionary<string, object>>(
                    "PhoneTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Phone_Tag_Tag"),
                    l => l.HasOne<Phone>().WithMany()
                        .HasForeignKey("PhoneId")
                        .HasConstraintName("FK_Phone_Tag_Phone"),
                    j =>
                    {
                        j.HasKey("PhoneId", "TagId");
                        j.ToTable("Phone_Tag");
                        j.IndexerProperty<Guid>("PhoneId").HasColumnName("Phone ID");
                        j.IndexerProperty<Guid>("TagId").HasColumnName("Tag ID");
                    });
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");

            entity.Property(e => e.TagId)
                .ValueGeneratedNever()
                .HasColumnName("Tag ID");
            entity.Property(e => e.TagName)
                .HasMaxLength(50)
                .HasColumnName("Tag_Name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
