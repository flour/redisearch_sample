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
        private const string ENV_PREFIX = "AUTOCOMPLETE_API_";

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, conf) =>
                    conf.AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables(ENV_PREFIX))
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
                                .UseRouting()
                                .UseEndpoints(endpoints =>
                                    endpoints.MapControllers())
                        ));
    }
}
