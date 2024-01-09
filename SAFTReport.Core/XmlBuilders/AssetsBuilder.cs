using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class AssetsBuilder : IAssetsBuilder
    {
        public XElement BuildSection(int month, int year)
        {
            XElement assets = new XElement("Assets");

            return assets;
        }
    }
}
