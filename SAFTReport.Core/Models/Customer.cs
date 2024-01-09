using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? AccountCCC { get; set; }
        public string? AccountId { get; set; }
        public string? FiscalCode { get; set; }
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public double InitialDebit { get; set; }
        public double InitialCredit { get; set; }
        public double FinalDebit { get; set; }
        public double FinalCredit { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set; }
    }
}
