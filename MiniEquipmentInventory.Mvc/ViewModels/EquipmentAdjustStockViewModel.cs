using System.ComponentModel.DataAnnotations;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentAdjustStockViewModel
{
    [Required]
    public int EquipId { get; set; }

    [Display(Name = "Tên thiết bị")]
    public string EquipName { get; set; } = string.Empty;

    [Display(Name = "SKU")]
    public string EquipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc.")]
    [Range(0, 100000, ErrorMessage = "Số lượng tồn kho phải từ 0 đến 100.000.")]
    [Display(Name = "Số lượng tồn kho mới")]
    public int EquipQuantity { get; set; }

    [Required]
    public string RowVersion { get; set; } = string.Empty;
}
