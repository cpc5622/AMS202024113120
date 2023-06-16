using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AMS202024113120.Models;

public partial class AssetContext : DbContext
{
    public AssetContext()
    {
    }

    public AssetContext(DbContextOptions<AssetContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Asset> Assets { get; set; }

    public virtual DbSet<AssetCategory> AssetCategories { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=(LocalDB)\\MSSQLLocalDB; AttachDbFilename= D:\\webASP\\AMS202024113120\\AMS202024113120\\App_Data\\AssetsDb.mdf;Integrated Security=True;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Asset>(entity =>
        {
            entity.HasKey(e => e.AssetId).HasName("PK__Assets__4349235211BD4D50");

            entity.Property(e => e.AssetName).HasMaxLength(255);
            entity.Property(e => e.AssetSpec).HasMaxLength(255);
            entity.Property(e => e.CustodianId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.ImgName).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(255);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchaseDate).HasColumnType("datetime");

            entity.HasOne(d => d.Category).WithMany(p => p.Assets)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK_Assets_AssetCategories");

            entity.HasOne(d => d.Custodian).WithMany(p => p.Assets)
                .HasForeignKey(d => d.CustodianId)
                .HasConstraintName("FK_Assets_Employees");
        });

        modelBuilder.Entity<AssetCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__AssetCat__19093A0BB826A9B6");

            entity.Property(e => e.CategoryName).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK__tmp_ms_x__B2079BEDBB1B43AC");

            entity.Property(e => e.DepartmentName).HasMaxLength(255);
            entity.Property(e => e.ManagerId)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Manager).WithMany(p => p.Departments)
                .HasForeignKey(d => d.ManagerId)
                .HasConstraintName("FK_Departments_Employees");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F11DA9C2956");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Department).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Employees_Departments");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
