using EtherScanTest;
using EtherScanTest.Infrastructure.Config;
using Polly;
using Serilog;
using EtherScanTest.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration config = hostContext.Configuration;
        var etherScanSetting = config.GetSection("EtherScan").Get<EtherScanSetting>();
        services.AddDatabase(config);
        services.AddHttpClient("Etherscan", client =>
        {
            client.BaseAddress = new Uri("https://api.etherscan.io");
        })
        .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
        {
            TimeSpan.FromSeconds(etherScanSetting.SleepDuration)
        }));
        services.AddServices();
        services.AddHostedService<Worker>();
        services.AddHttpClient(config);
    })
    .UseSerilog()
    .Build();

await host.RunAsync();
