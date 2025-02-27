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
            // Initialize the order repository, throw exception if null
            _iOrderRepo = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("GetAll method called.");

            // Retrieve all orders from the repository
            var orders = _iOrderRepo.GetAll().ToList();
            Console.WriteLine($"Retrieved {orders.Count} orders from repository.");

            // Map orders to a result list
            var resultList = orders.Select(order => new Order
            {
                Id = order.Id,
                CustomerName = order.CustomerName,
                Amount = order.Amount,
                CreatedAt = order.CreatedAt
            }).ToList();

            // Check if the result list is empty
            if (!resultList.Any())
            {
                Console.WriteLine("No orders found.");
                return NotFound();
            }

            Console.WriteLine("Returning orders.");
            return Ok(resultList);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            Console.WriteLine("Create method called.");

            // Validate the incoming order
            if (order == null)
            {
                Console.WriteLine("Invalid order data.");
                return BadRequest();
            }

            // Create a new order object with current timestamp
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
                _iOrderRepo.Add(newOrder);
                Console.WriteLine($"Order Created: {newOrder.Id}");

                // Return the created order with a 201 status code
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
            Console.WriteLine($"Delete method called with ID: {id}");

            // Validate the ID
            if (id <= 0)
            {
                Console.WriteLine("Invalid ID provided.");
                return BadRequest("Invalid ID");
            }

            try
            {
                // Attempt to delete the order
                _iOrderRepo.Delete(id);
                Console.WriteLine($"Order with ID: {id} deleted.");

                // Verify if the order still exists
                if (!_iOrderRepo.GetAll().Any(o => o.Id == id))
                {
                    Console.WriteLine("Order successfully deleted.");
                    return NoContent();
                }
                else
                {
                    Console.WriteLine("Deletion failed.");
                    return StatusCode(500, "Deletion failed");
                }
            }
            catch (Exception ex)
            {
                // Log the exception message
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
        }
    }
}