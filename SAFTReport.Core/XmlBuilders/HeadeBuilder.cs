using Microsoft.IdentityModel.Tokens;
using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Utility;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class HeadeBuilder : IHeaderBuilder
    {

        private readonly SAFTDbContext dbContext;
        private readonly DateUtility dateUtility;

        public HeadeBuilder(SAFTDbContext context, DateUtility utility)
        {
            dbContext = context;
            dateUtility = utility;
        }
        public XElement BuildSection(int month, int year)
        {
            var (startDate, endDate) = dateUtility.GetReportingPeriod(month, year);
            
            XElement headerElement =
                new XElement("Header",
                    new XElement("AuditFileVersion", "2.0"),
                    new XElement("AuditFileCountry", "RO"),
                    new XElement("AuditFileDateCreated", DateTime.Now.ToString("yyyy-MM-dd")),
                    new XElement("SoftwareCompanyName", "SHOE EXPRESS"),
                    new XElement("SoftwareID", "SAF-T Reporting"),
                    new XElement("SoftwareVersion", "1.0.0"),
                    new XElement("Company",
                        new XElement("RegistrationNumber", "RO30490303"),
                        new XElement("Name", "SHOE EXPRESS SA"),
                        new XElement("Address",
                            new XElement("StreetName", "Sos. Pipera"),
                            new XElement("Number", "42"),
                            new XElement("Building", "Globalworth Plaza"),
                            new XElement("City", "Bucuresti"),
                            new XElement("PostalCode", "20309"),
                            new XElement("Region", "RO-B"),
                            new XElement("Country", "RO"),
                            new XElement("AddressType", "PostalAddress")
                            ),
                        new XElement("Contact",
                            new XElement("ContactPerson",
                                new XElement("FirstName", "Carmen"),
                                new XElement("LastName", "Mihalcea")
                                ),
                            new XElement("Telephone", "078834325")
                            ),
                        new XElement("TaxRegistration",
                            new XElement("TaxRegistrationNumber", "30490303")
                            ),
                        new XElement("BankAccount",
                            new XElement("IBANNumber", "RO39BACX0000001606000000")
                            )
                        ),
                    new XElement("DefaultCurrencyCode", "RON"),
                    new XElement("SelectionCriteria",
                        new XElement("SelectionStartDate", startDate),
                        new XElement("SelectionEndDate", endDate)
                        ),
                    new XElement("HeaderComment", "L"),
                    new XElement("SegmentIndex", "1"),
                    new XElement("TotalSegmentsInsequence", "1"),
                    new XElement("TaxAccountingBasis", "A")
                    );


            return headerElement;
        }
    }
}
