using System;

namespace OrderTaskApi.Models
{
    public class Order
    {
        // Property to store the unique identifier for the order
        public int Id { get; set; }

        // Property to store the name of the customer who placed the order
        public string CustomerName { get; set; } = string.Empty;

        // Property to store the total amount of the order
        public decimal Amount { get; set; }

        // Property to store the date and time when the order was created
        public DateTime CreatedAt { get; set; }

        // Constructor to initialize an Order object
        public Order()
        {
            // Log the creation of a new Order object
            Console.WriteLine("Order object created.");
        }

        // Method to display order details
        public void DisplayOrderDetails()
        {
            // Log the start of the DisplayOrderDetails method
            Console.WriteLine("Displaying order details...");

            // Display the order details
            Console.WriteLine($"Order ID: {Id}");
            Console.WriteLine($"Customer Name: {CustomerName}");
            Console.WriteLine($"Amount: {Amount}");
            Console.WriteLine($"Created At: {CreatedAt}");

            // Log the end of the DisplayOrderDetails method
            Console.WriteLine("Order details displayed.");
        }
    }
}