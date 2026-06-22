using MiniEquipmentInventory.Mvc.Models;
using MiniEquipmentInventory.Mvc.Services;
using MiniEquipmentInventory.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using MiniEquipmentInventory.Mvc.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MiniEquipmentInventory.Mvc.Controllers;

public class EquipmentController : Controller
{
    private readonly IEquipmentService _equipmentService;
    private readonly AppDbContext _context;
    private readonly ILogger<EquipmentController> _logger;

    public EquipmentController(
        IEquipmentService equipmentService,
        AppDbContext context,
        ILogger<EquipmentController> logger)
    {
        _equipmentService = equipmentService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var equipment = await _equipmentService.GetActiveEquipmentsAsync();
        return View(equipment);
    }

    public async Task<IActionResult> Detail(int id)
    {
        var detailViewModel = await _equipmentService.GetDetailAsync(id);
        if (detailViewModel == null)
        {
            return NotFound("Không tìm thấy thiết bị này.");
        }
        return View(detailViewModel);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EquipmentCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var exists = await _context.Equipments
            .IgnoreQueryFilters()
            .AnyAsync(e => e.EquipCode == model.EquipCode);

        if (exists)
        {
            ModelState.AddModelError(nameof(model.EquipCode), "SKU này đã tồn tại.");
            return View(model);
        }

        await _equipmentService.CreateAsync(model);

        TempData["SuccessMessage"] = "Đã thêm thiết bị thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipId == id);
        if (equipment == null) return NotFound();

        var model = new EquipmentEditViewModel
        {
            EquipId = equipment.EquipId,
            EquipName = equipment.EquipName,
            EquipCode = equipment.EquipCode,
            EquipUnitPrice = equipment.EquipUnitPrice,
            EquipQuantity = equipment.EquipQuantity,
            Description = equipment.Description,
            RowVersion = Convert.ToBase64String(equipment.RowVersion)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EquipmentEditViewModel model)
    {
        if (id != model.EquipId) return NotFound();
        if (!ModelState.IsValid) return View(model);

        var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipId == id);
        if (equipment == null) return NotFound();

        equipment.EquipName = model.EquipName;
        equipment.EquipCode = model.EquipCode;
        equipment.EquipUnitPrice = model.EquipUnitPrice;
        equipment.EquipQuantity = model.EquipQuantity;
        equipment.Description = model.Description;
        equipment.UpdatedAt = DateTime.Now;
        equipment.EquipLastUpdatedAt = DateTime.Now;

        _context.Entry(equipment).Property(e => e.RowVersion).OriginalValue =
            Convert.FromBase64String(model.RowVersion);

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Equipment updated. EquipmentId={EquipmentId}", id);

            var log = new AuditLog
            {
                Time = DateTime.Now,
                Level = "Information",
                Message = $"Equipment updated. EquipmentId={id}"
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật thiết bị thành công.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateConcurrencyException)
        {
            ModelState.AddModelError(string.Empty,
                "Dữ liệu đã được người khác cập nhật. Vui lòng tải lại trang và thử lại.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var detailViewModel = await _equipmentService.GetDetailAsync(id);
        if (detailViewModel == null)
        {
            return NotFound("Không tìm thấy thiết bị này.");
        }
        return View(detailViewModel);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var success = await _equipmentService.SoftDeleteAsync(id);
        if (!success) return NotFound();

        TempData["SuccessMessage"] = "Đã xóa mềm thiết bị thành công.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Trash()
    {
        var deletedEquipments = await _equipmentService.GetTrashAsync();
        return View(deletedEquipments);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(int id)
    {
        var success = await _equipmentService.RestoreAsync(id);
        if (!success) return NotFound();
        
        TempData["SuccessMessage"] = "Khôi phục thiết bị thành công.";
        return RedirectToAction(nameof(Trash));
    }
}
