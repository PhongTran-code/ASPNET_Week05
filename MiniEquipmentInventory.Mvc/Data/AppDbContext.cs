using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Models;

namespace MiniEquipmentInventory.Mvc.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Equipment> Equipments => Set<Equipment>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<Equipment>(entity =>
        {
            entity.ToTable("Equipments");
            entity.HasKey(e => e.EquipId);
            entity.Property(e => e.EquipCode).IsRequired().HasMaxLength(50);
            entity.Property(e => e.EquipName).IsRequired().HasMaxLength(150);
            entity.Property(e => e.EquipUnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Equipments)
                  .HasForeignKey(e => e.CategoryId);

            // Lab 05 additions
            entity.HasIndex(e => e.EquipCode).IsUnique();
            entity.Property(e => e.RowVersion).IsRowVersion();
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
        });

        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.ToTable("AuditLogs");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Level).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Message).IsRequired().HasMaxLength(500);
        });

        // Thêm Seed Data
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Projection Equipment" },
            new Category { Id = 2, Name = "Board Equipment" },
            new Category { Id = 3, Name = "Audio Equipment" },
            new Category { Id = 4, Name = "Furniture" }
        );

        modelBuilder.Entity<Equipment>().HasData(
            new Equipment
            {
                EquipId = 1,
                EquipCode = "EQ-PRJ-001",
                EquipName = "Smart Projector",
                EquipCategory = "Projection Equipment",
                EquipSupplier = "Epson Vietnam",
                EquipUnitPrice = 8500000,
                EquipQuantity = 18,
                EquipMinStock = 5,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 1,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "High quality smart projector for classrooms."
            },
            new Equipment
            {
                EquipId = 2,
                EquipCode = "EQ-BRD-002",
                EquipName = "Interactive Whiteboard",
                EquipCategory = "Board Equipment",
                EquipSupplier = "SMART Technologies",
                EquipUnitPrice = 15000000,
                EquipQuantity = 4,
                EquipMinStock = 5,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 2,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "Interactive whiteboard supporting multi-touch."
            },
            new Equipment
            {
                EquipId = 3,
                EquipCode = "EQ-BRD-003",
                EquipName = "Magnetic Whiteboard",
                EquipCategory = "Board Equipment",
                EquipSupplier = "Local Board Supplier",
                EquipUnitPrice = 2500000,
                EquipQuantity = 0,
                EquipMinStock = 3,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 2,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "Standard magnetic whiteboard for writing."
            },
            new Equipment
            {
                EquipId = 4,
                EquipCode = "EQ-AUD-004",
                EquipName = "Classroom Audio System",
                EquipCategory = "Audio Equipment",
                EquipSupplier = "Bose Vietnam",
                EquipUnitPrice = 5000000,
                EquipQuantity = 9,
                EquipMinStock = 4,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 3,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "Premium audio system for spacious rooms."
            },
            new Equipment
            {
                EquipId = 5,
                EquipCode = "EQ-FUR-005",
                EquipName = "Student Desk",
                EquipCategory = "Furniture",
                EquipSupplier = "Local Furniture Supplier",
                EquipUnitPrice = 1200000,
                EquipQuantity = 2,
                EquipMinStock = 6,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 4,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "Durable wooden desk for students."
            },
            new Equipment
            {
                EquipId = 6,
                EquipCode = "EQ-FUR-006",
                EquipName = "Student Chair",
                EquipCategory = "Furniture",
                EquipSupplier = "Local Furniture Supplier",
                EquipUnitPrice = 800000,
                EquipQuantity = 7,
                EquipMinStock = 3,
                EquipLastUpdatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                CategoryId = 4,
                CreatedAt = new DateTime(2025, 5, 9, 9, 12, 0),
                IsDeleted = false,
                Description = "Ergonomic chair matching student desks."
            }
        );
    }
}