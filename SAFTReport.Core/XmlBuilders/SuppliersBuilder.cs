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
    public class SuppliersBuilder : ISupplierBuilders
    {
        private readonly SAFTDbContext dbContext;
        private readonly DateUtility utility;

        public SuppliersBuilder(SAFTDbContext context, DateUtility dateUtility)
        {
            dbContext = context;
            utility = dateUtility;
        }

        public XElement BuildSection(int month, int year)
        {

            XElement suppliers = new XElement("Suppliers");

            var vendors = from s in dbContext.Suppliers
                          where s.ReportingMonth == month && s.ReportingYear == year && s.Name != "TECHNICAL STORE (RO)" && s.Name != "Zaliczka Prepaid"
                          join e in dbContext.EUCountries on s.Country equals e.CountryCode into es
                          from e in es.DefaultIfEmpty()
                          join a in dbContext.Accounts on s.AccountCCC equals a.AccountCCC into asp
                          from a in asp.DefaultIfEmpty()
                          select new
                          {
                              registrationNumber = s.FiscalCode,
                              name = s.Name != null ? s.Name.Replace("&", " ") : " ",
                              city = s.City,
                              country = s.Country,
                              accountId = a.AccountSAFT,
                              openDebit = Convert.ToDouble(s.InitialCredit - s.InitialDebit),
                              closeDebit = Convert.ToDouble(s.FinalCredit - s.FinalDebit),
                          };
            var EUContries = dbContext.EUCountries.ToList();

            foreach (var v in vendors)
            {
                var vendorId = utility.MapFiscalCode(v.name, v.registrationNumber, v.country, EUContries);
                var vendorName = Regex.Replace(v.name.Normalize(NormalizationForm.FormD), @"\p{Mn}", "");

                XElement vendorElement = new XElement("Supplier",
                    new XElement("CompanyStructure",
                        new XElement("RegistrationNumber", vendorId),
                        new XElement("Name", vendorName),
                        new XElement("Address",
                            new XElement("City", v.city),
                            new XElement("Country", v.country)
                            )
                        ),
                    new XElement("SupplierID", vendorId),
                    new XElement("AccountID", v.accountId),
                    new XElement("OpeningDebitBalance", v.openDebit.ToString("F2")),
                    new XElement("ClosingDebitBalance", v.closeDebit.ToString("F2"))
                    );

                suppliers.Add(vendorElement);
            }

            return suppliers;
        }
    }
}
