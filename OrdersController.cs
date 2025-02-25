using Microsoft.AspNetCore.Mvc;
using OrderTaskApi.Models;
using OrderTaskApi.Repositories;
using System.Linq;
using System.Text.Json;

namespace OrderTaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;

        public OrdersController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            // Log the start of the GetAll method
            Console.WriteLine("Executing GetAll method.");

            var orders = _orderRepository.GetAll().ToList();

            if (!orders.Any())
            {
                // Log if no orders are found
                Console.WriteLine("No orders found.");
                return NotFound();
            }

            var resultList = orders.Select(order => new Order
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                CreatedAt = order.CreatedAt
            }).ToList();

            // Log the number of orders retrieved
            Console.WriteLine($"Retrieved {resultList.Count} orders.");
            return Ok(resultList);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            // Log the start of the Create method
            Console.WriteLine("Executing Create method.");

            if (order == null)
            {
                // Log if the order object is null
                Console.WriteLine("Order object is null.");
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
                // Log the successful creation of an order
                Console.WriteLine($"Order Created: {newOrder.Id}");
                return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Log the start of the Delete method
            Console.WriteLine($"Executing Delete method for order ID: {id}");

            if (id <= 0)
            {
                // Log if the provided ID is invalid
                Console.WriteLine("Invalid ID provided.");
                return BadRequest("Invalid ID");
            }

            try
            {
                _orderRepository.Delete(id);

                if (!_orderRepository.GetAll().Any(o => o.Id == id))
                {
                    // Log successful deletion
                    Console.WriteLine($"Order with ID: {id} successfully deleted.");
                    return NoContent();
                }
                else
                {
                    // Log if deletion failed
                    Console.WriteLine($"Deletion failed for order ID: {id}");
                    return StatusCode(500, "Deletion failed");
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}