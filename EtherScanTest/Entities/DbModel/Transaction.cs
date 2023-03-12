using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EtherScanTest.Entities.DbModel
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }
        [ForeignKey("Block")]
        public int BlockId { get; set; }
        public string Hash { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Value { get; set; }
        public decimal Gas { get; set; }
        public decimal GasPrice { get; set; }
        public int TransactionIndex { get; set; }
    }
}
