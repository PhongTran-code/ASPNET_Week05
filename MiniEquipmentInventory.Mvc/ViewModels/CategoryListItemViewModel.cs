namespace MiniEquipmentInventory.Mvc.ViewModels;

public class CategoryListItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EquipmentCount { get; set; }
    public string EquipmentsList { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string DbSet { get; set; } = string.Empty;
    public string ActionState { get; set; } = string.Empty;
}