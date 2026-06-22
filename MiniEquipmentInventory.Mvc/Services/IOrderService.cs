using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Services;

public interface IOrderService
{
    Task CreateOrderAsync(OrderCreateViewModel model);
}