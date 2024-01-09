using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class MovementsBuilder : IMovementsBuilder
    {
        public XElement BuildSection(int month, int year)
        {
            XElement movments = new XElement("MovementTypeTable");

            return movments;
        }
    }
}
