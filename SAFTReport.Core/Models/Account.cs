using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string? AccountId { get; set; }
        public string? AccountCCC { get; set; }
        public string? AccountSAFT { get; set; }

        public string? Name { get; set; }
        public string? Type { get; set; }
    }
}
