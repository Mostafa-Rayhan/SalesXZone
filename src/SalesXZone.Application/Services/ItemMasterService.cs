using SalesXZone.Application.Interfaces;
using SalesXZone.Application.Models;

namespace SalesXZone.Application.Services
{
    public class ItemMasterService : IItemMasterService
    {
        private readonly IItemRepository _repo;

        public ItemMasterService(IItemRepository repo)
        {
            _repo = repo;
        }

        public async Task<ItemMasterModel> CreateItemAsync(ItemCreateRequest request, CancellationToken cancellationToken = default)
        {
            // Basic server-side validation (additional validations can be added)
            if (string.IsNullOrWhiteSpace(request.ItemName))
                throw new ArgumentException("ItemName is required.", nameof(request.ItemName));

            if (request.WholesellPrice.HasValue && request.MrpPrice.HasValue && request.MrpPrice < request.WholesellPrice)
                throw new ArgumentException("MRP price cannot be less than wholesale price.");

            // add item via stored procedure
            var newId = await _repo.AddItemAsync(request, cancellationToken).ConfigureAwait(false);

            // return the created item
            var created = await _repo.GetItemByIdAsync(newId, cancellationToken).ConfigureAwait(false);
            if (created == null)
                throw new InvalidOperationException("Item created but could not fetch created item.");

            return created;
        }

        public async Task<ItemMasterModel?> GetItemByIdAsync(int itemId, CancellationToken cancellationToken = default)
        {
            return await _repo.GetItemByIdAsync(itemId, cancellationToken).ConfigureAwait(false);
        }
    }
}