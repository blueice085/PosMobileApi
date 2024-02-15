using Microsoft.EntityFrameworkCore;

namespace PosMobileApi.Data
{
    public static class DependencyInjection
    {
        public static void AddPersistance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<Context>(opts =>
            {
                string connString = configuration["Database:dbConnectionString"];
                Console.WriteLine(connString);
                opts.UseMySql(connString, ServerVersion.AutoDetect(connString));
            }, 128);

            services.AddTransient<Context>();

            services.AddTransient<IUow<Context>, Uow<Context>>();
        }
    }
}