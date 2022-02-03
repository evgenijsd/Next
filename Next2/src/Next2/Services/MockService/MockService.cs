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

        private IList<UserModel> _users;
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
                _base = new Dictionary<Type, object>();

                await Task.WhenAll(InitMembers());

                _initCompletionSource.TrySetResult(true);
        }

        private Task InitMembers() => Task.Run(() =>
        {
            var cultureInfo = new CultureInfo(Constants.DEFAULT_CULTURE);

            _users = new List<UserModel>
            {
                new UserModel
                {
                    Id = 1,
                    UserName = "Bob Marley",
                },
                new UserModel
                {
                    Id = 2,
                    UserName = "Tom Black",
                },
            };

            _base.Add(typeof(UserModel), _users);
        });

        #endregion
    }
}