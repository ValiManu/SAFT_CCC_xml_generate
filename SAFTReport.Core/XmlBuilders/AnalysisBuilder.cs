using SAFTReport.Core.DataAcces;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class AnalysisBuilder : IAnalysisBuilder
    {
        private readonly SAFTDbContext dbContext;

        public AnalysisBuilder(SAFTDbContext context)
        {
            dbContext = context;
        }
        public XElement BuildSection(int month, int year)
        {
            XElement analysis = new XElement("AnalysisTypeTable");

            var analysisEntries = dbContext.Analyses;

            foreach (var a in analysisEntries)
            {
                XElement analysisTypeTableEntry = new XElement("AnalysisTypeTableEntry",
                    new XElement("AnalysisType", a.Type),
                    new XElement("AnalysisTypeDescription", a.TypeDescription),
                    new XElement("AnalysisID", a.AnalysisID),
                    new XElement("AnalysisIDDescription", a.Name)
                    );

                analysis.Add(analysisTypeTableEntry);
            }

            return analysis;
        }
    }
}
