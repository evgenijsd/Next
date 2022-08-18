using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Reservation
{
    public interface IReservationService
    {
        Task<AOResult<IEnumerable<ReservationModel>>> GetReservationsAsync(string? searchQuery = null);

        IEnumerable<ReservationModel> GetSortedReservations(EReservationsSortingType typeSort, IEnumerable<ReservationModel> reservations);

        Task<AOResult<int>> AddReservationAsync(ReservationModel reservation);

        Task<AOResult> RemoveReservationAsync(ReservationModel reservation);
    }
}
