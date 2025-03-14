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
            var orders = _iOrderRepo.GetAll().ToList();
            var resultList = new List<Order>();

            foreach (var order in orders)
            {
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
                return BadRequest();
            else
            {
                if (deserializedResult.Count == 0)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(deserializedResult);
                }
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] Order order)
        {
            if (order == null)
            {
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
                return BadRequest("Invalid ID");
            }
            else if (id == 0)
            {
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
                        return NoContent();
                    }
                    else
                    {
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