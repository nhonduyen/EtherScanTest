using EtherScanTest.Entities.DbModel;
using EtherScanTest.Infrastructure.Config;

namespace EtherScanTest.Services.Interfaces
{
    public interface IEtherScanService
    {
        Task Process(EtherScanSetting etherScanSetting);
        Task<Block> GetBlockByNumber(int blockNumber);

        Task<int> GetBlockTransactionCountByNumber(int blockNumber);
        Task<Transaction> GetTransactionByBlockNumberAndIndex(Block block, int indexOfTransaction);
    }
}
