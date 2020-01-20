namespace LearnerFunctions.Contracts
{
    /// <summary>
    /// class to represent a customer order contract message
    /// </summary>
    public class Order
    {
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }


    }
}
