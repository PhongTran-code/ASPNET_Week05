using System.ComponentModel.DataAnnotations;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentCreateViewModel
{
    [Required(ErrorMessage = "Tên thiết bị là bắt buộc.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Tên thiết bị phải từ 3 đến 100 ký tự.")]
    public string EquipName { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU là bắt buộc.")]
    [RegularExpression(@"^[A-Z0-9\-]+$", ErrorMessage = "SKU chỉ gồm chữ in hoa, số và dấu -.")]
    public string EquipCode { get; set; } = string.Empty;

    [Range(1000, 100000000, ErrorMessage = "Giá phải từ 1.000 đến 100.000.000.")]
    public decimal EquipUnitPrice { get; set; }

    [Range(0, 100000, ErrorMessage = "Tồn kho phải từ 0 đến 100.000.")]
    public int EquipQuantity { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc.")]
    public string EquipCategory { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhà cung cấp là bắt buộc.")]
    public string EquipSupplier { get; set; } = string.Empty;

    [Range(1, 1000, ErrorMessage = "Mức tồn tối thiểu phải từ 1 đến 1.000.")]
    public int EquipMinStock { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự.")]
    public string? Description { get; set; }

}
