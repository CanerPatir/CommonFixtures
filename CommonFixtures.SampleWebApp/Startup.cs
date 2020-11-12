using CommonFixtures.SampleWebApp.Model;
using CommonFixtures.SampleWebApp.Services;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommonFixtures.SampleWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddMediatR(GetType().Assembly);

#if NET5_0
            services.AddDbContext<ApplicationDbContext>(opts => { opts.UseMySql("<my sql conn str>", ServerVersion.AutoDetect("<my sql conn str>")); });
#elif NETCOREAPP3_1
            services.AddDbContext<ApplicationDbContext>(opts => { opts.UseMySql("<my sql conn str>"); });
#endif
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ProductService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });
        }
    }
}