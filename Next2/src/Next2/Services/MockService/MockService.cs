using Next2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Next2.Services
{
    public class MockService : IMockService
    {
        private readonly TaskCompletionSource<bool> _initCompletionSource = new TaskCompletionSource<bool>();

        private IList<OrderModel> _orders = new List<OrderModel>();

        private Dictionary<Type, object> _base = new Dictionary<Type, object>();

        public MockService()
        {
            Task.Run(InitMocksAsync);
        }

        #region -- IMockService implementation --

        public async Task<int> AddAsync<T>(T entity)
            where T : IEntityBase, new()
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
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>();
        }

        public async Task<T> GetByIdAsync<T>(int id)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault(x => x.Id == id);
        }

        public async Task<bool> RemoveAsync<T>(T entity)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            var entityDelete = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);

            return GetBase<T>().Remove(entityDelete);
        }

        public async Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().RemoveAll(predicate);
        }

        public async Task<T> UpdateAsync<T>(T entity)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            var entityUpdate = GetBase<T>().FirstOrDefault(x => x.Id == entity.Id);
            entityUpdate = entity;

            return entityUpdate;
        }

        public async Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().FirstOrDefault<T>(expression);
        }

        public async Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new()
        {
            await _initCompletionSource.Task;

            return GetBase<T>().Any<T>(expression);
        }

        public async Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new()
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
                InitOrdersAsync());

            _initCompletionSource.TrySetResult(true);
        }

        private Task InitOrdersAsync() => Task.Run(() =>
        {
            _orders = new List<OrderModel>
            {
                new OrderModel()
                {
                    Id = 1,
                    CustomerName = "Bill Gates",
                    TableName = "Table 1",
                    OrderNumber = 1,
                    Total = 500,
                },
                new OrderModel()
                {
                    Id = 2,
                    CustomerName = "Kate White",
                    TableName = "Table 2",
                    OrderNumber = 2,
                    Total = 300,
                },
                new OrderModel()
                {
                    Id = 3,
                    CustomerName = "Sam Smith",
                    TableName = "Table 3",
                    OrderNumber = 3,
                    Total = 400,
                },
                new OrderModel()
                {
                    Id = 4,
                    CustomerName = "Steve Jobs",
                    TableName = "Table 4",
                    OrderNumber = 4,
                    Total = 350,
                },
                new OrderModel()
                {
                    Id = 5,
                    CustomerName = "Elon musk",
                    TableName = "Table 5",
                    OrderNumber = 5,
                    Total = 700,
                },
                new OrderModel()
                {
                    Id = 6,
                    CustomerName = "Keano Reaves",
                    TableName = "Table 6",
                    OrderNumber = 6,
                    Total = 600,
                },
                new OrderModel()
                {
                    Id = 7,
                    CustomerName = "Roderick Marvin",
                    TableName = "Table 7",
                    OrderNumber = 7,
                    Total = 450,
                },
                new OrderModel()
                {
                    Id = 8,
                    CustomerName = "Clinton Gleichner",
                    TableName = "Table 8",
                    OrderNumber = 8,
                    Total = 330,
                },
                new OrderModel()
                {
                    Id = 9,
                    CustomerName = "Victor Dickinson",
                    TableName = "Table 9",
                    OrderNumber = 9,
                    Total = 550,
                },
                new OrderModel()
                {
                    Id = 10,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderNumber = 10,
                    Total = 970,
                },
            };

            _base.Add(typeof(OrderModel), _orders);
        });

        #endregion
    }
}