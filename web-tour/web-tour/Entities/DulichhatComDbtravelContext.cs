using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace web_tour.Entities;

public partial class DulichhatComDbtravelContext : DbContext
{
    public DulichhatComDbtravelContext()
    {
    }

    public DulichhatComDbtravelContext(DbContextOptions<DulichhatComDbtravelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Newspaper> Newspapers { get; set; }

    public virtual DbSet<Tour> Tours { get; set; }

    public virtual DbSet<Tourdetail> Tourdetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Let Program.cs handle the configuration
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("dulichhat");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__category__D54EE9B4175135B3");

            entity.ToTable("category");

            entity.Property(e => e.CategoryId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("category_id");
            entity.Property(e => e.ImgCategory).HasColumnName("img_category");
            entity.Property(e => e.NameCategory)
                .HasMaxLength(255)
                .HasColumnName("name_category");
            entity.Property(e => e.StatusCategory).HasColumnName("status_category");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__company__3E267235D2E4598B");

            entity.ToTable("company");

            entity.Property(e => e.CompanyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("company_id");
            entity.Property(e => e.BusinessLicenseDate).HasColumnName("business_license_date");
            entity.Property(e => e.BusinessLicenseNo)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("business_license_no");
            entity.Property(e => e.CompanyAddress)
                .HasMaxLength(255)
                .HasColumnName("company_address");
            entity.Property(e => e.CompanyDescription).HasColumnName("company_description");
            entity.Property(e => e.CompanyEmail)
                .HasMaxLength(100)
                .HasColumnName("company_email");
            entity.Property(e => e.CompanyPhone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("company_phone");
            entity.Property(e => e.FacebookUrl)
                .HasMaxLength(255)
                .HasColumnName("facebook_url");
            entity.Property(e => e.InternationalTravelLicenseDate).HasColumnName("international_travel_license_date");
            entity.Property(e => e.InternationalTravelLicenseNo)
                .HasMaxLength(100)
                .HasColumnName("international_travel_license_no");
            entity.Property(e => e.IssuedBy)
                .HasMaxLength(255)
                .HasColumnName("issued_by");
            entity.Property(e => e.NameAbbr)
                .HasMaxLength(255)
                .HasColumnName("name_abbr");
            entity.Property(e => e.NameEng)
                .HasMaxLength(255)
                .HasColumnName("name_eng");
            entity.Property(e => e.NameVie)
                .HasMaxLength(255)
                .HasColumnName("name_vie");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__employee__C52E0BA824D29314");

            entity.ToTable("employee");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(20)
                .HasColumnName("employee_id");
            entity.Property(e => e.CompanyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("company_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FirstNameEmployee)
                .HasMaxLength(255)
                .HasColumnName("first_name_employee");
            entity.Property(e => e.LastNameEmployee)
                .HasMaxLength(255)
                .HasColumnName("last_name_employee");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .HasColumnName("username");

            entity.HasOne(d => d.Company).WithMany(p => p.Employees)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__employee__compan__38996AB5");
        });

        modelBuilder.Entity<Newspaper>(entity =>
        {
            entity.HasKey(e => e.NewspaperId).HasName("PK__newspape__710B3F0C0D28CBEE");

            entity.ToTable("newspaper");

            entity.Property(e => e.NewspaperId).HasColumnName("newspaper_id");
            entity.Property(e => e.CompanyId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("company_id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedTime)
                .HasColumnType("datetime")
                .HasColumnName("created_time");
            entity.Property(e => e.ImgTitle).HasColumnName("img_title");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Company).WithMany(p => p.Newspapers)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__newspaper__compa__4316F928");
        });

        modelBuilder.Entity<Tour>(entity =>
        {
            entity.HasKey(e => e.ToursId).HasName("PK__tours__A3BC1356A0D8DA46");

            entity.ToTable("tours");

            entity.Property(e => e.ToursId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tours_id");
            entity.Property(e => e.CategoryId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("category_id");
            entity.Property(e => e.ImgTitle).HasColumnName("img_title");
            entity.Property(e => e.StatusTour).HasColumnName("status_tour");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Category).WithMany(p => p.Tours)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__tours__category___3D5E1FD2");
        });

        modelBuilder.Entity<Tourdetail>(entity =>
        {
            entity.HasKey(e => e.TourdetailId).HasName("PK__tourdeta__404FE0866FAE64D1");

            entity.ToTable("tourdetail");

            entity.Property(e => e.TourdetailId).HasColumnName("tourdetail_id");
            entity.Property(e => e.ContentTour).HasColumnName("content_tour");
            entity.Property(e => e.DescriptionTour).HasColumnName("description_tour");
            entity.Property(e => e.PriceAfter)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_after");
            entity.Property(e => e.PriceBefore)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("price_before");
            entity.Property(e => e.ToursId)
                .HasMaxLength(20)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tours_id");

            entity.HasOne(d => d.Tours).WithMany(p => p.Tourdetails)
                .HasForeignKey(d => d.ToursId)
                .HasConstraintName("FK__tourdetai__tours__403A8C7D");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
