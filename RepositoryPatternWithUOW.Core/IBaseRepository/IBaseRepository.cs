using System.Linq.Expressions;


namespace Otlob.EF.IBaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        IQueryable<T>? Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        IQueryable<TResult>? GetAllWithSelect<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        TResult? GetOneWithSelect<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        void Create(T entity);
        void Edit(T entity);
        void UnSoftDelete(T entity);
        void SoftDelete(T entity);
        void HardDelete(T entity);
    }
}