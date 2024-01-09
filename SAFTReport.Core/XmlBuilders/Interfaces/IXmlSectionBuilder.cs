using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders.Interfaces
{
    public interface IXmlSectionBuilder
    {
        XElement BuildSection(int month, int year);
    }
}
