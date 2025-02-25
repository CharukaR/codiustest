using System;

namespace OrderTaskApi.Models
{
    public class Order
    {
        // Unique identifier for the order
        public int Id { get; set; }

        // Name of the customer who placed the order
        public string CustomerName { get; set; } = string.Empty;

        // Total amount for the order
        public decimal Amount { get; set; }

        // Timestamp indicating when the order was created
        public DateTime CreatedAt { get; set; }
    }
}
using System;
using Microsoft.Extensions.Logging;

namespace OrderTaskApi.Models
{
    public class Order
    {
        private readonly ILogger<Order> _logger;

        // Constructor to initialize the logger
        public Order(ILogger<Order> logger)
        {
            _logger = logger;
            _logger.LogInformation("Order instance created.");
        }

        // Unique identifier for the order
        public int Id { get; set; }

        // Name of the customer who placed the order
        public string CustomerName { get; set; } = string.Empty;

        // Total amount for the order
        public decimal Amount { get; set; }

        // Timestamp indicating when the order was created
        public DateTime CreatedAt { get; set; }

        // Method to log order details
        public void LogOrderDetails()
        {
            _logger.LogInformation("Logging order details: Id={Id}, CustomerName={CustomerName}, Amount={Amount}, CreatedAt={CreatedAt}", Id, CustomerName, Amount, CreatedAt);
        }
    }
}