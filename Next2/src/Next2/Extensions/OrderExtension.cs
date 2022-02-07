using Next2.Models;

namespace Next2.Extensions
{
    public static class OrderExtension
    {
        public static OrderBindableModel ToBindableModel(this OrderModel table)
        {
            return new OrderBindableModel
            {
                Id = table.Id,
                Ordertype = table.Ordertype,
                Total = table.Total,
            };
        }

        public static OrderModel ToModel(this OrderBindableModel table)
        {
            return new OrderModel
            {
                Id = table.Id,
                Ordertype = table.Ordertype,
                Total = table.Total,
            };
        }
    }
}
