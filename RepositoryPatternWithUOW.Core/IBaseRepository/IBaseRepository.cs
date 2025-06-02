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
        void SoftDelete(Expression<Func<T, bool>> expression);
        void UnSoftDelete(Expression<Func<T, bool>> expression);
        void HardDelete(T entity);
        TResult? AllowExplicitLoadingByReferenceWithSelect<TProperty, TResult>(T entity,
                                                           Expression<Func<T, TProperty>> navigationProperty,
                                                           Expression<Func<TProperty, TResult>> selector,
                                                           Expression<Func<TProperty, bool>>? expression = null) where TProperty : class;
    }
}