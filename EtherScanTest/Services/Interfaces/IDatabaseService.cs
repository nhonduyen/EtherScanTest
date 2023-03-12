using EtherScanTest.Entities.DbModel;

namespace EtherScanTest.Services.Interfaces
{
    public interface IDatabaseService
    {
        Task SaveData(Block block, List<Transaction> transactions);
    }
}
