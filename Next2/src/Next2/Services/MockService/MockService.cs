using Next2.Interfaces;
using Next2.Models;
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

        private Dictionary<Type, object> _base;

        public MockService()
        {
            Task.Run(InitMocksAsync);
        }

        #region -- IMockService implementation --

        public async Task<int> AddAsync<T>(T entity)
            where T : IEntityModel, new()
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

            return id;
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync<T>(T entity)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            var entityDelete = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);

            return GetBase<T>().Remove(entityDelete);
        }

        public async Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().RemoveAll(predicate);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            var entityUpdate = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);
            entityUpdate = entity;

            return entityUpdate;
        }

        public async Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault<T>(expression);
        }

        public async Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IEntityModel, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().Any<T>(expression);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IEntityModel, new()
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
            _base = new Dictionary<Type, object>();

            await Task.WhenAll(
                InitCategoriesAsync(),
                InitSubategoriesAsync(),
                InitSetsAsync());

            _initCompletionSource.TrySetResult(true);
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
                    CategoryId = 3,
                    Title = "H Burger Meals",
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
                    Title = "A Pulled Pork Sammy Meal",
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
                    SubcategoryId = 1,
                    Title = "D Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "F Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "G Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "H Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "I Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
                new SetModel()
                {
                    Id = id++,
                    SubcategoryId = 1,
                    Title = "J Pulled Pork Sammy Meal",
                    Price = 12.5f,
                    ImagePath = "https://static.onecms.io/wp-content/uploads/sites/9/2021/05/19/urdaburger-FT-RECIPE0621.jpg",
                },
            };

            _base.Add(typeof(SetModel), _sets);
        });

        #endregion
    }
}