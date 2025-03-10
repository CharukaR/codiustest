namespace OrderTaskApi.Models
{
    public class Order
    {
        // Property to store the unique identifier for each order
        public int Id { get; set; }

        // Property to store the name of the customer who placed the order
        public string CustomerName { get; set; } = string.Empty;

        // Property to store the total amount for the order
        public decimal Amount { get; set; }

        // Property to store the date and time when the order was created
        public DateTime CreatedAt { get; set; }
    }
}