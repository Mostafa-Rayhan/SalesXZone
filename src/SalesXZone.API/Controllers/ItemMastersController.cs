using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using SalesXZone.Application.Interfaces;
using SalesXZone.Application.Models;

namespace SalesXZone.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemMastersController : ControllerBase
    {
        private readonly IItemMasterService _itemService;
        private readonly ILogger<ItemMastersController> _logger;

        public ItemMastersController(IItemMasterService itemService, ILogger<ItemMastersController> logger)
        {
            _itemService = itemService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ItemCreateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                    return BadRequest(ApiResponse<object>.Fail("Request body required", "4000"));

                var created = await _itemService.CreateItemAsync(request, cancellationToken).ConfigureAwait(false);

                return CreatedAtAction(nameof(GetById), new { id = created.ItemId },
                    ApiResponse<ItemMasterModel>.Success(created, "Item Created Success", "2001"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                return BadRequest(ApiResponse<object>.Fail(ex.Message, "4000"));
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while adding item");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail("Database error", "5000"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding item");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail("Unexpected error", "5000"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool activeOnly = false, CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _itemService.GetItemsAsync(activeOnly, cancellationToken).ConfigureAwait(false);

                return Ok(ApiResponse<List<ItemMasterModel>>.Success(items, "Get Report Success", "2000"));
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while fetching items");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    ApiResponse<object>.Fail("Database error", "5000"));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _itemService.GetItemByIdAsync(id, cancellationToken).ConfigureAwait(false);

            if (item == null)
                return NotFound(ApiResponse<object>.Fail("Item not found", "4004"));

            return Ok(ApiResponse<ItemMasterModel>.Success(item, "Get Report Success", "2000"));
        }
    }
}
