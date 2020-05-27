using System;
using System.Data.Common;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CommonFixtures
{
    internal static class TestDbContextHelper
    {
        /// <summary>
        /// Replaces db context with in-memory one .
        /// </summary>
        /// <param name="services"></param>
        public static DbConnection ReplaceDbContext<TDbContext, TDbContextImplementation>(IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
            where TDbContext : DbContext
            where TDbContextImplementation : class, TDbContext
        {
            var dbOptionsDependencies = services.Where(d => d.ServiceType.IsAssignableFrom(typeof(DbContextOptions))).ToList();
            foreach (var serviceDescriptor in dbOptionsDependencies)
            {
                services.Remove(serviceDescriptor);
            }

            var dbDependencies = services.Where(d => d.ServiceType.IsAssignableFrom(typeof(TDbContext))).ToList();
            foreach (var serviceDescriptor in dbDependencies)
            {
                services.Remove(serviceDescriptor);
            }

            return RegisterSqlLiteTestDbContext<TDbContext, TDbContextImplementation>(services, dbContextOptionsBuilder);
        }

        private static DbConnection RegisterSqlLiteTestDbContext<TDbContext, TDbContextImplementation>(IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsBuilder)
            where TDbContext : DbContext
            where TDbContextImplementation : class, TDbContext
        {
            var testDbContextOptionsBuilder = new DbContextOptionsBuilder<TDbContext>().UseSqlite(CreateInMemorySqLiteConnection());

            dbContextOptionsBuilder(testDbContextOptionsBuilder);

            var dbContextOptions = testDbContextOptionsBuilder.Options;
            var dbConnection = RelationalOptionsExtension.Extract(dbContextOptions).Connection;

            services.AddScoped<TDbContext, TDbContextImplementation>(sp => (TDbContextImplementation) Activator.CreateInstance(typeof(TDbContextImplementation), dbContextOptions));
            // should register as scoped to isolate dbContexts across arrange, act, assert phases through using NewServiceScope() test method.
            // This approach is more reliable to create new instance of dbContext before each phase of tests
            // If you wish use shared dbContext you can uncomment below
            // services.AddSingleton<TDbContext, TDbContextImplementation>(sp => (TDbContextImplementation) Activator.CreateInstance(typeof(TDbContextImplementation), dbContextOptions));

            return dbConnection;
        }

        private static DbConnection CreateInMemorySqLiteConnection()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        }
    }
}