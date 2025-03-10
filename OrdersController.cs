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
            // Initialize the order repository, throw an exception if null
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("GetAll method called.");

            // Retrieve all orders from the repository
            var orders = _orderRepository.GetAll().ToList();

            // Check if there are no orders and return NotFound if true
            if (!orders.Any())
            {
                Console.WriteLine("No orders found.");
                return NotFound();
            }

            Console.WriteLine($"Returning {orders.Count} orders.");
            return Ok(orders);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            Console.WriteLine("Create method called.");

            // Validate the incoming order object
            if (order == null)
            {
                Console.WriteLine("Invalid order data received.");
                return BadRequest();
            }

            // Create a new order object with the current timestamp
            var newOrder = new Order
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                CreatedAt = DateTime.Now
            };

            try
            {
                // Add the new order to the repository
                _orderRepository.Add(newOrder);
                Console.WriteLine($"Order created with ID: {newOrder.Id}");
                return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                // Log the exception and return a 500 status code
                Console.WriteLine($"Error occurred while creating order: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine($"Delete method called with ID: {id}");

            // Validate the ID
            if (id <= 0)
            {
                Console.WriteLine("Invalid ID received.");
                return BadRequest("Invalid ID");
            }

            try
            {
                // Attempt to delete the order
                _orderRepository.Delete(id);

                // Verify if the order was successfully deleted
                if (!_orderRepository.GetAll().Any(o => o.Id == id))
                {
                    Console.WriteLine($"Order with ID: {id} successfully deleted.");
                    return NoContent();
                }
                else
                {
                    Console.WriteLine($"Failed to delete order with ID: {id}");
                    return StatusCode(500, "Deletion failed");
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow it
                Console.WriteLine($"Error occurred while deleting order: {ex.Message}");
                throw;
            }
        }
    }
}