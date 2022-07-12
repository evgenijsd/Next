using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Reservation
{
    public interface IReservationService
    {
        Task<AOResult<IEnumerable<ReservationModel>>> GetReservationsListAsync(string searchQuery = null);
    }
}
