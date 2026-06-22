using System;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentTrashItemViewModel
{
    public int EquipId { get; set; }
    public string EquipName { get; set; } = string.Empty;
    public DateTime? DeletedAt { get; set; }
    public string DeletedBy { get; set; } = string.Empty;
}
