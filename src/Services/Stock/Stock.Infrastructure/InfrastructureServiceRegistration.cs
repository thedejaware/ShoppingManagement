using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Stock.Application.Contracts.Infrastructure;
using Stock.Application.Contracts.Persistence;
using Stock.Application.Models;
using Stock.Infrastructure.Persistence;
using Stock.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stock.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {

            services.AddScoped<IStockRepository, StockRepository>();

            services.AddScoped<IStockContext, StockContext>();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IStockRepository, StockRepository>();

            services.AddScoped<IStockContext, StockContext>();

            return services;
        }
    }
}
