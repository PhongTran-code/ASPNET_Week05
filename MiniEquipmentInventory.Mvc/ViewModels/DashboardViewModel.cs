namespace MiniEquipmentInventory.Mvc.ViewModels;

public class DashboardViewModel
{
    public int TotalEquipment { get; set; }
    public int ActiveEquipment { get; set; }
    public int DeletedEquipment { get; set; }
    public int LogsToday { get; set; }
}
