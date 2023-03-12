using EtherScanTest.Entities.DbModel;
using EtherScanTest.Entities.EtherScanModel;
using EtherScanTest.Infrastructure.Config;
using EtherScanTest.Services.Interfaces;

namespace EtherScanTest.Services.Implements
{
    public class EtherScanService : IEtherScanService
    {
        private readonly ILogger<EtherScanService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientService _httpClientService;
        private readonly EtherScanSetting _etherScanSetting;
        private readonly IDatabaseService _databaseService;

        public EtherScanService(IConfiguration configuration, ILogger<EtherScanService> logger, IHttpClientService httpClientService, IDatabaseService databaseService)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClientService = httpClientService;
            _etherScanSetting = _configuration.GetSection("EtherScan").Get<EtherScanSetting>();
            _databaseService = databaseService;
        }

        public async Task<Block> GetBlockByNumber(int blockNumber)
        {
            var result = new Block();
            var getBlockByNumberUri = String.Format(_etherScanSetting.GetBlockByNumberUrl, blockNumber.ToString("X"), _etherScanSetting.ApiKeyToken);
            var blockByNumber = await _httpClientService.RequestAsync<BlockByNumber>(getBlockByNumberUri);
            if (blockByNumber != null)
            {
                result = new Block()
                {
                    BlockNumber = Convert.ToInt32(blockByNumber.result.number, 16),
                    Hash = blockByNumber.result.hash,
                    ParentHash = blockByNumber.result.parentHash,
                    Miner = blockByNumber.result.miner,
                    GasLimit = (decimal)Convert.ToInt32(blockByNumber.result.gasLimit, 16),
                    GasUsed = (decimal)Convert.ToInt32(blockByNumber.result.gasUsed, 16)
                };
            }
            return result;
        }

        public async Task<int> GetBlockTransactionCountByNumber(int blockNumber)
        {
            var result = 0;
            var getBlockTransactionCountByNumberUri = String.Format(_etherScanSetting.GetBlockTransactionCountByNumberUrl, blockNumber.ToString("X"), _etherScanSetting.ApiKeyToken);
            var blockTransactionCountByNumber = await _httpClientService.RequestAsync<BlockTransactionCountByNumber>(getBlockTransactionCountByNumberUri);
            if (blockTransactionCountByNumber != null && blockTransactionCountByNumber.result != null)
            {
                result = Convert.ToInt32(blockTransactionCountByNumber.result, 16);
            }
            return result;
        }

        public async Task<Entities.DbModel.Transaction> GetTransactionByBlockNumberAndIndex(Block block, int indexOfTransaction)
        {
            Entities.DbModel.Transaction result = null;
            var getTransactionByBlockNumberAndIndexUri = String.Format(_etherScanSetting.GetTransactionByBlockNumberAndIndexUrl, block.BlockNumber.ToString("X"), "0x" + indexOfTransaction.ToString("X"), _etherScanSetting.ApiKeyToken);
            var transaction = await _httpClientService.RequestAsync<TransactionByBlockNumberAndIndex>(getTransactionByBlockNumberAndIndexUri);
            if (transaction != null && transaction.result != null)
            {
                result = new Entities.DbModel.Transaction
                {
                    BlockId = block.BlockID,
                    Hash = transaction.result.hash,
                    From = transaction.result.from,
                    To = transaction.result.to,
                    Gas = (decimal)Convert.ToInt32(transaction.result.gas, 16),
                    GasPrice = (decimal)Convert.ToInt32(transaction.result.gas, 16),
                    TransactionIndex = Convert.ToInt32(transaction.result.transactionIndex, 16),
                    Value = ParseHexString(transaction.result.value)
                };
            }
            return result;
        }

        private static Decimal ParseHexString(string hexNumber)
        {
            hexNumber = hexNumber.Replace("x", string.Empty);
            long result = 0;
            long.TryParse(hexNumber, System.Globalization.NumberStyles.HexNumber, null, out result);
            return result;
        }

        public async Task Process(EtherScanSetting etherScanSetting)
        {
            try
            {
                for (var i = etherScanSetting.StartBlock; i <= etherScanSetting.EndBlock; i++)
                {
                    _logger.LogInformation($"Get inforation for Block {i}");
                    var block = await GetBlockByNumber(i);
                    var transactions = new List<Entities.DbModel.Transaction>();
                    _logger.LogInformation($"Get transactions of Block {i}");
                    var blockTransactionCountByNumber = await GetBlockTransactionCountByNumber(block.BlockNumber);
                    _logger.LogInformation($"Block {block.BlockNumber} has {blockTransactionCountByNumber} transactions");

                    if (blockTransactionCountByNumber != 0)
                    {
                        for (int transIndex = 0; transIndex < blockTransactionCountByNumber; transIndex++)
                        {
                            var blockTransaction = await GetTransactionByBlockNumberAndIndex(block, transIndex);
                            if (blockTransaction != null)
                            {
                                transactions.Add(blockTransaction);
                                _logger.LogInformation($"Block {i} added transaction {transIndex} to block");
                            }
                        }
                    }
                    await _databaseService.SaveData(block, transactions);
                    _logger.LogInformation($"Block {i} saved to database");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
            
    }
}
