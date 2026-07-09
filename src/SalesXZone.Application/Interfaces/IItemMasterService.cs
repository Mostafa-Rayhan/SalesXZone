using SalesXZone.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SalesXZone.Application.Interfaces
{
    public interface IItemMasterService
    {
        Task<ItemMasterModel> CreateItemAsync(ItemCreateRequest request, CancellationToken cancellationToken = default);
        Task<ItemMasterModel?> GetItemByIdAsync(int itemId, CancellationToken cancellationToken = default);
        Task<List<ItemMasterModel>> GetItemsAsync(bool activeOnly = false, CancellationToken cancellationToken = default);
    }
}
