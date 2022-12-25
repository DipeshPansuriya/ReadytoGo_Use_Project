using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using User_Infrastructure.Interface;

namespace User_Infrastructure
{
    public static class User_Infrastructure_Startup
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            _ = services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            _ = services.AddTransient<ICacheService, CacheService>();
            _ = services.AddScoped<CacheService>();
            _ = services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            _ = services.AddSingleton<IBackgroundJob, BackgroundJob>();
            _ = services.AddTransient<INotificationMsg, NotificationMsg>();
            _ = services.AddScoped<NotificationMsg>();
            _ = services.AddTransient<IDapperRepository, DapperRepository>();
            _ = services.AddScoped<IResponse_Request, Response_Request>();

            return services;
        }
    }
}