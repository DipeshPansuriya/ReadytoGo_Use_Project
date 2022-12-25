using System.Linq.Expressions;

namespace User_Infrastructure.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task Add(T entity);

        Task AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void UpdateRange(IEnumerable<T> entities);

        IEnumerable<T> Find(Expression<Func<T, bool>> expression);

        Task<IEnumerable<T>> GetAll();

        Task<T> GetById(int id);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entities);

        Task SaveAsync();
    }
}