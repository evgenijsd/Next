using Next2.Enums;
using Next2.Interfaces;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Mock
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>();

        private IList<TableModel> _tables;
        private IList<OrderModel> _orders;
        private IList<MemberModel> _members;

        private Dictionary<Type, object> _base;

        public MockService()
        {
            Task.Run(InitMocksAsync);
        }

        #region -- IMockService implementation --

        public async Task<int> AddAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;
            int id = 1;

            if (GetBase<T>().Count > 0)
            {
                id = GetBase<T>().Max(x => x.Id) + 1;
                entity.Id = id;
            }
            else
            {
                entity.Id = 1;
            }

            GetBase<T>().Add(entity);

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return id;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            var entityDelete = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Remove(entityDelete);
        }

        public async Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().RemoveAll(predicate);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            var entityUpdate = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);
            entityUpdate = entity;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return entityUpdate;
        }

        public async Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().FirstOrDefault<T>(expression);
        }

        public async Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Any<T>(expression);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Where<T>(expression);
        }

        #endregion

        #region -- Private helpers --

        private List<T> GetBase<T>()
        {
            return (List<T>)_base[typeof(T)];
        }

        private async Task InitMocksAsync()
        {
            try
            {
                _base = new Dictionary<Type, object>();

                await Task.WhenAll(
                    InitOrders(),
                    InitTables(),
                    InitMembers());

                _initCompletionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
            }
        }

        private Task InitOrders() => Task.Run(() =>
        {
            _orders = new List<OrderModel>
            {
                new OrderModel
                {
                    Id = 1,
                    Ordertype = EOrderType.DineIn,
                    Total = 125,
                },
                new OrderModel
                {
                    Id = 2,
                    Ordertype = EOrderType.DineIn,
                    Total = 50,
                },
                new OrderModel
                {
                    Id = 3,
                    Ordertype = EOrderType.DineIn,
                    Total = 200,
                },
                new OrderModel
                {
                    Id = 4,
                    Ordertype = EOrderType.DineIn,
                    Total = 300,
                },
                new OrderModel
                {
                    Id = 5,
                    Ordertype = EOrderType.DineIn,
                    Total = 270,
                },
                new OrderModel
                {
                    Id = 6,
                    Ordertype = EOrderType.DineIn,
                    Total = 215,
                },
            };

            _base.Add(typeof(OrderModel), _orders);
        });

        private Task InitTables() => Task.Run(() =>
        {
            _tables = new List<TableModel>
            {
                 new TableModel
                 {
                     Id = 1,
                     NumberOfSeats = 4,
                     NumberOfAvailableSeats = 4,
                 },
                 new TableModel
                 {
                     Id = 2,
                     NumberOfSeats = 4,
                     NumberOfAvailableSeats = 2,
                 },
                 new TableModel
                 {
                     Id = 3,
                     NumberOfSeats = 6,
                     NumberOfAvailableSeats = 6,
                 },
                 new TableModel
                 {
                     Id = 4,
                     NumberOfSeats = 6,
                     NumberOfAvailableSeats = 3,
                 },
                 new TableModel
                 {
                     Id = 5,
                     NumberOfSeats = 8,
                     NumberOfAvailableSeats = 8,
                 },
                 new TableModel
                 {
                     Id = 6,
                     NumberOfSeats = 8,
                     NumberOfAvailableSeats = 0,
                 },
                 new TableModel
                 {
                     Id = 7,
                     NumberOfSeats = 10,
                     NumberOfAvailableSeats = 10,
                 },
                 new TableModel
                 {
                     Id = 8,
                     NumberOfSeats = 10,
                     NumberOfAvailableSeats = 4,
                 },
                 new TableModel
                 {
                     Id = 9,
                     NumberOfSeats = 2,
                     NumberOfAvailableSeats = 2,
                 },
                 new TableModel
                 {
                     Id = 10,
                     NumberOfSeats = 2,
                     NumberOfAvailableSeats = 0,
                 },
            };

            _base.Add(typeof(TableModel), _tables);
        });

        private Task InitMembers() => Task.Run(() =>
        {
            var cultureInfo = new CultureInfo(Constants.DEFAULT_CULTURE);

            _members = new List<MemberModel>
            {
                new MemberModel
                {
                    Id = 1,
                    CustomerName = "Martin Schleifer",
                    Phone = "732-902-8298",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 13 2019 / 02:12 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Apr 20 2021 / 02:12 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 2,
                    CustomerName = "Ashlynn Westervelt",
                    Phone = "599-663-3931",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 21 2020 / 05:11 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "May 30 2022 / 05:11 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 3,
                    CustomerName = "Carla Dorwart",
                    Phone = "090-540-7412",
                    MembershipStartTime = DateTime.ParseExact(
                        "Jan 29 2021 / 09:30 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Feb 01 2022 / 09:30 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 4,
                    CustomerName = "Davis Septimus",
                    Phone = "301-472-3355",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 09:45 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Oct 19 2021 / 09:45 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 5,
                    CustomerName = "Kierra Bergson",
                    Phone = "503-778-7600",
                    MembershipStartTime = DateTime.ParseExact(
                        "Sep 29 2021 / 11:00 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Dec 20 2021 / 11:00 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 6,
                    CustomerName = "Angel Dias",
                    Phone = "672-533-7711",
                    MembershipStartTime = DateTime.ParseExact(
                        "Aug 28 2021 / 01:50 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Nov 28 2021 / 01:50 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 7,
                    CustomerName = "Kaiya Dorwart",
                    Phone = "688-905-0586",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 10 2021 / 03:00 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 03:00 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 8,
                    CustomerName = "Lincoln Lipshutz",
                    Phone = "174-449-2766",
                    MembershipStartTime = DateTime.ParseExact(
                        "Jul 01 2021 / 08:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Aug 20 2021 / 08:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 9,
                    CustomerName = "Ann Schleifer",
                    Phone = "962-399-9765",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 20 2021 / 10:34 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 27 2021 / 10:34 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 10,
                    CustomerName = "Randy Mango",
                    Phone = "500-803-7621",
                    MembershipStartTime = DateTime.ParseExact(
                        "Apr 29 2021 / 11:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Jul 29 2021 / 11:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 11,
                    CustomerName = "Cheyenne Calzoni",
                    Phone = "576-273-4018",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 20 2021 / 10:00 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Oct 15 2021 / 10:00 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 12,
                    CustomerName = "Zaire Levin",
                    Phone = "601-611-1754",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 11:12 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Jul 19 2021 / 11:12 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 13,
                    CustomerName = "Carla Mango",
                    Phone = "142-826-7912",
                    MembershipStartTime = DateTime.ParseExact(
                        "Apr 29 2021 / 02:40 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Jul 29 2021 / 02:40 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 14,
                    CustomerName = "Cheyenne Levin",
                    Phone = "210-626-0640",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 01 2021 / 06:48 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 21 2021 / 06:48 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
            };

            _base.Add(typeof(MemberModel), _members);
        });

        #endregion
    }
}