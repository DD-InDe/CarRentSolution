using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarRentSolution.Entity;

public partial class FrDbContext : DbContext
{
    public FrDbContext()
    {
    }

    public FrDbContext(DbContextOptions<FrDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auto> Autos { get; set; }

    public virtual DbSet<AutoStatus> AutoStatuses { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Model> Models { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderHistory> OrderHistories { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Rent> Rents { get; set; }

    public virtual DbSet<RepairCondition> RepairConditions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<TenantDocument> TenantDocuments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=localhost;Port=5050;Database=fr_db;Username=fr_login;Password=fr_pass");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auto>(entity =>
        {
            entity.HasKey(e => e.Vin).HasName("auto_pkey");

            entity.ToTable("auto");

            entity.HasIndex(e => e.ModelId, "IX_auto_model_id");

            entity.HasIndex(e => e.Body, "auto_body_key").IsUnique();

            entity.HasIndex(e => e.Engine, "auto_engine_key").IsUnique();

            entity.HasIndex(e => e.GovNumber, "auto_gov_number_key").IsUnique();

            entity.HasIndex(e => e.Passport, "auto_passport_key").IsUnique();

            entity.Property(e => e.Vin)
                .HasMaxLength(17)
                .HasColumnName("vin");
            entity.Property(e => e.Body)
                .HasMaxLength(17)
                .HasColumnName("body");
            entity.Property(e => e.Color)
                .HasColumnType("character varying")
                .HasColumnName("color");
            entity.Property(e => e.Engine)
                .HasMaxLength(17)
                .HasColumnName("engine");
            entity.Property(e => e.FullPrice)
                .HasColumnType("money")
                .HasColumnName("full_price");
            entity.Property(e => e.GovNumber)
                .HasMaxLength(9)
                .HasColumnName("gov_number");
            entity.Property(e => e.ModelId).HasColumnName("model_id");
            entity.Property(e => e.Passport)
                .HasMaxLength(12)
                .HasColumnName("passport");
            entity.Property(e => e.PassportDated).HasColumnName("passport_dated");
            entity.Property(e => e.PassportIssued)
                .HasMaxLength(100)
                .HasColumnName("passport_issued");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");
            entity.Property(e => e.RentPrice)
                .HasColumnType("money")
                .HasColumnName("rent_price");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Model).WithMany(p => p.Autos)
                .HasForeignKey(d => d.ModelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auto_model_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Autos)
                .HasForeignKey(d => d.StatusId)
                .HasConstraintName("auto_status_id_fkey");
        });

        modelBuilder.Entity<AutoStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("auto_status_pkey");

            entity.ToTable("auto_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("brand_pkey");

            entity.ToTable("brand");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_pkey");

            entity.ToTable("company");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bank)
                .HasMaxLength(100)
                .HasColumnName("bank");
            entity.Property(e => e.Bik)
                .HasMaxLength(9)
                .HasColumnName("bik");
            entity.Property(e => e.Boss)
                .HasColumnType("character varying")
                .HasColumnName("boss");
            entity.Property(e => e.CorrespondentAccount)
                .HasMaxLength(20)
                .HasColumnName("correspondent_account");
            entity.Property(e => e.CurrentAccount)
                .HasMaxLength(20)
                .HasColumnName("current_account");
            entity.Property(e => e.Document)
                .HasColumnType("character varying")
                .HasColumnName("document");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(10)
                .HasColumnName("inn");
            entity.Property(e => e.Kpp)
                .HasMaxLength(9)
                .HasColumnName("kpp");
            entity.Property(e => e.LegalAddress)
                .HasColumnType("character varying")
                .HasColumnName("legal_address");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Employees)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("employee___fk");
        });

        modelBuilder.Entity<Model>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("model_pkey");

            entity.ToTable("model");

            entity.HasIndex(e => e.BrandId, "IX_model_brand_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BrandId).HasColumnName("brand_id");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");

            entity.HasOne(d => d.Brand).WithMany(p => p.Models)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("model_brand_id_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pkey");

            entity.ToTable("order");

            entity.HasIndex(e => e.AutoId, "IX_order_auto_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutoId)
                .HasMaxLength(17)
                .HasColumnName("auto_id");
            entity.Property(e => e.ClientFirstName)
                .HasMaxLength(100)
                .HasColumnName("client_first_name");
            entity.Property(e => e.ClientLastName)
                .HasMaxLength(100)
                .HasColumnName("client_last_name");
            entity.Property(e => e.ClientMiddleName)
                .HasMaxLength(100)
                .HasColumnName("client_middle_name");
            entity.Property(e => e.ClientPhone)
                .HasMaxLength(20)
                .HasColumnName("client_phone");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateEndRent).HasColumnName("date_end_rent");
            entity.Property(e => e.DateStartRent).HasColumnName("date_start_rent");
            entity.Property(e => e.OrderId).HasColumnName("order_id");

            entity.HasOne(d => d.Auto).WithMany(p => p.Orders)
                .HasForeignKey(d => d.AutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_auto_id_fkey");

            entity.HasOne(d => d.OrderNavigation).WithMany(p => p.InverseOrderNavigation)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("order_order_id_fkey");
        });

        modelBuilder.Entity<OrderHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_history_pkey");

            entity.ToTable("order_history");

            entity.HasIndex(e => e.EmployeeId, "IX_order_history_employee_id");

            entity.HasIndex(e => e.OrderId, "IX_order_history_order_id");

            entity.HasIndex(e => e.StatusId, "IX_order_history_status_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Time).HasColumnName("time");

            entity.HasOne(d => d.Employee).WithMany(p => p.OrderHistories)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("order_history_employee_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderHistories)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_history_order_id_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderHistories)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_history_status_id_fkey");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_status_pkey");

            entity.ToTable("order_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Rent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("rent_pkey");

            entity.ToTable("rent");

            entity.HasIndex(e => e.AutoId, "IX_rent_auto_id");

            entity.HasIndex(e => e.EmployeeId, "IX_rent_employee_id");

            entity.HasIndex(e => e.RepairConditionId, "IX_rent_repair_condition_id");

            entity.HasIndex(e => e.TenantId, "IX_rent_tenant_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AutoId)
                .HasMaxLength(17)
                .HasColumnName("auto_id");
            entity.Property(e => e.DateCreated).HasColumnName("date_created");
            entity.Property(e => e.DateEnd).HasColumnName("date_end");
            entity.Property(e => e.DateStart).HasColumnName("date_start");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FinishPrice)
                .HasColumnType("money")
                .HasColumnName("finish_price");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Penalties)
                .HasColumnType("money")
                .HasColumnName("penalties");
            entity.Property(e => e.RepairConditionId).HasColumnName("repair_condition_id");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");

            entity.HasOne(d => d.Auto).WithMany(p => p.Rents)
                .HasForeignKey(d => d.AutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rent_auto_id_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Rents)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rent_employee_id_fkey");

            entity.HasOne(d => d.Order).WithMany(p => p.Rents)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("rent_order_id_fkey");

            entity.HasOne(d => d.RepairCondition).WithMany(p => p.Rents)
                .HasForeignKey(d => d.RepairConditionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rent_repair_condition_id_fkey");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Rents)
                .HasForeignKey(d => d.TenantId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("rent_tenant_id_fkey");
        });

        modelBuilder.Entity<RepairCondition>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("repair_condition_pkey");

            entity.ToTable("repair_condition");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("character varying")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tenant_pkey");

            entity.ToTable("tenant");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(100)
                .HasColumnName("first_name");
            entity.Property(e => e.LastName)
                .HasMaxLength(100)
                .HasColumnName("last_name");
            entity.Property(e => e.MiddleName)
                .HasMaxLength(100)
                .HasColumnName("middle_name");
            entity.Property(e => e.PassportDated).HasColumnName("passport_dated");
            entity.Property(e => e.PassportIssued)
                .HasColumnType("character varying")
                .HasColumnName("passport_issued");
            entity.Property(e => e.PassportNumber)
                .HasMaxLength(10)
                .HasColumnName("passport_number");
            entity.Property(e => e.PassportSerial)
                .HasMaxLength(4)
                .HasColumnName("passport_serial");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
        });

        modelBuilder.Entity<TenantDocument>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tenant_document_pkey");

            entity.ToTable("tenant_document");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DriveLicense)
                .HasColumnType("character varying")
                .HasColumnName("drive_license");
            entity.Property(e => e.Passport)
                .HasColumnType("character varying")
                .HasColumnName("passport");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.TenantDocument)
                .HasForeignKey<TenantDocument>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tenant_document_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
