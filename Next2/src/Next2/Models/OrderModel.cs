namespace Next2.Models
{
    public class OrderModel : IEntityBase
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public int TableNumber { get; set; }
        public string OrderStatus { get; set; }
        public string OrderType { get; set; }
        public int OrderNumber { get; set; }
        public double Total { get; set; }
    }
}
