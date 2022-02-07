using Next2.Models;

namespace Next2.Extensions
{
    public static class TableExtension
    {
        public static TableBindableModel ToBindableModel(this TableModel table)
        {
            return new TableBindableModel
            {
                Id = table.Id,
                NumberOfSeats = table.NumberOfSeats,
                NumberOfAvailableSeats = table.NumberOfAvailableSeats,
            };
        }

        public static TableModel ToModel(this TableBindableModel table)
        {
            return new TableModel
            {
                Id = table.Id,
                NumberOfSeats = table.NumberOfSeats,
                NumberOfAvailableSeats = table.NumberOfAvailableSeats,
            };
        }
    }
}
