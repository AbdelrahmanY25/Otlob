using System.Threading.Tasks;

namespace Otlob.EF.BaseRepository;

public class BaseRepository<T>(ApplicationDbContext context) : IBaseRepository<T> where T : class
{
    private readonly ApplicationDbContext context = context ?? throw new ArgumentNullException(nameof(context));

    private readonly DbSet<T> dbSet = context.Set<T>();

    public void Add(T entity)
    {
        dbSet.Add(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);
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
            .ExecuteUpdate(e => e.SetProperty(d => EFCore.Property<bool>(d, "IsDeleted"), true));
    }
   
    public void UnSoftDelete(Expression<Func<T, bool>> expression)
    {
        IQueryable<T> query = dbSet;

        query.Where(expression)
            .IgnoreQueryFilters()
            .ExecuteUpdate(e => e.SetProperty(d => EFCore.Property<bool>(d, "IsDeleted"), false));
    }

    public void HardDelete(T entity)
    {
        dbSet.Remove(entity);        
    }

    public void HardDeleteRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }

    public bool IsExist(Expression<Func<T, bool>> expression, bool ignoreQueryFilter = false)
    {
        return ignoreQueryFilter ? dbSet.AsNoTracking().IgnoreQueryFilters().Any(expression) : dbSet.AsNoTracking().Any(expression);
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
