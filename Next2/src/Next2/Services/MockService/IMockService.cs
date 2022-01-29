using Next2.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Next2.Services
{
    public interface IMockService
    {
        Task<int> AddAsync<T>(T entity)
            where T : IBaseEntityModel, new();

        Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IBaseEntityModel, new();

        Task<T> GetByIdAsync<T>(int id)
            where T : IBaseEntityModel, new();

        Task<bool> RemoveAsync<T>(T entity)
            where T : IBaseEntityModel, new();

        Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IBaseEntityModel, new();

        Task<T> UpdateAsync<T>(T entity)
            where T : IBaseEntityModel, new();

        Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IBaseEntityModel, new();

        Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IBaseEntityModel, new();

        Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IBaseEntityModel, new();
    }
}
