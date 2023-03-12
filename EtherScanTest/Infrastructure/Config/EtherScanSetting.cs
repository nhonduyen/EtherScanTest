using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtherScanTest.Infrastructure.Config
{
    public class EtherScanSetting
    {
        public string ApiKeyToken { get; set; }
        public string GetBlockByNumberUrl { get; set; }
        public string GetBlockTransactionCountByNumberUrl { get; set; }
        public string GetTransactionByBlockNumberAndIndexUrl { get; set; }
        public int NumberOfCallPerSecond { get; set; }
        public int StartBlock { get; set; }
        public int EndBlock { get; set; }
        public int RetryCount { get; set; }
        public int SleepDuration { get; set; }
    }
}
