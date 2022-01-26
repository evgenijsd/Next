using Next2.Models;
using Next2.ViewModels;

namespace Next2.Extensions
{
    public static class OrderExtension
    {
        public static OrderViewModel ToOrderView(this OrderModel orderModel)
        {
            return new OrderViewModel
            {
                CustomerName = orderModel.CustomerName,
                OrderNumber = orderModel.OrderNumber,
                TableName = orderModel.TableName,
                Total = orderModel.Total,
            };
        }

        public static OrderModel ToOrderModel(this OrderViewModel orderView)
        {
            return new OrderModel
            {
                CustomerName = orderView.CustomerName,
                OrderNumber = orderView.OrderNumber,
                TableName = orderView.TableName,
                Total = orderView.Total,
            };
        }
    }
}
