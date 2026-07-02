using SalesXZone.Application.Models;
using System.Threading;
using System.Threading.Tasks;

namespace SalesXZone.Application.Interfaces
{
    public interface IItemRepository
    {
        Task<int> AddItemAsync(ItemCreateRequest dto, CancellationToken cancellationToken = default);
        Task<ItemMasterModel?> GetItemByIdAsync(int itemId, CancellationToken cancellationToken = default);
    }
}
