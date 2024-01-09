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
    public class AccountsBuilders : IAccountsBuilder
    {
        private readonly SAFTDbContext dbContext;
        public AccountsBuilders(SAFTDbContext context)
        {
             dbContext = context;
        }
        public XElement BuildSection(int month, int year)
        {
            XElement section = new XElement("Account");

            return section;
        }
    }
}
