using System.ComponentModel.DataAnnotations;

namespace MiniEquipmentInventory.Mvc.Models;

public class Equipment
{
    public int EquipId { get; set; }
    public string EquipCode { get; set; } = "";
    public string EquipName { get; set; } = "";
    public string EquipCategory { get; set; } = "";
    public string EquipSupplier { get; set; } = "";
    public decimal EquipUnitPrice { get; set; }
    public int EquipQuantity { get; set; }
    public int EquipMinStock { get; set; }
    public DateTime EquipLastUpdatedAt { get; set; }
    
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
}
