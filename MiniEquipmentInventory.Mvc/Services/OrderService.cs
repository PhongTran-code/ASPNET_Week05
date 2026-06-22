using Microsoft.EntityFrameworkCore;
using MiniEquipmentInventory.Mvc.Data;
using MiniEquipmentInventory.Mvc.Models;
using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateOrderAsync(OrderCreateViewModel model)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var equipment = await _context.Equipments.FirstOrDefaultAsync(e => e.EquipId == model.EquipmentId);
            if (equipment == null) throw new Exception("Không tìm thấy thiết bị");
            if (equipment.EquipQuantity < model.Quantity) throw new Exception("Số lượng tồn kho không đủ");

            var order = new Order
            {
                CustomerName = model.CustomerName,
                CreatedAt = DateTime.Now, 
                TotalAmount = equipment.EquipUnitPrice * model.Quantity
            };
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var item = new OrderItem
            {
                OrderId = order.Id,
                EquipmentId = equipment.EquipId,
                Quantity = model.Quantity,
                UnitPrice = equipment.EquipUnitPrice
            };
            _context.OrderItems.Add(item);
            equipment.EquipQuantity -= model.Quantity;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}