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
                    IsSelect = true,
                    CustomerName = "Bill Gates",
                    TableName = "Table 1",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 1,
                    Total = 50.2,
                },
                new OrderModel()
                {
                    Id = 2,
                    IsSelect = true,
                    CustomerName = "Kate White",
                    TableName = "Table 2",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 2,
                    Total = 30.3,
                },
                new OrderModel()
                {
                    Id = 3,
                    IsSelect = true,
                    CustomerName = "Sam Smith",
                    TableName = "Table 3",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 3,
                    Total = 40.45,
                },
                new OrderModel()
                {
                    Id = 4,
                    IsSelect = true,
                    CustomerName = "Steve Jobs",
                    TableName = "Table 4",
                    OrderStatus = "Annuled",
                    OrderType = "Dine In",
                    OrderNumber = 4,
                    Total = 3.67,
                },
                new OrderModel()
                {
                    Id = 5,
                    IsSelect = true,
                    CustomerName = "Elon musk",
                    TableName = "Table 5",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 5,
                    Total = 70.44,
                },
                new OrderModel()
                {
                    Id = 6,
                    IsSelect = true,
                    CustomerName = "Keano Reaves",
                    TableName = "Table 6",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 6,
                    Total = 6.77,
                },
                new OrderModel()
                {
                    Id = 7,
                    IsSelect = true,
                    CustomerName = "Roderick Marvin",
                    TableName = "Table 7",
                    OrderStatus = "Annuled",
                    OrderType = "Dine In",
                    OrderNumber = 7,
                    Total = 45.11,
                },
                new OrderModel()
                {
                    Id = 8,
                    IsSelect = true,
                    CustomerName = "Clinton Gleichner",
                    TableName = "Table 8",
                    OrderStatus = "Annuled",
                    OrderType = "Dine In",
                    OrderNumber = 8,
                    Total = 33.67,
                },
                new OrderModel()
                {
                    Id = 9,
                    IsSelect = true,
                    CustomerName = "Victor Dickinson",
                    TableName = "Table 9",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 9,
                    Total = 55.16,
                },
                new OrderModel()
                {
                    Id = 10,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 10,
                    Total = 97.66,
                },
                new OrderModel()
                {
                    Id = 11,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 11,
                    Total = 96.00,
                },
                new OrderModel()
                {
                    Id = 12,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Annuled",
                    OrderType = "Dine In",
                    OrderNumber = 12,
                    Total = 9.50,
                },
                new OrderModel()
                {
                    Id = 13,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Annuled",
                    OrderType = "Dine In",
                    OrderNumber = 13,
                    Total = 9.40,
                },
                new OrderModel()
                {
                    Id = 14,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 14,
                    Total = 9.30,
                },
                new OrderModel()
                {
                    Id = 15,
                    IsSelect = true,
                    CustomerName = "Dave Glover",
                    TableName = "Table 10",
                    OrderStatus = "Pending",
                    OrderType = "Dine In",
                    OrderNumber = 15,
                    Total = 9.20,
                },
            };

            _base.Add(typeof(OrderModel), _orders);
        });

        #endregion
    }
}