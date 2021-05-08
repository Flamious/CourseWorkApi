using Course.DAL.Entities;
using Course.DAL.Interfaces;
using Course.DAL.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Ninject
{
    public static class Connection
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(typeof(DbContext), typeof(PostContext));
            services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<PostContext>();
            services.AddScoped(typeof(IRepository<User>), typeof(UserRepository));
            services.AddScoped(typeof(IRepository<Post>), typeof(PostRepository));
            services.AddScoped(typeof(IRepository<Commentary>), typeof(CommentaryRepository));
            services.AddScoped(typeof(IRepository<Assessment>), typeof(AssessmentRepository));
            services.AddScoped(typeof(IDbRepository), typeof(DbRepository));
            services.AddDbContext<PostContext>(options => options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));



            return services;
        }
    }
}
