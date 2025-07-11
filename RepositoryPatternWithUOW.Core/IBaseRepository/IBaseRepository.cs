namespace Otlob.EF.IBaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T>? Get(Expression<Func<T, object>>[]? includeProps = null,
                           Expression<Func<T, bool>>? expression = null, bool tracked = true, bool ignoreQueryFilter = false);
        T? GetOne(Expression<Func<T, object>>[]? includeProps = null,
                  Expression<Func<T, bool>>? expression = null, bool tracked = true, bool ignoreQueryFilter = false);
        IQueryable<TResult>? GetAllWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                                       Expression<Func<T, object>>[]? includeProps = null,
                                                       Expression<Func<T, bool>>? expression = null,
                                                       bool tracked = true, bool ignoreQueryFilter = false);
        TResult? GetOneWithSelect<TResult>(Expression<Func<T, TResult>> selector,
                                           Expression<Func<T, object>>[]? includeProps = null,
                                           Expression<Func<T, bool>>? expression = null,
                                           bool tracked = true, bool ignoreQueryFilter = false);
        void Create(T entity);
        void CreateRange(IEnumerable<T> entities);
        void Edit(T entity);
        void IgnoreChanges(T entity, Expression<Func<T, object>> navigationProperty);
        void ModifyProperty(T entity, Expression<Func<T, object>> property);
        void SoftDelete(Expression<Func<T, bool>> expression);
        void UnSoftDelete(Expression<Func<T, bool>> expression);
        void HardDelete(T entity);
        bool IsExist(Expression<Func<T, bool>> expression);
        IQueryable<KeyValuePair<TKey, int>> EntityWithCountBy<TKey>(Expression<Func<T, TKey>> property);
    }
}