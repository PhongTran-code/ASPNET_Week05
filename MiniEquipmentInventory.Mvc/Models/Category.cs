namespace MiniEquipmentInventory.Mvc.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Equipment> Equipments { get; set; } = new List<Equipment>();
}