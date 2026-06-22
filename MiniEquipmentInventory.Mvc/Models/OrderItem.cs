namespace MiniEquipmentInventory.Mvc.Models;

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}