using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherScanTest.Entities.DbModel
{
    public class Block
    {
        [Key]
        public int BlockID { get; set; }
        public int BlockNumber { get; set; }
        public string Hash { get; set; }
        public string ParentHash { get; set; }
        public string Miner { get; set; }
        public decimal GasLimit { get; set; }
        public decimal GasUsed { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
