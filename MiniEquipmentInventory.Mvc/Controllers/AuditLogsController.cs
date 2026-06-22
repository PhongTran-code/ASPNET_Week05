using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using System.Threading.Tasks;

namespace MiniEquipmentInventory.Mvc.Controllers;

public class AuditLogsController : Controller
{
    private readonly AppDbContext _context;

    public AuditLogsController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var logs = await _context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(l => l.Time)
            .ToListAsync();

        return View(logs);
    }
}
