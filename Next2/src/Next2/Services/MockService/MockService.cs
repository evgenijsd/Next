using Next2.Interfaces;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>();

        private IList<MemberModel> _members;

        private Dictionary<Type, object> _base;
        private List<CustomerModel> _customers;

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

                await Task.WhenAll(InitCustomers(), InitMembers());

                _initCompletionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
            }
        }

        private Task InitMembers() => Task.Run(() =>
        {
            _members = new List<MemberModel>();
            var cultureInfo = new CultureInfo(Constants.DEFAULT_CULTURE);

            _members = new List<MemberModel>
            {
                new MemberModel
                {
                    Id = 1,
                    CustomerName = "Martin Schleifer",
                    Phone = "732-902-8298",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 13 2019 / 04:12 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 20 2021 / 08:36 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 2,
                    CustomerName = "Ashlynn Westervelt",
                    Phone = "599-663-3931",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 21 2020 / 09:11 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 30 2022 / 10:00 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                Id = 3,
                CustomerName = "Carla Dorwart",
                Phone = "090-540-7412",
                MembershipStartTime = DateTime.ParseExact(
                    "Mar 29 2021 / 09:30 AM",
                    Constants.LONG_DATE_FORMAT,
                    cultureInfo),
                MembershipEndTime = DateTime.ParseExact(
                    "Mar 01 2022 / 07:35 AM",
                    Constants.LONG_DATE_FORMAT,
                    cultureInfo),
                },
                new MemberModel
                {
                Id = 4,
                CustomerName = "Davis Septimus",
                Phone = "301-472-3355",
                MembershipStartTime = DateTime.ParseExact(
                    "Mar 29 2021 / 09:22 AM",
                    Constants.LONG_DATE_FORMAT,
                    cultureInfo),
                MembershipEndTime = DateTime.ParseExact(
                    "Mar 19 2021 / 09:22 AM",
                    Constants.LONG_DATE_FORMAT,
                    cultureInfo),
                },
                new MemberModel
                {
                    Id = 5,
                    CustomerName = "Kierra Bergson",
                    Phone = "503-778-7600",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 12:22 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 20 2021 / 12:22 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 6,
                    CustomerName = "Angel Dias",
                    Phone = "672-533-7711",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:54 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 28 2021 / 08:54 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 7,
                    CustomerName = "Kaiya Dorwart",
                    Phone = "688-905-0586",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 03:51 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 03:51 PM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 8,
                    CustomerName = "Lincoln Lipshutz",
                    Phone = "174-449-2766",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 9,
                    CustomerName = "Ann Schleifer",
                    Phone = "962-399-9765",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 10,
                    CustomerName = "Randy Mango",
                    Phone = "500-803-7621",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 11,
                    CustomerName = "Cheyenne Calzoni",
                    Phone = "576-273-4018",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 12,
                    CustomerName = "Zaire Levin",
                    Phone = "601-611-1754",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 13,
                    CustomerName = "Carla Mango",
                    Phone = "142-826-7912",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
                new MemberModel
                {
                    Id = 14,
                    CustomerName = "Cheyenne Levin",
                    Phone = "210-626-0640",
                    MembershipStartTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                    MembershipEndTime = DateTime.ParseExact(
                        "Mar 29 2021 / 02:48 AM",
                        Constants.LONG_DATE_FORMAT,
                        cultureInfo),
                },
            };

            _base.Add(typeof(MemberModel), _members);
        });

        private Task InitCustomers() => Task.Run(() =>
        {
            _customers = CustomersMock.Create();
            _base.Add(typeof(CustomerModel), _customers);
        });

        #endregion
    }
}