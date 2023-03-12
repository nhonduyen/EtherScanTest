using EtherScanTest.Infrastructure.Config;
using EtherScanTest.Services.Interfaces;

namespace EtherScanTest
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;
        private readonly IEtherScanService _etherScanService;

        public Worker(IConfiguration configuration,ILogger<Worker> logger, IEtherScanService etherScanService)
        {
            _logger = logger;
            _configuration = configuration;
            _etherScanService = etherScanService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var etherScanSetting = _configuration.GetSection("EtherScan").Get<EtherScanSetting>();
            await _etherScanService.Process(etherScanSetting);
            _logger.LogInformation("Process is done");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}