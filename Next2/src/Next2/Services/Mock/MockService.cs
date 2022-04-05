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

        private IList<OrderModel> _orders;
        private IList<CategoryModel> _categories;
        private IList<SubcategoryModel> _subcategories;
        private IList<SeatModel> _seats;
        private IList<SetModel> _sets;
        private IList<TableModel> _tables;
        private IList<UserModel> _users;
        private IList<MemberModel> _members;
        private IList<TaxModel> _tax;
        private IList<BonusModel> _bonuses;
        private IList<BonusConditionModel> _bonusConditions;
        private IList<BonusSetModel> _bonusSets;
        private IList<PortionModel> _portions;
        private Dictionary<Type, object> _base;
        private Dictionary<Type, int> _maxIdentifiers;
        private List<CustomerModel> _customers;

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

            await Task.Delay(Constants.SERVER_RESPONCE_DELAY);

            return entity.Id;
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

        private int GetMaxId(IEnumerable<IBaseModel> list) => list.Any()
            ? list.Max(x => x.Id)
            : 0;

        private async Task InitMocksAsync()
        {
            _base = new Dictionary<Type, object>();
            _maxIdentifiers = new Dictionary<Type, int>();

            await Task.WhenAll(
                InitOrdersAsync(),
                InitMembersAsync(),
                InitCategoriesAsync(),
                InitSubategoriesAsync(),
                InitSetsAsync(),
                InitTables(),
                InitUsersAsync(),
                InitCustomersAsync(),
                InitTaxAsync(),
                InitBonusAsync(),
                InitBonusSetAsync(),
                InitBonusConditionAsync(),
                InitPortionsAsync());

            _initCompletionSource.TrySetResult(true);
        }

        private Task InitTaxAsync() => Task.Run(() =>
        {
            _tax = new List<TaxModel>
            {
                new TaxModel
                {
                    Id = 1,
                    Name = "Tax",
                    Value = 0.1,
                },
            };

            _base.Add(typeof(TaxModel), _tax);
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

            _base.Add(typeof(BonusConditionModel), _bonusConditions);
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

            _base.Add(typeof(BonusSetModel), _bonusSets);
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
                    Name = "$ 5.00",
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

            _base.Add(typeof(BonusModel), _bonuses);
        });

        private Task InitOrdersAsync() => Task.Run(async () =>
        {
            await InitSeatsAsync();

            _orders = new List<OrderModel>
            {
                new OrderModel()
                {
                    Id = 1,
                    CustomerName = "Bill Gates",
                    TableNumber = 10,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 1,
                    Total = 50.2,
                    Tax = 5.02,
                },
                new OrderModel()
                {
                    Id = 2,
                    CustomerName = "Kate White",
                    TableNumber = 9,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 2,
                    Total = 30.3,
                    Tax = 3.03,
                },
                new OrderModel()
                {
                    Id = 3,
                    CustomerName = "Sam Smith",
                    TableNumber = 8,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 3,
                    Total = 40.45,
                    Tax = 4.045,
                },
                new OrderModel()
                {
                    Id = 4,
                    CustomerName = "Steve Jobs",
                    TableNumber = 7,
                    OrderStatus = "Cancelled",
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 4,
                    Total = 3.67,
                    Tax = 0.367,
                },
                new OrderModel()
                {
                    Id = 5,
                    CustomerName = "Elon musk",
                    TableNumber = 6,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 5,
                    Total = 70.44,
                    Tax = 7.044,
                },
                new OrderModel()
                {
                    Id = 6,
                    CustomerName = "Keano Reaves",
                    TableNumber = 5,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 6,
                    Total = 6.77,
                    Tax = 0.68,
                },
                new OrderModel()
                {
                    Id = 7,
                    CustomerName = "Roderick Marvin",
                    TableNumber = 4,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 7,
                    Total = 45.11,
                    Tax = 4.511,
                },
                new OrderModel()
                {
                    Id = 8,
                    CustomerName = "Clinton Gleichner",
                    TableNumber = 3,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 8,
                    Total = 33.67,
                    Tax = 3.367,
                },
                new OrderModel()
                {
                    Id = 9,
                    CustomerName = "Victor Dickinson",
                    TableNumber = 2,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 9,
                    Total = 55.16,
                    Tax = 5.516,
                },
                new OrderModel()
                {
                    Id = 10,
                    CustomerName = "Dave Glover",
                    TableNumber = 1,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 10,
                    Total = 97.66,
                    Tax = 9.766,
                },
                new OrderModel()
                {
                    Id = 11,
                    CustomerName = "Dave Glover",
                    TableNumber = 11,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 11,
                    Total = 96.00,
                    Tax = 9.60,
                },
                new OrderModel()
                {
                    Id = 12,
                    CustomerName = "Dave Glover",
                    TableNumber = 12,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 12,
                    Total = 9.50,
                    Tax = 0.95,
                },
                new OrderModel()
                {
                    Id = 13,
                    CustomerName = "Dave Glover",
                    TableNumber = 13,
                    OrderStatus = Constants.OrderStatus.CANCELLED,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 13,
                    Total = 9.40,
                    Tax = 0.94,
                },
                new OrderModel()
                {
                    Id = 14,
                    CustomerName = "Dave Glover",
                    TableNumber = 14,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 14,
                    Total = 9.30,
                    Tax = 0.93,
                },
                new OrderModel()
                {
                    Id = 15,
                    CustomerName = "Dave Glover",
                    TableNumber = 15,
                    OrderStatus = Constants.OrderStatus.IN_PROGRESS,
                    OrderType = EOrderType.DineIn,
                    OrderNumber = 15,
                    Total = 9.20,
                    Tax = 0.92,
                },
            };

            for (int i = 0; i < _orders.Count; i++)
            {
                var orderSeats = _seats.Where(s => s.OrderId == _orders[i].Id);
                var amountsBySeats = orderSeats.Select(x => x.Sets.Select(x => x.Price).Sum());

                _orders[i].Total = amountsBySeats.Sum();
            }

            _base.Add(typeof(OrderModel), _orders);
            _maxIdentifiers.Add(typeof(OrderModel), GetMaxId(_orders));
        });

        private Task InitCategoriesAsync() => Task.Run(() =>
        {
            int id = 1;

            _categories = new List<CategoryModel>
            {
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Baskets & Meals",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Sauces",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Steaks & Chops",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Sides & Snack",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Starters",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Dessert",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Salads",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Beverages",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Burgers & Sandwiches",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Breakfast",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Soups",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Baskets & Meals",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Sauces",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Steaks & Chops",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Sides & Snack",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Starters",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Dessert",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Salads",
                },
                new CategoryModel()
                {
                    Id = id++,
                    Title = "Beverages",
                },
            };

            _base.Add(typeof(CategoryModel), _categories);
            _maxIdentifiers.Add(typeof(CategoryModel), GetMaxId(_categories));
        });

        private Task InitSubategoriesAsync() => Task.Run(() =>
        {
            int id = 1;

            _subcategories = new List<SubcategoryModel>
            {
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 1,
                    Title = "A Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 1,
                    Title = "B Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 1,
                    Title = "C Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 2,
                    Title = "D Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 2,
                    Title = "F Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 3,
                    Title = "G Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 4,
                    Title = "H Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 5,
                    Title = "H5 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 6,
                    Title = "H6 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 7,
                    Title = "H7 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 8,
                    Title = "H8 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 9,
                    Title = "H9 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 10,
                    Title = "H10 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 11,
                    Title = "H11 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 12,
                    Title = "H12 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 13,
                    Title = "H13 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 14,
                    Title = "H14 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 15,
                    Title = "H15 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 16,
                    Title = "H16 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 17,
                    Title = "H17 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 18,
                    Title = "H18 Burger Meals",
                },
                new SubcategoryModel()
                {
                    Id = id++,
                    CategoryId = 19,
                    Title = "19 Burger Meals",
                },
            };

            _base.Add(typeof(SubcategoryModel), _subcategories);
            _maxIdentifiers.Add(typeof(SubcategoryModel), GetMaxId(_subcategories));
        });

        private Task InitSetsAsync() => Task.Run(() =>
        {
            int id = 1;
            int portionId = -2;

            _sets = new List<SetModel>
            {
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    DefaultPortionId = portionId += 3,
                    Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    DefaultPortionId = portionId += 4,
                    Title = "B Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    DefaultPortionId = portionId += 4,
                    Title = "C Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 2,
                    DefaultPortionId = portionId += 1,
                    Title = "D Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 3,
                    DefaultPortionId = portionId += 3,
                    Title = "F Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 3,
                    DefaultPortionId = portionId += 3,
                    Title = "F2 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 4,
                    DefaultPortionId = portionId += 3,
                    Title = "G Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 5,
                    DefaultPortionId = portionId += 3,
                    Title = "H Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 6,
                    DefaultPortionId = portionId += 3,
                    Title = "I Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 7,
                    DefaultPortionId = portionId += 3,
                    Title = "J Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 8,
                    DefaultPortionId = portionId += 3,
                    Title = "J8 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 9,
                    DefaultPortionId = portionId += 3,
                    Title = "J9 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 10,
                    DefaultPortionId = portionId += 3,
                    Title = "J10 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 11,
                    DefaultPortionId = portionId += 3,
                    Title = "J11 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 12,
                    DefaultPortionId = portionId += 3,
                    Title = "J12 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 13,
                    DefaultPortionId = portionId += 3,
                    Title = "J13 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 14,
                    DefaultPortionId = portionId += 3,
                    Title = "J14 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 15,
                    DefaultPortionId = portionId += 3,
                    Title = "J15 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 16,
                    DefaultPortionId = portionId += 3,
                    Title = "J16 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 17,
                    DefaultPortionId = portionId += 3,
                    Title = "J17 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 18,
                    DefaultPortionId = portionId += 3,
                    Title = "J18 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 19,
                    DefaultPortionId = portionId += 3,
                    Title = "J19 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 20,
                    DefaultPortionId = portionId += 3,
                    Title = "J20 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 21,
                    DefaultPortionId = portionId += 3,
                    Title = "J21 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    DefaultPortionId = portionId += 3,
                    Title = "J22 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    DefaultPortionId = portionId += 3,
                    Title = "J23 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    DefaultPortionId = portionId += 3,
                    Title = "J24 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
            };

            _base.Add(typeof(SetModel), _sets);
            _maxIdentifiers.Add(typeof(SetModel), GetMaxId(_sets));
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
            };

            _base.Add(typeof(UserModel), _users);
            _maxIdentifiers.Add(typeof(UserModel), GetMaxId(_users));
        });

        private Task InitSeatsAsync() => Task.Run(() =>
        {
            int seatId = 1;
            int tableId = 1;
            int setId = 1;
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                    SeatNumber = 3,
                    Sets = new List<SetModel>
                    {
                        new SetModel()
                        {
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
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
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                        new SetModel()
                        {
                            Id = setId++,
                            SubcategoryId = 1,
                            Title = "B Pulled Pork Sammy Meal",
                            Price = rand.Next(10, 40),
                            ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                        },
                    },
                },
            };

            _base.Add(typeof(SeatModel), _seats);
            _maxIdentifiers.Add(typeof(SeatModel), GetMaxId(_seats));
        });

        private Task InitTables() => Task.Run(() =>
        {
            _tables = new List<TableModel>
            {
                 new TableModel
                 {
                     Id = 1,
                     TableNumber = 1,
                 },
                 new TableModel
                 {
                     Id = 2,
                     TableNumber = 2,
                 },
                 new TableModel
                 {
                     Id = 3,
                     TableNumber = 3,
                 },
                 new TableModel
                 {
                     Id = 4,
                     TableNumber = 4,
                 },
                 new TableModel
                 {
                     Id = 5,
                     TableNumber = 5,
                 },
                 new TableModel
                 {
                     Id = 6,
                     TableNumber = 6,
                 },
                 new TableModel
                 {
                     Id = 7,
                     TableNumber = 7,
                 },
                 new TableModel
                 {
                     Id = 8,
                     TableNumber = 8,
                 },
                 new TableModel
                 {
                     Id = 9,
                     TableNumber = 9,
                 },
                 new TableModel
                 {
                     Id = 10,
                     TableNumber = 10,
                 },
                 new TableModel
                 {
                     Id = 11,
                     TableNumber = 11,
                 },
                 new TableModel
                 {
                     Id = 12,
                     TableNumber = 12,
                 },
                 new TableModel
                 {
                     Id = 13,
                     TableNumber = 13,
                 },
                 new TableModel
                 {
                     Id = 14,
                     TableNumber = 14,
                 },
                 new TableModel
                 {
                     Id = 15,
                     TableNumber = 15,
                 },
                 new TableModel
                 {
                     Id = 16,
                     TableNumber = 16,
                 },
                 new TableModel
                 {
                     Id = 17,
                     TableNumber = 17,
                 },
                 new TableModel
                 {
                     Id = 18,
                     TableNumber = 18,
                 },
                 new TableModel
                 {
                     Id = 19,
                     TableNumber = 19,
                 },
            };

            _base.Add(typeof(TableModel), _tables);
            _maxIdentifiers.Add(typeof(TableModel), GetMaxId(_tables));
        });

        private Task InitMembersAsync() => Task.Run(() =>
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
            _maxIdentifiers.Add(typeof(MemberModel), GetMaxId(_members));
        });

        private Task InitCustomersAsync() => Task.Run(() =>
        {
            _customers = CustomersMock.Create();
            _base.Add(typeof(CustomerModel), _customers);
            _maxIdentifiers.Add(typeof(CustomerModel), GetMaxId(_customers));
        });

        private Task InitPortionsAsync() => Task.Run(() =>
        {
            int id = 1;
            int setId = 1;
            var rand = new Random();

            _portions = new List<PortionModel>
            {
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = rand.Next(10, 20),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = rand.Next(20, 30),
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = rand.Next(30, 40),
                },
            };

            _base.Add(typeof(PortionModel), _portions);
            _maxIdentifiers.Add(typeof(PortionModel), GetMaxId(_portions));
        });

        #endregion
    }
}