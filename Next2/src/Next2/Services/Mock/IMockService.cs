using Next2.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services.Mock
{
    public interface IMockService
    {
        Task<int> AddAsync<T>(T entity)
            where T : IBaseModel, new();

        Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IBaseModel, new();

        Task<T> GetByIdAsync<T>(int id)
            where T : IBaseModel, new();

        Task<bool> RemoveAsync<T>(T entity)
            where T : IBaseModel, new();

        Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IBaseModel, new();

        Task<T> UpdateAsync<T>(T entity)
            where T : IBaseModel, new();

        Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new();

        Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new();

        Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IBaseModel, new();
    }
}
