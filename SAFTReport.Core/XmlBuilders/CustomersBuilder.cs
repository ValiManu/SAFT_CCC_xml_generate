using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Utility;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class CustomersBuilder : ICustomersBuilder
    {
        private readonly SAFTDbContext dbContext;
        private readonly DateUtility utility;

        public CustomersBuilder(SAFTDbContext context, DateUtility dateUtility)
        {
            dbContext = context;
            utility = dateUtility;
        }

        public XElement BuildSection(int month, int year)
        {

            XElement customers = new XElement("Customers");

            var clients = from c in dbContext.Customers
                           where c.ReportingMonth == month && c.ReportingYear == year && c.Name != "TECHNICAL STORE (RO)" && c.Name != "Zaliczka Prepaid"
                           join e in dbContext.EUCountries on c.Country equals e.CountryCode into ec
                           from e in ec.DefaultIfEmpty()
                           join a in dbContext.Accounts on c.AccountCCC equals a.AccountCCC into ac
                           from a in ac.DefaultIfEmpty()
                           select new
                           {
                               registrationNumber = c.FiscalCode,
                               name = c.Name != null ? c.Name.Replace("&", " ") : " ",
                               city = c.City,
                               country = c.Country,
                               accountId = a.AccountSAFT,
                               openDebit = Convert.ToDouble(c.InitialDebit - c.InitialCredit),
                               closeDebit = Convert.ToDouble(c.FinalDebit - c.FinalCredit),
                           };
            var EUContries = dbContext.EUCountries.ToList();

            foreach ( var c in clients )
            {
                var customerId = utility.MapFiscalCode(c.name, c.registrationNumber, c.country, EUContries);
                var customerName = Regex.Replace(c.name.Normalize(NormalizationForm.FormD), @"\p{Mn}", "");

                XElement customerElement = new XElement("Customer",
                    new XElement("CompanyStructure",
                        new XElement("RegistrationNumber", customerId ),
                        new XElement("Name", customerName),
                        new XElement("Address",
                            new XElement("City", c.city),
                            new XElement("Country", c.country)
                            )
                        ),
                    new XElement("CustomerID", customerId),
                    new XElement("AccountID", c.accountId),
                    new XElement("OpeningDebitBalance", c.openDebit.ToString("F2")),
                    new XElement("ClosingDebitBalance", c.closeDebit.ToString("F2"))
                    );

                customers.Add(customerElement);
            }

            return customers;
        }
    }
}
