using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.Models;
using MiniEquipmentInventory.Mvc.Settings;
using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Services;

public class EquipmentService : IEquipmentService
{
    private readonly AppSettings _settings;
    private readonly AppDbContext _context;
    private readonly ILogger<EquipmentService> _logger;

    public EquipmentService(
        IOptions<AppSettings> options,
        AppDbContext context,
        ILogger<EquipmentService> logger)
    {
        _settings = options.Value;
        _context = context;
        _logger = logger;
    }

    public async Task<List<EquipmentListItemViewModel>> GetActiveEquipmentsAsync()
    {
        return await _context.Equipments
            .Include(e => e.Category)
            .AsNoTracking()
            .OrderBy(e => e.EquipId)
            .Select(e => new EquipmentListItemViewModel
            {
                EquipId = e.EquipId,
                EquipCode = e.EquipCode,
                EquipName = e.EquipName,
                EquipUnitPrice = e.EquipUnitPrice,
                EquipQuantity = e.EquipQuantity,
                EquipMinStock = e.EquipMinStock,
                EquipCategory = e.Category != null ? e.Category.Name : "N/A",
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<EquipmentDetailViewModel?> GetDetailAsync(int id)
    {
        var equipment = await _context.Equipments
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.EquipId == id);

        if (equipment == null)
        {
            _logger.LogWarning("Không tìm thấy thiết bị với ID: {Id}", id);
            return null;
        }

        int threshold = _settings.LowStockThreshold;

        return new EquipmentDetailViewModel
        {
            EquipId = equipment.EquipId,
            EquipCode = equipment.EquipCode,
            EquipName = equipment.EquipName,
            EquipCategory = equipment.Category != null ? equipment.Category.Name : "N/A",
            EquipSupplier = equipment.EquipSupplier,
            PriceText = equipment.EquipUnitPrice.ToString("N0") + " VND",
            EquipQuantity = equipment.EquipQuantity,
            EquipMinStock = equipment.EquipMinStock,
            InventoryValueText = (equipment.EquipUnitPrice * equipment.EquipQuantity).ToString("N0") + " VND",
            StockStatus = equipment.EquipQuantity == 0 ? "Hết hàng" : (equipment.EquipQuantity <= threshold ? $"Sắp hết hàng (Ngưỡng: {threshold})" : "Còn hàng"),
            LastUpdatedText = equipment.EquipLastUpdatedAt.ToString("dd/MM/yyyy HH:mm")
        };
    }

    public async Task CreateAsync(EquipmentCreateViewModel model)
    {
        var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == model.EquipCategory);
        var equipment = new Equipment
        {
            EquipName = model.EquipName,
            EquipCode = model.EquipCode,
            EquipUnitPrice = model.EquipUnitPrice,
            EquipQuantity = model.EquipQuantity,
            Description = model.Description,
            CategoryId = category?.Id ?? 1,
            EquipCategory = model.EquipCategory,
            EquipSupplier = model.EquipSupplier,
            EquipMinStock = model.EquipMinStock,
            CreatedAt = DateTime.Now,
            EquipLastUpdatedAt = DateTime.Now
        };
        await _context.Equipments.AddAsync(equipment);
        await _context.SaveChangesAsync();
        _logger.LogInformation("Equipment created. EquipmentId={EquipmentId}, SKU={SKU}", equipment.EquipId, equipment.EquipCode);

        var log = new AuditLog
        {
            Time = DateTime.Now,
            Level = "Information",
            Message = $"Equipment created. EquipmentId={equipment.EquipId}"
        };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> SoftDeleteAsync(int id)
    {
        var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipId == id);
        if (equipment == null) return false;

        equipment.IsDeleted = true;
        equipment.DeletedAt = DateTime.Now;
        equipment.UpdatedAt = DateTime.Now;
        equipment.EquipLastUpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        _logger.LogWarning("Equipment soft deleted. EquipmentId={EquipmentId}", id);

        var log = new AuditLog
        {
            Time = DateTime.Now,
            Level = "Warning",
            Message = $"Equipment soft deleted. EquipmentId={id}"
        };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<EquipmentTrashItemViewModel>> GetTrashAsync()
    {
        return await _context.Equipments
            .IgnoreQueryFilters()
            .Where(e => e.IsDeleted)
            .AsNoTracking()
            .Select(e => new EquipmentTrashItemViewModel
            {
                EquipId = e.EquipId,
                EquipName = e.EquipName,
                DeletedAt = e.DeletedAt,
                DeletedBy = "admin"
            })
            .ToListAsync();
    }

    public async Task<bool> RestoreAsync(int id)
    {
        var equipment = await _context.Equipments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(e => e.EquipId == id && e.IsDeleted);

        if (equipment == null) return false;

        equipment.IsDeleted = false;
        equipment.DeletedAt = null;
        equipment.UpdatedAt = DateTime.Now;
        equipment.EquipLastUpdatedAt = DateTime.Now;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Equipment restored. EquipmentId={EquipmentId}", id);

        var log = new AuditLog
        {
            Time = DateTime.Now,
            Level = "Information",
            Message = $"Equipment restored. EquipmentId={id}"
        };
        await _context.AuditLogs.AddAsync(log);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<List<EquipmentListItemViewModel>> SearchEquipmentsAsync(string? keyword, string? stockStatus)
    {
        var query = _context.Equipments
            .Include(e => e.Category)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var cleanKeyword = keyword.Trim();
            query = query.Where(e => e.EquipName.Contains(cleanKeyword) || 
                                     e.EquipCode.Contains(cleanKeyword) || 
                                     (e.Description != null && e.Description.Contains(cleanKeyword)));
        }

        if (!string.IsNullOrWhiteSpace(stockStatus) && stockStatus != "all")
        {
            int threshold = _settings.LowStockThreshold;
            query = stockStatus switch
            {
                "outOfStock" => query.Where(e => e.EquipQuantity == 0),
                "lowStock" => query.Where(e => e.EquipQuantity > 0 && e.EquipQuantity <= threshold),
                "inStock" => query.Where(e => e.EquipQuantity > threshold),
                _ => query
            };
        }

        return await query
            .OrderBy(e => e.EquipId)
            .Select(e => new EquipmentListItemViewModel
            {
                EquipId = e.EquipId,
                EquipCode = e.EquipCode,
                EquipName = e.EquipName,
                EquipUnitPrice = e.EquipUnitPrice,
                EquipQuantity = e.EquipQuantity,
                EquipMinStock = e.EquipMinStock,
                EquipCategory = e.Category != null ? e.Category.Name : "N/A",
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();
    }
}
