namespace LearnerFunctions.TableStorageClasses
{
    /// <summary>
    /// class for storing order data in table storage
    /// </summary>
    public class OrderStorage : TableStorageBase
    {
        
        public string OrderId { get; set; }
        public string ProductId { get; set; }
        public string Email { get; set; }
        public decimal Price { get; set; }
    }

}
