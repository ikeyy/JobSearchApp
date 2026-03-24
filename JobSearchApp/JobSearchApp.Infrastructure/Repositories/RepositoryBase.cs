using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace JobSearchApp.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected RepositoryBase(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        protected IQueryable<T> FindAll(bool trackChanges = false) =>
            trackChanges ? _dbSet : _dbSet.AsNoTracking();

        protected IQueryable<T> FindByCondition(
            Expression<Func<T, bool>> expression,
            bool trackChanges = false) =>
            trackChanges
                ? _dbSet.Where(expression)
                : _dbSet.Where(expression).AsNoTracking();


        protected IQueryable<TResult> Search<TEntity, TResult>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<TEntity, object>> orderBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
        where TEntity : class
            {
            return _context.Set<TEntity>()
                .Where(filter)
                .AsNoTracking()
                .OrderBy(orderBy)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(selector);                   
            }

        protected void Create(T entity) => _dbSet.Add(entity);

        protected void Update(T entity) => _dbSet.Update(entity);

        protected void Delete(T entity) => _dbSet.Remove(entity);
    }
}
