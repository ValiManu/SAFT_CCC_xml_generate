using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string? CoCd { get; set; }
        public string? HouseBk { get; set; }
        public string? AcctId { get; set; }
        public string? StmtId { get; set; }
        public string? StmtDate { get; set; }
        public string? ShrtKey { get; set; }
        public string? MrNo { get; set; }
        public string? Type { get; set; }
        public string? ImpDate { get; set; }
        public string? ImpTime { get; set; }
        public string? EbUser { get; set; }
        public string? DC { get; set; }
        public string? Amount { get; set; }
        public string? Ac { get; set; }
        public string? TransTyp { get; set; }
        public string? TxTk { get; set; }
        public string? ExTt { get; set; }
        public string? PostRule { get; set; }
        public string? Text { get; set; }
        public string? ProfitCtr { get; set; }
        public string? TransactionId { get; set; }
        public string? SubDocNo { get; set; }
        public string? A { get; set; }
        public string? Account { get; set; }
        public string? BankNumber { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set; }

    }
}
