using Next2.Models;
using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services.MockService
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>();

        private IList<CategoryModel> _categories;
        private IList<SubcategoryModel> _subcategories;
        private IList<SetModel> _sets;
        private IList<UserModel> _users;
        private IList<MemberModel> _members;
        private IList<PortionModel> _portions;

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
                    InitMembersAsync(),
                    InitCategoriesAsync(),
                    InitSubategoriesAsync(),
                    InitSetsAsync(),
                    InitUsersAsync(),
                    InitPortionsAsync());

                _initCompletionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
            }
        }

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
        });

        private Task InitSetsAsync() => Task.Run(() =>
        {
            int id = 1;

            _sets = new List<SetModel>
            {
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "A Pulled Pork Sammy Meal Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "B Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "C Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 2,
                    Title = "D Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 3,
                    Title = "F Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 3,
                    Title = "F2 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 4,
                    Title = "G Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 5,
                    Title = "H Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 6,
                    Title = "I Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 7,
                    Title = "J Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 8,
                    Title = "J8 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 9,
                    Title = "J9 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 10,
                    Title = "J10 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 11,
                    Title = "J11 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 12,
                    Title = "J12 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 13,
                    Title = "J13 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 14,
                    Title = "J14 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 15,
                    Title = "J15 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 16,
                    Title = "J16 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 17,
                    Title = "J17 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 18,
                    Title = "J18 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 19,
                    Title = "J19 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 20,
                    Title = "J20 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 21,
                    Title = "J21 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    Title = "J22 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    Title = "J23 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 22,
                    Title = "J24 Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
            };

            _base.Add(typeof(SetModel), _sets);
        });

        private Task InitUsersAsync() => Task.Run(() =>
        {
            _users = new List<UserModel>
            {
                new UserModel
                {
                    Id = 000001,
                    UserName = "Bob Marley",
                },
                new UserModel
                {
                    Id = 000002,
                    UserName = "Tom Black",
                },
            };

            _base.Add(typeof(UserModel), _users);
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
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Small",
                    Price = 12f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId,
                    Title = "Medium",
                    Price = 18.8f,
                },
                new PortionModel()
                {
                    Id = id++,
                    SetId = setId++,
                    Title = "Large",
                    Price = 23.4f,
                },
            };

            _base.Add(typeof(PortionModel), _portions);
        });

        #endregion
    }
}