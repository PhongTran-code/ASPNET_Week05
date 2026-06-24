using MiniEquipmentInventory.Mvc.ViewModels;

namespace MiniEquipmentInventory.Mvc.Services;

public interface IEquipmentService
{
    Task<List<EquipmentListItemViewModel>> GetActiveEquipmentsAsync();
    Task<EquipmentDetailViewModel?> GetDetailAsync(int id);
    Task CreateAsync(EquipmentCreateViewModel model);
    Task<bool> SoftDeleteAsync(int id);
    Task<List<EquipmentTrashItemViewModel>> GetTrashAsync();
    Task<bool> RestoreAsync(int id);
    Task<List<EquipmentListItemViewModel>> SearchEquipmentsAsync(string? keyword, string? stockStatus);
}