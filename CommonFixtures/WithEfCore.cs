using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    public abstract class WithEfCore<TDbContext> : WithEfCore<TDbContext, TDbContext>
        where TDbContext : DbContext
    {
    }

    public abstract class  WithEfCore<TDbContext, TDbContextImplementation> : WithIoC
        where TDbContext : DbContext
        where TDbContextImplementation : class, TDbContext
    {
        protected virtual bool UseLazyLoading => false;
        protected virtual Action<DbContextOptionsBuilder> DbContextOptionsBuilder => (opts) => { };

        /// <summary>
        /// Pulls the database to the desired state. You can call from test class constructor or top of your test method.
        /// Saves changes automatically 
        /// </summary>
        /// <param name="arrangeAction"></param>
        protected void Arrange(Action<TDbContext> arrangeAction)
        {
            arrangeAction?.Invoke(DbContext);
            DbContext.SaveChanges();
            NewServiceScope();
        }

        protected override IServiceProvider ServiceProviderFactory()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ReplaceDbContext(services);
            return services.BuildServiceProvider();
        }

        protected void ReplaceDbContext(IServiceCollection services)
        {
            DbConnection = TestDbContextHelper.ReplaceDbContext<TDbContext, TDbContextImplementation>(services, opts =>
            {
                DbContextOptionsBuilder(opts);
                if (UseLazyLoading)
                {
                    opts.UseLazyLoadingProxies();
                }
            });
        }

        protected TDbContext DbContext => GetService<TDbContext>();

        protected DbConnection DbConnection { get; private set; }

        /// <summary>
        /// Queries from test db context with given predicate expression
        /// </summary>
        protected IReadOnlyCollection<T> Query<T>(Expression<Func<T, bool>> predicate) where T : class
            => DbContext.Set<T>().Where(predicate).ToList();

        /// <summary>
        /// Gets given type of entity from test db context with id
        /// </summary>
        protected T Get<T>(object id) where T : class
            => DbContext.Set<T>().Find(id);

        public override void Dispose()
        {
            base.Dispose();
            DbConnection?.Dispose();
        }
    }
}