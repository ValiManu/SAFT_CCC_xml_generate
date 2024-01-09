using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string? DocumentType { get; set; }
        public string? SupplierId { get; set; }
        public string? CustomerId { get; set; }
        public string? AccountCCC { get; set; }
        public string? AccountType { get; set; }
        public string? AccountId { get; set; }
        public string? CustomerName { get; set; }
        public string? SupplierName { get; set; }
        public string? Description { get; set; }
        public string? TransactioId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? PostingDate { get; set; }
        public string? DocumentDate { get; set; }
        public string? Value { get; set; }
        public string? Ccy { get; set; }
        public string? ProfitCenter { get; set; }
        public string? TaxCode { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set; }
        public int FileNo { get; set; }

    }
}
