using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class TaxPayment
    {
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public string? Description { get; set; }
        public string? TransactionId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PostingDate { get; set; }
        public string? DocumentDate { get; set; }
        public string? TaxBaseAmount { get; set; }
        public string? VatAmout { get; set; }
        public string? VatRate { get; set; }
        public string? TaxCode { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set; }
        public int FileNo { get; set; }
    }
}
