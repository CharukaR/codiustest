namespace OrderTaskApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Constructor to initialize an Order object
        public Order()
        {
            Console.WriteLine("Order object is being created.");
        }

        // Method to display order details
        public void DisplayOrderDetails()
        {
            Console.WriteLine($"Order ID: {Id}");
            Console.WriteLine($"Customer Name: {CustomerName}");
            Console.WriteLine($"Amount: {Amount}");
            Console.WriteLine($"Created At: {CreatedAt}");
        }
    }
}