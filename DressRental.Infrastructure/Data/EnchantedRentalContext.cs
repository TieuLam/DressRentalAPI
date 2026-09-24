using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using DressRental.Domain.Entities;
using Type = DressRental.Domain.Entities.Type;

namespace DressRental.Infrastructure.Data;

public partial class EnchantedRentalContext : DbContext
{
    public EnchantedRentalContext()
    {
    }

    public EnchantedRentalContext(DbContextOptions<EnchantedRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<ConsultationBooking> ConsultationBookings { get; set; }

    public virtual DbSet<CustomerProfile> CustomerProfiles { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductImage> ProductImages { get; set; }

    public virtual DbSet<ProductItem> ProductItems { get; set; }

    public virtual DbSet<ReturnInspection> ReturnInspections { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //    => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Category");

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Type).WithMany(p => p.Categories)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Category_Type");
        });

        modelBuilder.Entity<ConsultationBooking>(entity =>
        {
            entity.ToTable("ConsultationBooking");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");

            entity.HasOne(d => d.Customer).WithMany(p => p.ConsultationBookings)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK_ConsultationBooking_Customer");
        });

        modelBuilder.Entity<CustomerProfile>(entity =>
        {
            entity.ToTable("CustomerProfile");

            entity.HasIndex(e => e.PhoneNumber, "IX_CustomerProfile_PhoneNumber");

            entity.HasIndex(e => e.UserId, "UQ_CustomerProfile_UserId").IsUnique();

            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithOne(p => p.CustomerProfile)
                .HasForeignKey<CustomerProfile>(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_CustomerProfile_User");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.HasIndex(e => new { e.StartDate, e.EndDate, e.Status }, "IX_Order_RentalDates_Status");

            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("PENDING");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Order_CustomerProfile");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.HasIndex(e => new { e.ItemId, e.OrderId }, "IX_OrderDetail_ItemId_OrderId");

            entity.Property(e => e.RentalType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Item).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderDetail_ProductItem");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_OrderDetail_Order");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.PaidAt).HasDefaultValueSql("(sysdatetime())");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PaymentType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TransactionCode)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.Order).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK_Payment_Order");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.Color).HasMaxLength(50);
            entity.Property(e => e.DepositRate).HasDefaultValue(100);
            entity.Property(e => e.Name).HasMaxLength(255);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Product_Category");
        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.ToTable("ProductImage");

            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK_ProductImage_Product");
        });

        modelBuilder.Entity<ProductItem>(entity =>
        {
            entity.ToTable("ProductItem");

            entity.HasIndex(e => e.ItemCode, "IX_ProductItem_ItemCode");

            entity.HasIndex(e => e.ItemCode, "UQ_ProductItem_ItemCode").IsUnique();

            entity.Property(e => e.CurrentCondition)
                .HasMaxLength(100)
                .HasDefaultValue("Mới 99%");
            entity.Property(e => e.ItemCode)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Size)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("AVAILABLE");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductItem_Product");
        });

        modelBuilder.Entity<ReturnInspection>(entity =>
        {
            entity.ToTable("ReturnInspection");

            entity.HasIndex(e => e.OrderDetailId, "UQ_ReturnInspection_OrderDetailId").IsUnique();

            entity.Property(e => e.InspectAt).HasDefaultValueSql("(sysdatetime())");

            entity.HasOne(d => d.InspectUser).WithMany(p => p.ReturnInspections)
                .HasForeignKey(d => d.InspectUserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_ReturnInspection_User");

            entity.HasOne(d => d.OrderDetail).WithOne(p => p.ReturnInspection)
                .HasForeignKey<ReturnInspection>(d => d.OrderDetailId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReturnInspection_OrderDetail");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.ToTable("Type");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.UserName, "UQ_User_UserName").IsUnique();

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasDefaultValue("STAFF");
            entity.Property(e => e.UserName)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
