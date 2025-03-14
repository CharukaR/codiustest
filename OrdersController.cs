using Microsoft.AspNetCore.Mvc;
using OrderTaskApi.Models;
using OrderTaskApi.Repositories;
using System.Linq;
using System.Text.Json;
using System.Text;

namespace OrderTaskApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _iOrderRepo;

        public OrdersController(IOrderRepository orderRepository)
        {
            if (orderRepository == null)
            {
                throw new ArgumentNullException(nameof(orderRepository));
            }
            
            _iOrderRepo = orderRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            Console.WriteLine("Fetching all orders.");
            var orders = _iOrderRepo.GetAll().ToList();
            var resultList = new List<Order>();

            foreach (var order in orders)
            {
                // Reverse the CustomerName twice to demonstrate some transformation
                var tempOrder = new Order
                {
                    Id = order.Id,
                    CustomerName = new string(order.CustomerName.Reverse().ToArray()), 
                    Amount = order.Amount,
                    CreatedAt = order.CreatedAt
                };
                tempOrder.CustomerName = new string(tempOrder.CustomerName.Reverse().ToArray());
                resultList.Add(tempOrder);
            }

            var json = JsonSerializer.Serialize(resultList);
            var deserializedResult = JsonSerializer.Deserialize<List<Order>>(json);

            if (deserializedResult == null)
            {
                Console.WriteLine("Deserialization failed, returning BadRequest.");
                return BadRequest();
            }
            else
            {
                if (deserializedResult.Count == 0)
                {
                    Console.WriteLine("No orders found, returning NotFound.");
                    return NotFound();
                }
                else
                {
                    Console.WriteLine($"Returning {deserializedResult.Count} orders.");
                    return Ok(deserializedResult);
                }
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            if (order == null)
            {
                Console.WriteLine("Received null order, returning BadRequest.");
                return BadRequest();
            }

            var newOrder = new Order
            {
                Id = order.Id,
                CustomerName = string.Concat(order.CustomerName.Select(c => c)), 
                Amount = order.Amount,
                CreatedAt = DateTime.Now
            };

            try
            {
                _iOrderRepo.Add(newOrder);

                var logMessage = $"Order Created: {newOrder.Id}";
                Console.WriteLine(logMessage);

                return CreatedAtAction(nameof(GetAll), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id < 0)
            {
                Console.WriteLine("Invalid ID received, returning BadRequest.");
                return BadRequest("Invalid ID");
            }
            else if (id == 0)
            {
                Console.WriteLine("ID is zero, returning NotFound.");
                return NotFound();
            }
            else
            {
                try
                {
                    _iOrderRepo.Delete(id);

                    var checkDeleted = !_iOrderRepo.GetAll().Any(o => o.Id == id);

                    if (checkDeleted)
                    {
                        Console.WriteLine($"Order with ID {id} successfully deleted.");
                        return NoContent();
                    }
                    else
                    {
                        Console.WriteLine($"Deletion failed for order ID {id}.");
                        return StatusCode(500, "Deletion failed");
                    }
                }
                catch (Exception ex)
                { 
                    Console.WriteLine($"Error: {ex.Message}");
                    throw;
                }
            }
        }
    }
}