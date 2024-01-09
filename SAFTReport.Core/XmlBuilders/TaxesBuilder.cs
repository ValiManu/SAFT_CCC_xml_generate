using SAFTReport.Core.DataAcces;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class TaxesBuilder : ITaxesBuilder
    {
        private readonly SAFTDbContext dbContext;

        public TaxesBuilder(SAFTDbContext context)
        {
            dbContext = context;
        }
        public XElement BuildSection(int month, int year)
        {

            XElement taxTableEntry = new XElement("TaxTableEntry",
                new XElement("TaxType", "300"),
                new XElement("Description", "Taxa pe valoarea adaugata")
                );

            var taxEntries = dbContext.Taxes;

            foreach (var t in taxEntries)
            {
                XElement taxCodeDetails = new XElement("TaxCodeDetails",
                    new XElement("TaxCode", t.ChildTaxCode),
                    new XElement("TaxPercentage", t.VATRate),
                    new XElement("BaseRate", "0"),
                    new XElement("Country", "RO")
                    );

                taxTableEntry.Add( taxCodeDetails );
            }

            XElement taxes = new XElement("TaxTable", taxTableEntry);

            return taxes;
        }
    }
}
