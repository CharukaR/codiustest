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

        // Date and time when the order was created
        public DateTime CreatedAt { get; set; }
    }
}