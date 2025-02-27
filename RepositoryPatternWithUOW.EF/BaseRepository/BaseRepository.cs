using Microsoft.EntityFrameworkCore;
using Otlob.EF.IBaseRepository;
using System.Linq.Expressions;

namespace Otlob.EF.BaseRepository
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;

        private readonly DbSet<T> dbSet;

        public BaseRepository(ApplicationDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
            dbSet = context.Set<T>();
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }

        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        public IQueryable<T>? Get(Expression<Func<T, object>>[]? includeProps = null,
                                   Expression<Func<T, bool>>? expression = null,
                                   bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            if (!tracked)
                query = query.AsNoTracking();

            return query;
        }

        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null,
                         Expression<Func<T, bool>>? expression = null,
                         bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            if (!tracked)
                query = query.AsNoTracking();

            return query.FirstOrDefault();
        }

        public IQueryable<TResult>? GetAllWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                                              Expression<Func<T, object>>[]? includeProps = null,
                                                              Expression<Func<T, bool>>? expression = null,
                                                              bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach(var prop in includeProps)
                    query = query.Include(prop);

            if (!tracked)
                query = query.AsNoTracking();

            return query.Select(selector);
        }

        public TResult? GetOneWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                                             Expression<Func<T, object>>[]? includeProps = null,
                                                             Expression<Func<T, bool>>? expression = null,
                                                             bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            if (!tracked)
                query = query.AsNoTracking();

            return query.Select(selector).FirstOrDefault();
        }
    }
}
