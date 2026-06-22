using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Controllers;

public class CategoryController : Controller
{
    private readonly AppDbContext _context;

    public CategoryController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        // Lấy toàn bộ dữ liệu kèm theo danh sách thiết bị
        var categoriesData = await _context.Categories
            .Include(c => c.Equipments)
            .AsNoTracking()
            .ToListAsync();

        var categories = categoriesData.Select(c => new CategoryListItemViewModel
        {
            Id = c.Id,
            Name = c.Name,
            EquipmentCount = c.Equipments.Count,
            EquipmentsList = string.Join(", ", c.Equipments.Select(e => e.EquipName)),
            Relationship = "1 - Many", // 1 danh mục chứa nhiều thiết bị
            DbSet = "Categories",
            ActionState = "Read-only"
        }).ToList();

        return View(categories);
    }
}