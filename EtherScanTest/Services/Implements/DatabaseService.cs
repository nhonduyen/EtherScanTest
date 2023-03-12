using EtherScanTest.Entities.DbModel;
using EtherScanTest.Infrastructure.Data;
using EtherScanTest.Services.Interfaces;

namespace EtherScanTest.Services.Implements
{
    public class DatabaseService : IDatabaseService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DatabaseService> _logger;

        public DatabaseService(ILogger<DatabaseService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task SaveData(Block block, List<Transaction> transactions)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var _etherScanContext = services.GetService<EtherScanContext>();
                using (var transaction = _etherScanContext.Database.BeginTransaction())
                {
                    try
                    {
                        transaction.CreateSavepoint("BeforeInsertBlocksAndTransaction");
                        _logger.LogInformation("Create saving point database");
                        _etherScanContext.Blocks.Add(block);
                        var effectedBlockNumber = await _etherScanContext.SaveChangesAsync();
                        _logger.LogInformation($"Block {block.BlockNumber} saved to database. Effected rows {effectedBlockNumber}");

                        transactions = transactions.Select(t => { t.BlockId = block.BlockID; return t; }).ToList();
                        _etherScanContext.Transactions.AddRange(transactions);
                        var effectedTransNumber = await _etherScanContext.SaveChangesAsync();
                        _logger.LogInformation($"Block {block.BlockNumber} saved to database. Number of effected transactions {effectedTransNumber}");
                        await transaction.CommitAsync();
                        _logger.LogInformation($"Transaction commited");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                        transaction.RollbackToSavepoint("BeforeInsertBlocksAndTransaction");
                        _logger.LogInformation("Rolled back to saving point.");
                    }
                }
            }

        }
    }
}
