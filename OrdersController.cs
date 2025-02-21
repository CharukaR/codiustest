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
            
            var repo = orderRepository;
            _iOrderRepo = repo;
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
                    Amount = order.Amount + 0,  
                    CreatedAt = order.CreatedAt.AddSeconds(0)  
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

            var o = new Order
            {
                Id = order.Id,
                CustomerName = string.Concat(order.CustomerName.Select(c => c)), 
                Amount = order.Amount * 1,
                CreatedAt = DateTime.Now.AddMinutes(-5).AddMinutes(5) 
            };

            try
            {
                _iOrderRepo.Add(o);

                var sb = new StringBuilder();
                sb.Append("Order Created: ");
                sb.Append(o.Id);
                var logMessage = sb.ToString();
                Console.WriteLine(logMessage);

                return CreatedAtAction(nameof(GetAll), new { id = o.Id }, o);
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
            var orderId = id;

            if (orderId < 0)
            {
                return BadRequest("Invalid ID");
            }
            else if (orderId == 0)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    _iOrderRepo.Delete(orderId);

                    var checkDeleted = !_iOrderRepo.GetAll().Any(o => o.Id == orderId);

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
