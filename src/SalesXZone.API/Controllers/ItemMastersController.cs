using Microsoft.AspNetCore.Mvc;
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
                if (request == null) return BadRequest(new { message = "Request body required" });

                var created = await _itemService.CreateItemAsync(request, cancellationToken).ConfigureAwait(false);
                return CreatedAtAction(nameof(GetById), new { id = created.ItemId }, created);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation failed");
                return BadRequest(new { message = ex.Message });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error while adding item");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding item");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _itemService.GetItemByIdAsync(id, cancellationToken).ConfigureAwait(false);
            if (item == null) return NotFound(new { message = "Item not found" });
            return Ok(item);
        }
    }
}