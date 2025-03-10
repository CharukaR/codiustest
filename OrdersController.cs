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
        private readonly IOrderRepository _iOrderRepo;

        public OrdersController(IOrderRepository orderRepository)
        {
            _iOrderRepo = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("GetAll method called.");

            var orders = _iOrderRepo.GetAll().ToList();
            Console.WriteLine($"Number of orders retrieved: {orders.Count}");

            if (!orders.Any())
            {
                Console.WriteLine("No orders found.");
                return NotFound();
            }

            Console.WriteLine("Orders retrieved successfully.");
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            Console.WriteLine("Create method called.");

            if (order == null)
            {
                Console.WriteLine("Bad request: Order is null.");
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
                _iOrderRepo.Add(newOrder);
                Console.WriteLine($"Order Created: {JsonSerializer.Serialize(newOrder)}");

                return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating order: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine($"Delete method called with ID: {id}");

            if (id <= 0)
            {
                Console.WriteLine("Bad request: Invalid ID.");
                return BadRequest("Invalid ID");
            }

            try
            {
                _iOrderRepo.Delete(id);
                Console.WriteLine($"Order with ID {id} deleted.");

                if (!_iOrderRepo.GetAll().Any(o => o.Id == id))
                {
                    Console.WriteLine("Order deletion confirmed.");
                    return NoContent();
                }
                else
                {
                    Console.WriteLine("Deletion failed: Order still exists.");
                    return StatusCode(500, "Deletion failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting order: {ex.Message}");
                throw;
            }
        }
    }
}