using Otlob.Core.Models;
using Otlob.EF.IBaseRepository;
using RepositoryPatternWithUOW.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otlob.Core.IUnitOfWorkRepository
{
    public interface IUnitOfWorkRepository : IDisposable
    {
        IBaseRepository<ContactUs> ContactUs { get; }
        IBaseRepository<CustomerConcern> CustomerConcerns { get; }
        IBaseRepository<Delivery> Deliveries { get; }
        IBaseRepository<Meal> Meals { get; }
        IBaseRepository<Order> Orders { get; }
        IBaseRepository<Restaurant> Restaurants { get; }
        IBaseRepository<Point> Points { get; }
        IBaseRepository<Address> Addresses { get; }
        void SaveChanges();
    }
}
