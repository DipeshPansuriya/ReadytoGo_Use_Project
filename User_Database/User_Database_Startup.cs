using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace User_Database
{
    public static class Users_Database_Startup
    {
        public static void AddDatabase(this IServiceCollection services, string SQLConnstr)
        {
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());
            _ = services.AddDbContext<UserDBContext>(options =>
                options.UseSqlServer(SQLConnstr));
        }
    }
}