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
        private readonly TaskCompletionSource<bool> _initCompletionSource = new ();

        private IList<OrderModel> _orders;
        private IList<SeatModel> _seats;
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
                InitOrdersAsync(),
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
                { typeof(OrderModel), _orders },
                { typeof(UserModel), _users },
                { typeof(SeatModel), _seats },
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
                { typeof(OrderModel), GetMaxId(_orders) },
                { typeof(UserModel), GetMaxId(_users) },
                { typeof(SeatModel), GetMaxId(_seats) },
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
                    Value = 0.1f,
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
                    Value = 0.1f,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 2,
                    Name = "$ 2.00 Off",
                    Value = 2.0f,
                    Type = EBonusValueType.Value,
                },
                new BonusModel
                {
                    Id = 3,
                    Name = "50% Off BigMack",
                    Value = 0.5f,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 4,
                    Name = "$ 5.00 Off",
                    Value = 5f,
                    Type = EBonusValueType.Value,
                },
                new BonusModel
                {
                    Id = 5,
                    Name = "BOGO Buy 1 and get 1 free",
                    Value = 1.0f,
                    Type = EBonusValueType.Percent,
                },
                new BonusModel
                {
                    Id = 6,
                    Name = "GoodNeighbor",
                    Value = 1.0f,
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

        private Task InitOrdersAsync() => Task.Run(async () =>
        {
            await InitSeatsAsync();

            _orders = new List<OrderModel>
            {
                new OrderModel()
                {
                    Id = 1,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("05c541cf-a5b7-4c50-aed4-e30afd4be1b2"),
                        FullName = "Adam Brody",
                    },
                    TableNumber = 10,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 1,
                    Total = 50.2f,
                    PriceTax = 5.02f,
                    PaymentStatus = Enums.EOrderStatus.InProgress,
                },
                new OrderModel()
                {
                    Id = 2,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("0777f536-3be6-4baf-b436-83541a21989c"),
                        FullName = "Abraham Linkoln",
                    },
                    TableNumber = 9,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 2,
                    Total = 30.3f,
                    PriceTax = 3.03f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 3,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("0c6d6d48-c6a6-4f8a-8d0b-0b545427a598"),
                        FullName = "Aaron Rodgers",
                    },
                    TableNumber = 8,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 3,
                    Total = 40.45f,
                    PriceTax = 4.05f,
                    PaymentStatus = Enums.EOrderStatus.InProgress,
                },
                new OrderModel()
                {
                    Id = 4,
                    Customer = new (),
                    TableNumber = 7,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 4,
                    Total = 3.67f,
                    PriceTax = 0.37f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 5,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("269428aa-5676-48e8-95ee-ccc1c12b2e38"),
                        FullName = "Angel Dias",
                    },
                    TableNumber = 6,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 5,
                    Total = 70.44f,
                    PriceTax = 7.04f,
                    PaymentStatus = Enums.EOrderStatus.InProgress,
                },
                new OrderModel()
                {
                    Id = 6,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("27db9e1e-e6be-48e6-bf62-9de7d201727f"),
                        FullName = "Kaiya Dorwart",
                    },
                    TableNumber = 5,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 6,
                    Total = 6.77f,
                    PriceTax = 0.68f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 7,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("395b3d48-0768-4838-a1a8-dc2511cee620"),
                        FullName = "Lincoln Lipshutz",
                    },
                    TableNumber = 4,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 7,
                    Total = 45.11f,
                    PriceTax = 4.51f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 8,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("41904417-37af-4a71-b66d-c467879737f2"),
                        FullName = "Randy Mango",
                    },
                    TableNumber = 3,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 8,
                    Total = 33.67f,
                    PriceTax = 3.37f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 9,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("6706ce20-5afc-4444-8c96-4a200181b07f"),
                        FullName = "Martin Schleifer",
                    },
                    TableNumber = 2,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 9,
                    Total = 55.16f,
                    PriceTax = 5.52f,
                    PaymentStatus = Enums.EOrderStatus.InProgress,
                },
                new OrderModel()
                {
                    Id = 10,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("73272a88-d19b-4d26-b290-169e416ff72b"),
                        FullName = "Carla Dorwart",
                    },
                    TableNumber = 1,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 10,
                    Total = 97.66f,
                    PriceTax = 9.77f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 11,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("8363e8a5-3d2c-4876-b8c7-a826a21c2a6e"),
                        FullName = "Davis Septimus",
                    },
                    TableNumber = 11,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 11,
                    Total = 96.00f,
                    PriceTax = 9.60f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 12,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("9e0f532a-2a7b-4516-9aac-5f851d024626"),
                        FullName = "Grover Parsons",
                    },
                    TableNumber = 12,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 12,
                    Total = 9.50f,
                    PriceTax = 0.95f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 13,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("b6fb51f4-9865-43ab-a81f-a50190818801"),
                        FullName = "Douglas Moreno",
                    },
                    TableNumber = 13,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 13,
                    Total = 9.40f,
                    PriceTax = 0.94f,
                    PaymentStatus = Enums.EOrderStatus.InProgress,
                },
                new OrderModel()
                {
                    Id = 14,
                    Customer = new (),
                    TableNumber = 14,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 14,
                    Total = 9.30f,
                    PriceTax = 0.93f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
                new OrderModel()
                {
                    Id = 15,
                    Customer = new CustomerBindableModel
                    {
                        Id = new Guid("debcbbd7-2799-4907-8c48-57a7a33cba67"),
                        FullName = "Kenneth Feron",
                    },
                    TableNumber = 15,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 15,
                    Total = 9.20f,
                    PriceTax = 0.92f,
                    PaymentStatus = Enums.EOrderStatus.WaitingForPayment,
                },
            };

            for (int i = 0; i < _orders.Count; i++)
            {
                var orderSeats = _seats.Where(s => s.OrderId == _orders[i].Id);
                var amountsBySeats = orderSeats.Select(x => x.Sets.Select(x => x.Price).Sum());

                _orders[i].Total = amountsBySeats.Sum();
            }
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

        private Task InitSeatsAsync() => Task.Run(() =>
        {
            int seatId = 1;
            int tableId = 1;
            var rand = new Random();

            _seats = new List<SeatModel>
            {
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 3,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 5,
                            SubcategoryId = 1,
                            Title = "E Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 5,
                            SubcategoryId = 1,
                            Title = "E Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "C Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 2,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId,
                    SeatNumber = 1,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 1,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
                new SeatModel
                {
                    Id = seatId++,
                    OrderId = tableId++,
                    SeatNumber = 2,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = 3,
                            SubcategoryId = 1,
                            Title = "3 Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = 4,
                            SubcategoryId = 1,
                            Title = "D Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
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
                    Price = 41.3f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 56.3f,
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
                    Price = 29.4f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    Price = 44.7f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 57.8f,
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
                    ProductPrice = 10.5f,
                    TotalPrice = 10.5f,
                },
                new()
                {
                    Id = id,
                    SetId = setId++,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 30.8f,
                    TotalPrice = 30.8f,
                },
                new()
                {
                    Id = id,
                    SetId = setId,
                    DefaultOptionId = optionId += 2,
                    DefaultProductId = id,
                    Title = "Product " + id++,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                    ProductPrice = 16.4f,
                    TotalPrice = 16.4f,
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