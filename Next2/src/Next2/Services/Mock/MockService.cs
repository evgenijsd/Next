using Next2.Enums;
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

        private IList<UserModel> _users;
        private IList<TaxModel> _tax;
        private IList<BonusModel> _bonuses;
        private IList<BonusConditionModel> _bonusConditions;
        private IList<BonusSetModel> _bonusSets;
        private IList<PortionModel> _portions;
        private IList<RewardModel> _rewards;
        private IList<ProductModel> _products;
        private IList<ReplacementProductModel> _replacementProducts;
        private IList<OptionModel> _optionsProduct;
        private IList<IngredientCategoryModel> _ingredientCategories;
        private IList<IngredientModel> _ingredients;
        private IList<IngredientOfProductModel> _ingredientsOfProductModel;
        private IList<WorkLogRecordModel> _workLogBook;
        private IList<GiftCardModelDTO> _giftCards;

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
                InitPortionsAsync(),
                IniRewardsAsync(),
                InitReplacementProductsAsync(),
                InitProductsAsync(),
                InitOptionsProductAsync(),
                InitIngredientCategoriesAsync(),
                InitIngredientsAsync(),
                InitWorkLogBookAsync(),
                InitIngredientsOfProductAsync());

            _base = new Dictionary<Type, object>
            {
                { typeof(TaxModel), _tax },
                { typeof(BonusConditionModel), _bonusConditions },
                { typeof(BonusSetModel), _bonusSets },
                { typeof(BonusModel), _bonuses },
                { typeof(RewardModel), _rewards },
                { typeof(UserModel), _users },
                { typeof(IngredientCategoryModel), _ingredientCategories },
                { typeof(IngredientOfProductModel), _ingredientsOfProductModel },
                { typeof(IngredientModel), _ingredients },
                { typeof(PortionModel), _portions },
                { typeof(ReplacementProductModel), _replacementProducts },
                { typeof(ProductModel), _products },
                { typeof(OptionModel), _optionsProduct },
                { typeof(WorkLogRecordModel), _workLogBook },
                { typeof(GiftCardModelDTO), _giftCards },
            };

            _maxIdentifiers = new Dictionary<Type, int>
            {
                { typeof(RewardModel), GetMaxId(_rewards) },
                { typeof(UserModel), GetMaxId(_users) },
                { typeof(WorkLogRecordModel), GetMaxId(_workLogBook) },
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
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    SetId = 2,
                    SetTitle = "B Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                    SetId = 3,
                    SetTitle = "C Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0c6d6d48-c6a6-4f8a-8d0b-0b545427a598"),
                    SetId = 1,
                    SetTitle = "A Pulled Pork Sammy Meal",
                },
                new RewardModel
                {
                    Id = rewardId++,
                    CustomerId = new Guid("0c6d6d48-c6a6-4f8a-8d0b-0b545427a598"),
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

        private Task InitIngredientCategoriesAsync() => Task.Run(() =>
        {
            int id = 1;

            _ingredientCategories = new List<IngredientCategoryModel>
            {
                new()
                {
                    Id = id++,
                    Title = "Bread",
                },
                new()
                {
                    Id = id++,
                    Title = "Vegetables",
                },
                new()
                {
                    Id = id++,
                    Title = "Meats",
                },
                new()
                {
                    Id = id++,
                    Title = "Cheese",
                },
                new()
                {
                    Id = id++,
                    Title = "Sauce",
                },
                new()
                {
                    Id = id++,
                    Title = "Sides & Snack",
                },
            };
        });

        private Task InitIngredientsAsync() => Task.Run(() =>
        {
            int id = 1;
            int categoryId = 1;

            _ingredients = new List<IngredientModel>
            {
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 5,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 8,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 10,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 12,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 7,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId,
                    Title = "Ingredient " + id++,
                    Price = 3,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new()
                {
                    Id = id,
                    CategoryId = categoryId++,
                    Title = "Ingredient " + id++,
                    Price = 9,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
            };
        });

        private Task InitIngredientsOfProductAsync() => Task.Run(() =>
        {
            int id = 1;
            int productId = 1;

            _ingredientsOfProductModel = new List<IngredientOfProductModel>
            {
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 15,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    IngredientId = 1,
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    IngredientId = 2,
                },
            };
        });

        private Task InitPortionsAsync() => Task.Run(() =>
        {
            int id = 1;
            int setId = 1;

            _portions = new List<PortionModel>
            {
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 25,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 30,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 35,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 27,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 35,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 43,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 33,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 45,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 56,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 48,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 60,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 70,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 41.3m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 56.3m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 72,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 29.4m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 51,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 37,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 44.7m,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8m,
                },
            };
        });

        private Task InitReplacementProductsAsync() => Task.Run(() =>
        {
            int id = 1;
            int replacementProductId = 1;
            int productId = 1;

            _replacementProducts = new List<ReplacementProductModel>
            {
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = replacementProductId,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId,
                    ProductId = productId++,
                },
                new()
                {
                    Id = id++,
                    ReplacementProductId = replacementProductId++,
                    ProductId = productId++,
                },
            };
        });

        private Task InitProductsAsync() => Task.Run(() =>
        {
            int id = 1;
            int setId = 1;
            int optionId = -2;

            _products = new List<ProductModel>
            {
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 3,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 3,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 15,
                    TotalPrice = 15,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 12,
                    TotalPrice = 12,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 3,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 15,
                    TotalPrice = 15,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 15,
                    TotalPrice = 15,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 18,
                    TotalPrice = 18,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 20,
                    TotalPrice = 20,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 28,
                    TotalPrice = 28,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10.5m,
                    TotalPrice = 10.5m,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 30.8m,
                    TotalPrice = 30.8m,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 16.4m,
                    TotalPrice = 16.4m,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 13,
                    TotalPrice = 13,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 10,
                    TotalPrice = 10,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 27,
                    TotalPrice = 27,
                },
            };
        });

        private Task InitOptionsProductAsync() => Task.Run(() =>
        {
            int id = 1;
            int productId = 1;

            _optionsProduct = new List<OptionModel>
            {
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Well Done",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Well Done",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Well Done",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Well Done",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId,
                    Title = "Rare",
                },
                new()
                {
                    Id = id++,
                    ProductId = productId++,
                    Title = "Medium",
                },
            };
        });

        #endregion
    }
}