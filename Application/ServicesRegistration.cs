using InvestmentApp.Core.Application.Interfaces;
using InvestmentApp.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InvestmentApp.Core.Application
{
    public static class ServicesRegistration
    {
        //Extension method - Decorator pattern
        public static void AddApplicationLayerIoc(this IServiceCollection services)
        {    
            #region Services IOC
            services.AddTransient<IAssetService, AssetService>();
            services.AddTransient<IAssetTypeService, AssetTypeService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IInvestmentPortfolioService, InvestmentPortfolioService>();
            services.AddTransient<IAssetHistoryService, AssetHistoryService>();
            services.AddTransient<IInvestmentAssetsService, InvestmentAssetsService>();
            #endregion
        }

    }
}
