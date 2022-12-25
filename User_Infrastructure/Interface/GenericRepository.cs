using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using User_Database;

namespace User_Infrastructure.Interface
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly UserDBContext _context;

        public GenericRepository(UserDBContext context)
        {
            _context = context;
        }

        public async Task Add(T entity)
        {
            _ = await _context.Set<T>().AddAsync(entity);
        }

        public async Task AddRange(IEnumerable<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _ = _context.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _context.Set<T>().UpdateRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await _context.Set<T>()
                .FindAsync(id);
        }

        public void Remove(T entity)
        {
            _ = _context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task SaveAsync()
        {
            _ = await _context.SaveChangesAsync();
        }
    }
}