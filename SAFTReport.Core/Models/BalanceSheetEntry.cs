using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class BalanceSheetEntry
    {
        public int Id { get; set; }
        public string? AccountCCC { get; set; }
        public double InitialDebit { get; set; }
        public double InitialCredit { get; set; }
        public double DebitTurnover { get; set; }
        public double CreditTurnover { get; set; }
        public double FinalDebit { get; set; }
        public double FinalCredit { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set; }
    }
}
