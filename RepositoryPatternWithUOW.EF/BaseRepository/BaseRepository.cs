using Microsoft.EntityFrameworkCore;
using NuGet.ContentModel;
using Otlob.EF.IBaseRepository;
using RepositoryPatternWithUOW.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.EF.BaseRepository
{
    public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context = context;
        private DbSet<T> dbSet = context.Set<T>();

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
        public IEnumerable<T>? Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);
            if (includeProps != null)
            {
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            if (!tracked)
                query = query.AsNoTracking();

            return query.ToList();
        }
        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            return Get(includeProps, expression, tracked).FirstOrDefault();
        }
    }
}
