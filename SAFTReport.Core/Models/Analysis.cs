using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Models
{
    public class Analysis
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? TypeDescription { get; set; }
        public string? AnalysisID { get; set; }
        public string? Name { get; set; }
    }
}
