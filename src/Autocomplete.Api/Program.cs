using Autocomplete.Api.Models;
using Autocomplete.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Autocomplete.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder
                        .ConfigureServices((context, services) =>
                            services
                                .Configure<RedisSettings>(options =>
                                    context.Configuration.GetSection(nameof(RedisSettings)).Bind(options))
                                .AddSingleton<ISearcher<ProductModel>, ProductSearcher>()
                                .AddControllers()
                        )
                        .Configure(app =>
                            app
                                .UseHttpsRedirection()
                                .UseRouting()
                                .UseEndpoints(endpoints =>
                                    endpoints.MapControllers())
                        ));
    }
}
