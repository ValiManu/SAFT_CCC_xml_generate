using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class OwnersBuilder : IOwnersBuilder
    {
        public XElement BuildSection(int month, int year)
        {
            XElement owners = new XElement("Owners");

            return owners;
        }
    }
}
