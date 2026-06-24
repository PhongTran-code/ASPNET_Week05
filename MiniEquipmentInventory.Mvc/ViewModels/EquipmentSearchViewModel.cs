using System.Collections.Generic;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentSearchViewModel
{
    public string? Keyword { get; set; }
    public string? StockStatus { get; set; }
    public List<EquipmentListItemViewModel> Results { get; set; } = new();
}
