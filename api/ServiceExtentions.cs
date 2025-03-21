using api.Data.Implementation;
using api.Data.Interface;
using api.Repositories.Implementation;
using api.Repositories.Interface;
using api.Services;
using api.Services.IService;

namespace api
{
    public static class ServiceExtentions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IFactoryDbContext, FactoryDbContext>();

            services.AddScoped<IShortUrlService, ShortUrlService>();


            services.AddScoped<IShortUrlRepository, ShortUrlRepositories>();
            services.AddScoped<IRedisRepository, RedisRepository>();
            return services;
        }

        public static IServiceCollection RegisterFrameworkServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            return services;
        }
    }
}
