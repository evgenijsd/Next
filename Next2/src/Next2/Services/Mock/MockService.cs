using Next2.Interfaces;
using Next2.Models;
using Next2.Models.API.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.Mock
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new();

        private IList<RewardModel> _rewards = new List<RewardModel>();
        private IList<WorkLogRecordModel> _workLogBook = new List<WorkLogRecordModel>();
        private IList<ReservationModel> _reservationsList = new List<ReservationModel>();
        private IList<HoldDishModel> _holdDishes = new List<HoldDishModel>();

        private Dictionary<Type, object> _base = new();
        private Dictionary<Type, int> _maxIdentifiers = new();

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
                InitReservationsAsync(),
                InitHoldDishesAsync());

            _base = new Dictionary<Type, object>
            {
                { typeof(RewardModel), _rewards },
                { typeof(WorkLogRecordModel), _workLogBook },
                { typeof(ReservationModel), _reservationsList },
                { typeof(HoldDishModel), _holdDishes },
            };

            _maxIdentifiers = new Dictionary<Type, int>
            {
                { typeof(RewardModel), GetMaxId(_rewards) },
                { typeof(WorkLogRecordModel), GetMaxId(_workLogBook) },
                { typeof(ReservationModel), GetMaxId(_reservationsList) },
                { typeof(HoldDishModel), GetMaxId(_holdDishes) },
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
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Jonas Jenkins",
                    Phone = "4454631426",
                    GuestsAmount = 2,
                    Table = new()
                    {
                        Id = Guid.Parse("729f2eeb-437a-4323-ba43-523ab6255801"),
                        Number = 24,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Amiya Emard",
                    Phone = "7923342784",
                    GuestsAmount = 4,
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Lillie Skiles",
                    Phone = "2974674030",
                    GuestsAmount = 3,
                    Table = new()
                    {
                        Id = Guid.Parse("925634a3-7e86-409b-9076-a0492bb08425"),
                        Number = 26,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    CustomerName = "Ara Predovic",
                    Phone = "1691358155",
                    GuestsAmount = 5,
                    Table = new()
                    {
                        Id = Guid.Parse("3fd89029-6eef-48fe-bf02-3a18b113d97c"),
                        Number = 27,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "Jarrell Keebler",
                    Phone = "1334331439",
                    GuestsAmount = 2,
                    Table = new()
                    {
                        Id = Guid.Parse("176d3142-a5a1-4e3d-8c5c-daef051ad7ff"),
                        Number = 28,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "Ernestina Walter",
                    Phone = "5637335818",
                    GuestsAmount = 2,
                    Table = new()
                    {
                        Id = Guid.Parse("6ef2b7cc-680a-4dcb-a73f-25126bd45746"),
                        Number = 29,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "Queen Christiansen",
                    Phone = "4711804745",
                    GuestsAmount = 3,
                    Table = new()
                    {
                        Id = Guid.Parse("0294f5b8-d731-4b11-ae37-c5dce1c45f36"),
                        Number = 30,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "Samantha Quigley",
                    Phone = "7834775681",
                    GuestsAmount = 1,
                    Table = new()
                    {
                        Id = Guid.Parse("73ddc3b9-5af9-4263-a481-86a77f46e7aa"),
                        Number = 31,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "Tia Casper",
                    Phone = "4283749819",
                    GuestsAmount = 1,
                    Table = new()
                    {
                        Id = Guid.Parse("d5b6480e-6634-49ab-bcfe-4bde785cf989"),
                        Number = 32,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "555555",
                        UserName = "555555",
                    },
                    CustomerName = "James Carder",
                    Phone = "6351341232",
                    GuestsAmount = 3,
                    Table = new()
                    {
                        Id = Guid.Parse("d434c3d1-4bab-4246-a96f-609de3f7ffb0"),
                        Number = 33,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Cooper Kenter",
                    Phone = "3110419788",
                    GuestsAmount = 4,
                    Table = new()
                    {
                        Id = Guid.Parse("f9a7cdb2-84df-4146-93df-2f021c06abfe"),
                        Number = 34,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Terry Bergson",
                    Phone = "2347852424",
                    GuestsAmount = 1,
                    Table = new()
                    {
                        Id = Guid.Parse("c721f174-32d0-4832-9f73-3198c2a7232d"),
                        Number = 35,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Cooper Torff",
                    Phone = "1320761237",
                    GuestsAmount = 3,
                    Table = new()
                    {
                        Id = Guid.Parse("343be7c3-94a2-4ba9-8781-317ea1a84d97"),
                        Number = 36,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = reservationId++,
                    Employee = new()
                    {
                        EmployeeId = "111111",
                        UserName = "111111",
                    },
                    CustomerName = "Makenna Calzoni",
                    Phone = "8672130872",
                    GuestsAmount = 1,
                    Table = new()
                    {
                        Id = Guid.Parse("2681c6e5-4301-4041-a8c0-a4aff794e8c5"),
                        Number = 37,
                        SeatNumbers = 10,
                    },
                    Comment = null,
                    DateTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
            };
        });

        private Task InitHoldDishesAsync() => Task.Run(() =>
        {
            Random random = new();

            _holdDishes = new List<HoldDishModel>
            {
                new()
                {
                    Id = 1,
                    TableNumber = 12,
                    DishName = "bBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 2,
                    TableNumber = 19,
                    DishName = "cBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 3,
                    TableNumber = 12,
                    DishName = "dBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 4,
                    TableNumber = 19,
                    DishName = "eBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 5,
                    TableNumber = 10,
                    DishName = "rBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 6,
                    TableNumber = 21,
                    DishName = "tBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 7,
                    TableNumber = 10,
                    DishName = "yBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 8,
                    TableNumber = 21,
                    DishName = "uBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 9,
                    TableNumber = 13,
                    DishName = "iBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 10,
                    TableNumber = 4,
                    DishName = "oBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 11,
                    TableNumber = 13,
                    DishName = "pBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 12,
                    TableNumber = 4,
                    DishName = "qBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 13,
                    TableNumber = 7,
                    DishName = "wBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 14,
                    TableNumber = 10,
                    DishName = "zBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
                new()
                {
                    Id = 15,
                    TableNumber = 7,
                    DishName = "xBig Mak",
                    ReleaseTime = DateTime.Now.AddSeconds(random.Next(3600, 1209600)),
                },
            };
        });

        #endregion
    }
}