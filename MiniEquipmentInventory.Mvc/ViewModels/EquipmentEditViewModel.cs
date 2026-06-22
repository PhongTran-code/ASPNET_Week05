using System.ComponentModel.DataAnnotations;

namespace MiniEquipmentInventory.Mvc.ViewModels;

public class EquipmentEditViewModel : EquipmentCreateViewModel
{
    [Required]
    public int EquipId { get; set; }

    public string RowVersion { get; set; } = string.Empty;
}
