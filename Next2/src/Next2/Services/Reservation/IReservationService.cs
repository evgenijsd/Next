using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Reservation
{
    public interface IReservationService
    {
        Task<AOResult<IEnumerable<ReservationModel>>> GetReservationsAsync(string? searchQuery = null);

        IEnumerable<ReservationBindableModel> GetSortedReservations(EReservationsSortingType typeSort, IEnumerable<ReservationBindableModel> reservations);

        Task<AOResult<int>> AddReservationAsync(ReservationModel reservation);

        Task<AOResult> RemoveReservationByIdAsync(int reservationId);
    }
}
