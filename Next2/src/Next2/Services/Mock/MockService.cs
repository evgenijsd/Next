using Next2.Interfaces;
using Next2.Models;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Mock
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new();

        private IList<RewardModel> _rewards;
        private IList<WorkLogRecordModel> _workLogBook;
        private IList<GiftCardModelDTO> _giftCards;
        private IList<ReservationModel> _reservationsList;

        private Dictionary<Type, object> _base;
        private Dictionary<Type, int> _maxIdentifiers;

        public MockService()
        {
            Task.Run(InitMocksAsync);
        }

        #region -- IMockService implementation --

        public int MaxIdentifier<T>() => _maxIdentifiers[typeof(T)];

        public async Task<int> AddAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            entity.Id = ++_maxIdentifiers[typeof(T)];

            GetBase<T>().Add(entity);

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return entity.Id;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            var entityDelete = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Remove(entityDelete);
        }

        public async Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().RemoveAll(predicate);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            var entityUpdate = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);
            var index = GetBase<T>().IndexOf(entityUpdate);
            GetBase<T>()[index] = entity;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return entityUpdate;
        }

        public async Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().FirstOrDefault<T>(expression);
        }

        public async Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Any<T>(expression);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new()
        {
            await _initCompletionSource.Task;

            await Task.Delay(Constants.Limits.SERVER_RESPONCE_DELAY);

            return GetBase<T>().Where<T>(expression);
        }

        #endregion

        #region -- Private helpers --

        private List<T> GetBase<T>()
        {
            return (List<T>)_base[typeof(T)];
        }

        private int GetMaxId(IEnumerable<IBaseModel> list) => list.Any()
            ? list.Max(x => x.Id)
            : 0;

        private async Task InitMocksAsync()
        {
            await Task.WhenAll(
                InitRewardsAsync(),
                InitWorkLogBookAsync(),
                InitReservationsAsync());

            _base = new Dictionary<Type, object>
            {
                { typeof(RewardModel), _rewards },
                { typeof(WorkLogRecordModel), _workLogBook },
                { typeof(GiftCardModelDTO), _giftCards },
                { typeof(ReservationModel), _reservationsList },
            };

            _maxIdentifiers = new Dictionary<Type, int>
            {
                { typeof(RewardModel), GetMaxId(_rewards) },
                { typeof(WorkLogRecordModel), GetMaxId(_workLogBook) },
                { typeof(ReservationModel), GetMaxId(_reservationsList) },
            };

            _initCompletionSource.TrySetResult(true);
        }

        private Task InitWorkLogBookAsync() => Task.Run(() =>
        {
            _workLogBook = new List<WorkLogRecordModel>();
        });

        private Task InitRewardsAsync() => Task.Run(() =>
        {
            int rewardId = 1;

            _rewards = new List<RewardModel>
            {
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    DishId = 1,
                    DishTitle = "A Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    DishId = 2,
                    DishTitle = "B Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    DishId = 3,
                    DishTitle = "C Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    DishId = 3,
                    DishTitle = "C Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    DishId = 1,
                    DishTitle = "A Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    DishId = 1,
                    DishTitle = "A Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    DishId = 3,
                    DishTitle = "C Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0c6d6d48-c6a6-4f8a-8d0b-0b545427a598"),
                    DishId = 1,
                    DishTitle = "A Pulled Pork Sammy Meal",
                },
                new()
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0c6d6d48-c6a6-4f8a-8d0b-0b545427a598"),
                    DishId = 4,
                    DishTitle = "D Pulled Pork Sammy Meal",
                },
            };
        });

        private Task InitReservationsAsync() => Task.Run(() =>
        {
            int reservationId = 1;

            Random random = new();

            _reservationsList = new List<ReservationModel>
            {
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Assunta Dickinson",
                    Phone = "7452735838",
                    GuestsAmount = 1,
                    TableNumber = 12,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Jonas Jenkins",
                    Phone = "4454631426",
                    GuestsAmount = 2,
                    TableNumber = 19,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Amiya Emard",
                    Phone = "7923342784",
                    GuestsAmount = 4,
                    TableNumber = 9,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Lillie Skiles",
                    Phone = "2974674030",
                    GuestsAmount = 3,
                    TableNumber = 21,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Ara Predovic",
                    Phone = "1691358155",
                    GuestsAmount = 5,
                    TableNumber = 10,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Jarrell Keebler",
                    Phone = "1334331439",
                    GuestsAmount = 2,
                    TableNumber = 20,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Ernestina Walter",
                    Phone = "5637335818",
                    GuestsAmount = 2,
                    TableNumber = 2,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Queen Christiansen",
                    Phone = "4711804745",
                    GuestsAmount = 3,
                    TableNumber = 6,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Samantha Quigley",
                    Phone = "7834775681",
                    GuestsAmount = 1,
                    TableNumber = 13,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Tia Casper",
                    Phone = "4283749819",
                    GuestsAmount = 1,
                    TableNumber = 4,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "James Carder",
                    Phone = "6351341232",
                    GuestsAmount = 3,
                    TableNumber = 16,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Cooper Kenter",
                    Phone = "3110419788",
                    GuestsAmount = 4,
                    TableNumber = 12,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Terry Bergson",
                    Phone = "2347852424",
                    GuestsAmount = 1,
                    TableNumber = 7,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Cooper Torff",
                    Phone = "1320761237",
                    GuestsAmount = 3,
                    TableNumber = 10,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Makenna Calzoni",
                    Phone = "8672130872",
                    GuestsAmount = 1,
                    TableNumber = 18,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
            };
        });

        #endregion
    }
}