using InvestmentApp.Core.Domain.Interfaces;
using InvestmentApp.Infrastructure.Persistence.Contexts;
using InvestmentApp.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentApp.Infrastructure.Persistence
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration config)
        {
            #region Contexts
            if (config.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<InvestmentAppContext>(opt =>
                                              opt.UseInMemoryDatabase("AppDb"));
            }
            else
            {
                var connectionString = config.GetConnectionString("DefaultConnection");
                services.AddDbContext<InvestmentAppContext>(opt =>
                opt.UseSqlServer(connectionString,
                m=> m.MigrationsAssembly(typeof(InvestmentAppContext).Assembly.FullName))
                , ServiceLifetime.Transient);
            }
            #endregion

            #region Repositories IOC
            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<IAssetRepository, AssetRepository>();
            services.AddTransient<IAssetTypeRepository, AssetTypeRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IInvestmentPortfolioRepository, InvestmentPortfolioRepository>();
            services.AddTransient<IInvestmentAssetRepository, InvestmentAssetRepository>();
            services.AddTransient<IAssetHistoryRepository, AssetHistoryRepository>();
            #endregion
        }
    }
}
