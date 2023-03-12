using EtherScanTest.Infrastructure.Config;
using EtherScanTest.Infrastructure.Data;
using EtherScanTest.Services.Implements;
using EtherScanTest.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Timeout;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EtherScanTest.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<EtherScanContext>(option =>
            {
                option.UseMySQL(configuration.GetConnectionString("ConnectionString"), providerOptions => providerOptions.CommandTimeout(120));
            });
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IHttpClientService, HttpClientService>()
                .AddTransient<IEtherScanService, EtherScanService>()
                .AddTransient<IDatabaseService, DatabaseService>();
        }

        public static IServiceCollection AddHttpClient(this IServiceCollection services, IConfiguration configuration)
        {
            var etherScanSetting = configuration.GetSection("EtherScan").Get<EtherScanSetting>();
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
                    .CreateLogger();

            var httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
            .Or<HttpRequestException>()
            .Or<TimeoutRejectedException>() 
            .WaitAndRetryAsync(etherScanSetting.RetryCount, retryAttempt =>
            TimeSpan.FromSeconds(etherScanSetting.SleepDuration),
            onRetry: (exception, sleepDuration, attemptNumber, context) =>
            {
                Log.Logger.Error($"Call api error. Retrying in {sleepDuration}. {attemptNumber} / {etherScanSetting.RetryCount}");
            });
            
            return services.AddSingleton<IAsyncPolicy<HttpResponseMessage>>(httpRetryPolicy);
        }

    }
}
