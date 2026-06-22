using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.Services;
using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Controllers;

public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly AppDbContext _context;

    public OrdersController(IOrderService orderService, AppDbContext context)
    {
        _orderService = orderService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var viewModel = new OrderCreateViewModel { AvailableEquipments = await GetEquipmentListAsync() };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrderCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableEquipments = await GetEquipmentListAsync();
            return View(model);
        }

        try
        {
            await _orderService.CreateOrderAsync(model);
            TempData["SuccessMessage"] = "Đã tạo đơn hàng thành công và trừ tồn kho!";
            return RedirectToAction("Create");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message); // Bắt lỗi "Số lượng không đủ" từ Service
            model.AvailableEquipments = await GetEquipmentListAsync();
            return View(model);
        }
    }

    private async Task<List<SelectListItem>> GetEquipmentListAsync()
    {
        var equipments = await _context.Equipments.AsNoTracking().Where(e => e.EquipQuantity > 0).ToListAsync();
        return equipments.Select(e => new SelectListItem { Value = e.EquipId.ToString(), Text = $"{e.EquipName} (Còn {e.EquipQuantity} - {e.EquipUnitPrice:N0} VND)" }).ToList();
    }
}
