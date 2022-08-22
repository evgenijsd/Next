﻿using Next2.Enums;
using Next2.Helpers.ProcessHelpers;
using Next2.Models;
using Next2.Models.Bindables;
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

        public async Task<AOResult<IEnumerable<ReservationModel>>> GetReservationsAsync(string? searchQuery = null)
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
                result.SetError($"{nameof(GetReservationsAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public IEnumerable<ReservationBindableModel> GetSortedReservations(EReservationsSortingType typeSort, IEnumerable<ReservationBindableModel> reservations)
        {
            Func<ReservationBindableModel, object> sortingSelector = typeSort switch
            {
                EReservationsSortingType.ByCustomerName => x => x.CustomerName,
                EReservationsSortingType.ByTableNumber => x => x.TableNumber,
                _ => throw new NotImplementedException(),
            };

            return reservations.OrderBy(sortingSelector);
        }

        public async Task<AOResult<int>> AddReservationAsync(ReservationModel reservation)
        {
            var result = new AOResult<int>();

            try
            {
                var recordId = await _mockService.AddAsync(reservation);

                if (recordId > 0)
                {
                    result.SetSuccess(recordId);
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(AddReservationAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> RemoveReservationByIdAsync(int reservationId)
        {
            var result = new AOResult();

            try
            {
                var reservationForRemove = await _mockService.GetByIdAsync<ReservationModel>(reservationId);

                if (reservationForRemove is not null)
                {
                    var isReservationRemoved = await _mockService.RemoveAsync(reservationForRemove);

                    if (isReservationRemoved)
                    {
                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(RemoveReservationByIdAsync)}: exception", Strings.SomeIssues, ex);
            }

            return result;
        }

        public async Task<AOResult> UpdateReservationAsync(ReservationModel? reservation)
        {
            var result = new AOResult();

            try
            {
                if (reservation is not null)
                {
                    var updateResult = await _mockService.UpdateAsync(reservation);

                    if (updateResult is not null)
                    {
                        result.SetSuccess();
                    }
                }
            }
            catch (Exception ex)
            {
                result.SetError($"{nameof(UpdateReservationAsync)}: exception", Strings.SomeIssues, ex);
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
                var allReservations = await _mockService.GetAllAsync<ReservationModel>();

                if (allReservations is not null)
                {
                    result.SetSuccess(allReservations);
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
