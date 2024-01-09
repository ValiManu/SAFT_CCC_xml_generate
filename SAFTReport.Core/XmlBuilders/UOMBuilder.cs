using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class UOMBuilder : IUOMBuilder
    {
        public XElement BuildSection(int month, int year)
        {
            XElement uOM = new XElement("UOMTable",
                new XElement("UOMTableEntry",
                    new XElement("UnitOfMeasure", "H87"),
                    new XElement("Description", "piece")
                    )
                );

            return uOM;
        }

    }
}
