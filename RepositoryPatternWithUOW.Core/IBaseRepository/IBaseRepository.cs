using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.EF.IBaseRepository
{
    public interface IBaseRepository<T> where T : class
    {
        IEnumerable<T>? Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        void Create(T entity);

        void Edit(T entity);

        void Delete(T entity);
    }
}