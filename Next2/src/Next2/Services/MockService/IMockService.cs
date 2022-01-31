using Next2.Interfaces;
using Next2.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Next2.Services.MockService
{
    public interface IMockService
    {
        Task<int> AddAsync<T>(T entity)
            where T : IEntityBase, new();
        Task<IEnumerable<T>> GetAllAsync<T>()
            where T : IEntityBase, new();
        Task<T> GetByIdAsync<T>(int id)
            where T : IEntityBase, new();
        Task<bool> RemoveAsync<T>(T entity)
            where T : IEntityBase, new();
        Task<int> RemoveAllAsync<T>(Predicate<T> predicate)
            where T : IEntityBase, new();
        Task<T> UpdateAsync<T>(T entity)
            where T : IEntityBase, new();
        Task<T> FindAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new();
        Task<bool> AnyAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new();
        Task<IEnumerable<T>> GetAsync<T>(Func<T, bool> expression)
            where T : IEntityBase, new();
    }
}
