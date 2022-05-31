using Next2.Enums;
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
        private readonly TaskCompletionSource<bool> _initCompletionSource = new ();

        private IList<UserModel> _users;
        private IList<TaxModel> _tax;
        private IList<BonusModel> _bonuses;
        private IList<BonusConditionModel> _bonusConditions;
        private IList<BonusSetModel> _bonusSets;
        private IList<RewardModel> _rewards;
        private IList<WorkLogRecordModel> _workLogBook;
        private IList<GiftCardModel> _giftCards;

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
                InitUsersAsync(),
                InitTaxAsync(),
                InitBonusAsync(),
                InitBonusSetAsync(),
                InitBonusConditionAsync(),
                IniRewardsAsync(),
                InitWorkLogBookAsync(),
                InitGiftCardsAsync());

            _base = new Dictionary<Type, object>
            {
                { typeof(TaxModel), _tax },
                { typeof(BonusConditionModel), _bonusConditions },
                { typeof(BonusSetModel), _bonusSets },
                { typeof(BonusModel), _bonuses },
                { typeof(RewardModel), _rewards },
                { typeof(UserModel), _users },
                { typeof(WorkLogRecordModel), _workLogBook },
                { typeof(GiftCardModel), _giftCards },
            };

            _maxIdentifiers = new Dictionary<Type, int>
            {
                { typeof(RewardModel), GetMaxId(_rewards) },
                { typeof(UserModel), GetMaxId(_users) },
                { typeof(WorkLogRecordModel), GetMaxId(_workLogBook) },
                { typeof(GiftCardModel), GetMaxId(_giftCards) },
            };

            _initCompletionSource.TrySetResult(true);
        }

        private Task InitWorkLogBookAsync() => Task.Run(() =>
        {
            _workLogBook = new List<WorkLogRecordModel>();
        });

        private Task InitTaxAsync() => Task.Run(() =>
        {
            _tax = new List<TaxModel>
            {
                new TaxModel
                {
                    Id = 1,
                    Name = "Tax",
                    Value = 0.1m,
                },
            };
        });

        private Task InitBonusConditionAsync() => Task.Run(() =>
        {
            _bonusConditions = new List<BonusConditionModel>
            {
                new BonusConditionModel
                {
                    Id = 1,
                    SetId = 1,
                    BonusId = 3,
                },
                new BonusConditionModel
                {
                    Id = 2,
                    SetId = 2,
                    BonusId = 3,
                },
                new BonusConditionModel
                {
                    Id = 3,
                    SetId = 2,
                    BonusId = 5,
                },
                new BonusConditionModel
                {
                    Id = 4,
                    SetId = 2,
                    BonusId = 6,
                },
            };
        });

        private Task InitBonusSetAsync() => Task.Run(() =>
        {
            _bonusSets = new List<BonusSetModel>
            {
                new BonusSetModel
                {
                    Id = 1,
                    SetId = 3,
                    BonusId = 3,
                },
                new BonusSetModel
                {
                    Id = 2,
                    SetId = 2,
                    BonusId = 5,
                },
                new BonusSetModel
                {
                    Id = 3,
                    SetId = 2,
                    BonusId = 4,
                },
                new BonusSetModel
                {
                    Id = 4,
                    SetId = 1,
                    BonusId = 4,
                },
                new BonusSetModel
                {
                    Id = 5,
                    SetId = 2,
                    BonusId = 2,
                },
                new BonusSetModel
                {
                    Id = 6,
                    SetId = 2,
                    BonusId = 6,
                },
            };
        });

        private Task InitBonusAsync() => Task.Run(() =>
        {
            _bonuses = new List<BonusModel>
            {
                new BonusModel
                {
                    Id = 1,
                    Name = "10% Off",
                    Value = 0.1m,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 2,
                    Name = "$ 2.00 Off",
                    Value = 2.0m,
                    Type = EBonusValueType.Value,
                },
                new BonusModel
                {
                    Id = 3,
                    Name = "50% Off BigMack",
                    Value = 0.5m,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 4,
                    Name = "$ 5.00 Off",
                    Value = 5m,
                    Type = EBonusValueType.Value,
                },
                new BonusModel
                {
                    Id = 5,
                    Name = "BOGO Buy 1 and get 1 free",
                    Value = 1.0m,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 6,
                    Name = "GoodNeighbor",
                    Value = 1.0m,
                    Type = EBonusValueType.Percent,
                },
            };
        });

        private Task IniRewardsAsync() => Task.Run(() =>
        {
            int rewardId = 1;
            _rewards = new List<RewardModel>
            {
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c6"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c6"),
                    SetId = 2,
                    SetTitle = "B Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c6"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c6"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c7"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c7"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c7"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c8"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0b214de7895849568eed28f9ba2c47c8"),
                    SetId = 4,
                    SetTitle = "D Pulled Pork Sammy Meal",
                },
            };
        });
        private Task InitUsersAsync() => Task.Run(() =>
        {
            _users = new List<UserModel>
            {
                new UserModel
                {
                    Id = 0,
                    UserName = "Tom",
                    UserType = EUserType.User,
                },
                new UserModel
                {
                    Id = 1,
                    UserName = "Bob Marley",
                    UserType = EUserType.User,
                },
                new UserModel
                {
                    Id = 2,
                    UserName = "Tom Black",
                    UserType = EUserType.User,
                },
                new UserModel
                {
                    Id = 101,
                    UserName = "Admin",
                    UserType = EUserType.Admin,
                },
                new UserModel
                {
                    Id = 111111,
                    UserName = "Admin",
                    UserType = EUserType.Admin,
                },
                new UserModel
                {
                    Id = 555555,
                    UserName = "Waiter",
                    UserType = EUserType.User,
                },
            };
        });
        private Task InitGiftCardsAsync() => Task.Run(() =>
        {
            int id = 7;
            int giftCardNumber = 107;

            _giftCards = new List<GiftCardModel>
            {
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 25.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 65.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 25.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 55.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 15.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 25m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 85.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 15m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 5.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 7.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 65m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 95.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 85m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 25.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 15.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 65m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 25.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 615.50m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 225m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
                 new()
                 {
                     Id = id++,
                     GiftCardFunds = 80m,
                     GiftCardNumber = giftCardNumber++,
                     IsRegistered = false,
                 },
            };
        });

        #endregion
    }
}