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

        public void CreateRange(IEnumerable<T> entities)
        {
            dbSet.AddRange(entities);
        }

        public void Edit(T entity)
        {
            dbSet.Update(entity);
        }

        public void IgnoreChanges(T entity, Expression<Func<T, object>> property)
        {
            dbSet.Attach(entity);
            context.Entry(entity).Property(property).IsModified = false;
        }

        public void ModifyProperty(T entity, Expression<Func<T, object>> property)
        {
            dbSet.Attach(entity);
            context.Entry(entity).Property(property).IsModified = true;
        }
               
        public void SoftDelete(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = dbSet;

            query.Where(expression)
                .ExecuteUpdateAsync(e => e.SetProperty(d => EFCore.Property<bool>(d, "IsDeleted"), true));
        }
       
        public void UnSoftDelete(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = dbSet;

            query.Where(expression).IgnoreQueryFilters()
                .ExecuteUpdateAsync(e => e.SetProperty(d => EFCore.Property<bool>(d, "IsDeleted"), false));
        }

        public void HardDelete(T entity)
        {
            dbSet.Remove(entity);
        }

        public bool IsExist(Expression<Func<T, bool>> expression)
        {
            return dbSet.AsNoTracking().AnyAsync(expression).GetAwaiter().GetResult();
        }

        public IQueryable<KeyValuePair<TKey, int>> EntityWithCountBy<TKey>(Expression<Func<T, TKey>> property)
        {
            IQueryable<T> query = dbSet;

            var result = query.AsNoTracking().CountBy(property);

            return result;
        }

        public IQueryable<T>? Get(Expression<Func<T, object>>[]? includeProps = null,
                                   Expression<Func<T, bool>>? expression = null,
                                   bool tracked = true,
                                   bool ignoreQueryFilter = false)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (includeProps != null)
            {
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            query = tracked ? query : query.AsNoTracking();

            query = ignoreQueryFilter ? query.IgnoreQueryFilters() : query;

            return query;
        }

        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null,
                         Expression<Func<T, bool>>? expression = null,
                         bool tracked = true,
                         bool ignoreQueryFilter = false)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            query = tracked ? query : query.AsNoTracking();

            query = ignoreQueryFilter ? query.IgnoreQueryFilters() : query;

            return query.FirstOrDefault();
        }

        public IQueryable<TResult>? GetAllWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                                              Expression<Func<T, object>>[]? includeProps = null,
                                                              Expression<Func<T, bool>>? expression = null,
                                                              bool tracked = true,
                                                              bool ignoreQueryFilter = false)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            query = tracked ? query : query.AsNoTracking();

            query = ignoreQueryFilter ? query.IgnoreQueryFilters() : query;

            return query.Select(selector);
        }

        public TResult? GetOneWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                                             Expression<Func<T, object>>[]? includeProps = null,
                                                             Expression<Func<T, bool>>? expression = null,
                                                             bool tracked = true,
                                                             bool ignoreQueryFilter = false)
        {
            IQueryable<T> query = dbSet;

            if (expression != null)
                query = query.Where(expression);

            if (includeProps != null)
                foreach (var prop in includeProps)
                    query = query.Include(prop);

            query = tracked ? query : query.AsNoTracking();

            query = ignoreQueryFilter ? query.IgnoreQueryFilters() : query;

            return query.Select(selector).FirstOrDefault();
        }       
    } 
}
