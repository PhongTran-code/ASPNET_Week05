using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class OrderCreateViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập tên khách hàng")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng chọn thiết bị")]
    public int EquipmentId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số lượng")]
    [Range(1, 1000, ErrorMessage = "Số lượng phải từ 1 trở lên")]
    public int Quantity { get; set; }

    public List<SelectListItem>? AvailableEquipments { get; set; }
}