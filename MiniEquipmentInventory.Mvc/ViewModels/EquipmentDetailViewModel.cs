namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentDetailViewModel
{
    public int EquipId { get; set; }
    public string EquipCode { get; set; } = string.Empty;
    public string EquipName { get; set; } = string.Empty;
    public string EquipCategory { get; set; } = string.Empty;
    public string EquipSupplier { get; set; } = string.Empty;
    public string PriceText { get; set; } = string.Empty;
    public int EquipQuantity { get; set; }
    public int EquipMinStock { get; set; }
    public string InventoryValueText { get; set; } = string.Empty;
    public string StockStatus { get; set; } = string.Empty;
    public string LastUpdatedText { get; set; } = string.Empty;
}