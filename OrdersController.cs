using Microsoft.AspNetCore.Mvc;
using OrderTaskApi.Models;
using OrderTaskApi.Repositories;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace OrderTaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("GetAll method called.");

            var orders = _orderRepository.GetAll().ToList();

            if (!orders.Any())
            {
                _logger.LogWarning("No orders found.");
                return NotFound();
            }

            var resultList = orders.Select(order => new Order
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                CreatedAt = order.CreatedAt
            }).ToList();

            _logger.LogInformation("Returning {Count} orders.", resultList.Count);
            return Ok(resultList);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            _logger.LogInformation("Create method called with order: {Order}", JsonSerializer.Serialize(order));

            if (order == null)
            {
                _logger.LogWarning("Order is null.");
                return BadRequest();
            }

            var newOrder = new Order
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                CreatedAt = DateTime.Now
            };

            try
            {
                _orderRepository.Add(newOrder);
                _logger.LogInformation("Order created successfully with ID: {OrderId}", newOrder.Id);
                return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order.");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Delete method called with ID: {Id}", id);

            if (id <= 0)
            {
                _logger.LogWarning("Invalid ID: {Id}", id);
                return BadRequest("Invalid ID");
            }

            try
            {
                _orderRepository.Delete(id);
                _logger.LogInformation("Order with ID: {Id} deleted.", id);

                if (!_orderRepository.GetAll().Any(o => o.Id == id))
                {
                    _logger.LogInformation("Order with ID: {Id} successfully deleted.", id);
                    return NoContent();
                }
                else
                {
                    _logger.LogError("Deletion failed for order with ID: {Id}", id);
                    return StatusCode(500, "Deletion failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting order with ID: {Id}", id);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}