using Next2.Models;
using Next2.Services.MockService;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace InterTwitter.Services
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>();

        private Dictionary<Type, object> _base;
        private List<CustomerModel> _customers;

        public MockService()
        {
            Task.Run(InitMocksAsync);
        }

        #region -- IMockService implementation --

        public async Task<int> AddAsync<T>(T entity)
            where T : IEntityModelBase, new()
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
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync<T>(T entity)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            var entityDelete = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);

            return GetBase<T>().Remove(entityDelete);
        }

        public async Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().RemoveAll(predicate);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            var entityUpdate = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);
            entityUpdate = entity;

            return entityUpdate;
        }

        public async Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault<T>(expression);
        }

        public async Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IEntityModelBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().Any<T>(expression);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IEntityModelBase, new()
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

                await Task.WhenAll(InitCustomers());

                _initCompletionSource.TrySetResult(true);
            }
            catch (Exception e)
            {
            }
        }

        private Task InitCustomers() => Task.Run(() =>
        {
            CustomersMock.Create(out _customers);
            _base.Add(typeof(CustomerModel), _customers);
        });

        #endregion
    }
}