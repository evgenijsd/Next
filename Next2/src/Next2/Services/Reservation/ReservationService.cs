using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Resources.Strings;
using Next2.Services.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Reservation
{
    public class ReservationService : IReservationService
    {
        private readonly IMockService _mockService;

        public ReservationService(IMockService mockService)
        {
            _mockService = mockService;
        }

        #region -- IReservationService implementation --

        public async Task<AOResult<IEnumerable<ReservationModel>>> GetReservationsListAsync(string searchQuery = null)
        {
            var result = new AOResult<IEnumerable<ReservationModel>>();

            try
            {
                var resultOfGettingReservations = await GetAllReservationsAsync();

                if (resultOfGettingReservations.IsSuccess)
                {
                    bool containsName(ReservationModel x) => x.CustomerName.Contains(searchQuery, StringComparison.OrdinalIgnoreCase);
                    bool containsPhone(ReservationModel x) => x.Phone.Contains(searchQuery);

                    var reservations = string.IsNullOrEmpty(searchQuery)
                        ? resultOfGettingReservations.Result
                        : resultOfGettingReservations.Result.Where(x => containsName(x) || containsPhone(x));

                    result.SetSuccess(reservations);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetReservationsListAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion

        #region -- Private helpers --

        private async Task<AOResult<IEnumerable<ReservationModel>>> GetAllReservationsAsync()
        {
            var result = new AOResult<IEnumerable<ReservationModel>>();

            try
            {
                var resultOfGettingReservations = await _mockService.GetAllAsync<ReservationModel>();

                if (resultOfGettingReservations is not null)
                {
                    result.SetSuccess(resultOfGettingReservations);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(GetAllReservationsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        #endregion
    }
}
