using System;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentListItemViewModel
{
    public int EquipId { get; set; }
    public string EquipName { get; set; } = "";
    public string EquipCategory { get; set; } = "";
    public string EquipSupplier { get; set; } = "";
    public decimal EquipUnitPrice { get; set; }
    public int EquipQuantity { get; set; }
    public int EquipMinStock { get; set; }
    public string EquipCode { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public string PriceText => $"{EquipUnitPrice:N0} VND";
    public decimal InventoryValue => EquipUnitPrice * EquipQuantity;
    public string InventoryValueText => $"{InventoryValue:N0} VND";

    public string StockStatus
    {
        get
        {
            if (EquipQuantity <= 0)
            {
                return "Hết hàng";
            }
            if (EquipQuantity <= EquipMinStock)
            {
                return "Cần nhập thêm";
            }
            if (EquipQuantity >= 20)
            {
                return "Tồn kho cao";
            }
            return "Còn hàng";
        }
    }

    public string StockStatusClass
    {
        get
        {
            if (EquipQuantity <= 0)
            {
                return "badge badge-danger";
            }
            if (EquipQuantity <= EquipMinStock)
            {
                return "badge badge-warning";
            }
            if (EquipQuantity >= 20)
            {
                return "badge badge-info";
            }
            return "badge badge-success";
        }
    }
}
