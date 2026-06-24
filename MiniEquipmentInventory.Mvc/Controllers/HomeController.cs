using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.Models;
using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(AppDbContext context, ILogger<HomeController> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var totalEquipment = await _context.Equipments.IgnoreQueryFilters().CountAsync();
        
        var activeEquipment = await _context.Equipments.CountAsync();
        
        var deletedEquipment = await _context.Equipments.IgnoreQueryFilters().CountAsync(e => e.IsDeleted);
        
        var today = DateTime.Today;
        var logsToday = await _context.AuditLogs.CountAsync(l => l.Time >= today);

        var model = new DashboardViewModel
        {
            TotalEquipment = totalEquipment,
            ActiveEquipment = activeEquipment,
            DeletedEquipment = deletedEquipment,
            LogsToday = logsToday
        };

        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    [Route("Home/StatusCode")]
    public new IActionResult StatusCode(int code)
    {
        ViewData["StatusCode"] = code;
        ViewData["Message"] = code switch
        {
            404 => "Rất tiếc, trang bạn yêu cầu không tồn tại.",
            403 => "Bạn không có quyền truy cập trang này.",
            500 => "Lỗi hệ thống nghiêm trọng.",
            _ => $"Đã xảy ra lỗi hệ thống (Mã lỗi: {code})."
        };
        return View();
    }
}
