using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Tax
    {
        public int Id { get; set; }
        public string? sapId { get; set; }
        public  int VATRate { get; set; }
        public int ParentTaxCode { get; set; }
        public int ChildTaxCode { get; set; }
        public string? ParentTaxName { get; set; }
        public string? ChildTaxName { get; set; }
    }
}
