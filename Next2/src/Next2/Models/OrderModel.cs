namespace Next2.Models
{
    public class OrderModel : IEntityBase
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string TableName { get; set; }
        public int OrderNumber { get; set; }
        public int Total { get; set; }
    }
}
