using System;

namespace MiniEquipmentInventory.Mvc.Models;

public class AuditLog
{
    public int Id { get; set; }
    public DateTime Time { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
