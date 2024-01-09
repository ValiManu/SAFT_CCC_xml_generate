using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Constant
    {
        public int Id { get; set; }
        public int ReportingMonth { get; set; }
        public int ReportingYear { get; set;}
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

    }
}
