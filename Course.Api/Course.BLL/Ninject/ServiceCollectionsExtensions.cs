using Course.BLL.Interfaces;
using Course.BLL.Services;
using Course.DAL.Ninject;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.BLL.Ninject
{
    public static class ServiceCollectionsExtensions
    {
        public static IServiceCollection RegisterBllServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterRepositories(configuration);
            services.AddScoped<IDbCrud, DbCrudOperations>();
            services.AddScoped<IDbInitializer, DbInitializer>();
            services.AddScoped<IJwtGenerator, JwtGenerator>();
            services.AddScoped<IAuthorizationService, AuthorizationService>();

            return services;
        }
    }
}
