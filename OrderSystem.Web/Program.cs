using Microsoft.Data.SqlClient;
using OrderSystem.Application.Services;
using OrderSystem.Application.Services.Interfaces;
using OrderSystem.Infra.Data.Repositories;
using OrderSystem.Infrastructure.Repositories;
using OrderSystem.Infrastructure.Repositories.Interfaces;
using OrderSystem.Services;
using System.Data;

namespace OrderSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IDbConnection>(sp =>
                new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddScoped<ProductService>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<OrderListRepository>();
            builder.Services.AddScoped<OrderListService>();
            builder.Services.AddScoped<ClientService>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
