using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Utility
{
    public class DateUtility
    {
        private readonly SAFTDbContext db;

        public DateUtility(SAFTDbContext context)
        {
            db = context;
        }

        public (string? startDate, string? endDate) GetReportingPeriod(int month, int year)
        {
            var period = db.Constants.FirstOrDefault(c => c.ReportingMonth == month && c.ReportingYear == year);

            string? startDate = "";
            string? endDate = "";

            if (period != null)
            {
                startDate = period.StartDate;
                endDate = period.EndDate;
            }

            return (startDate, endDate);
        }

        public string MapFiscalCode(string name, string fiscalCode, string country, List<EUCountry> eUCountries)
        {

            var excludedPartners = new List<string>
            {
                "TECHNICAL STORE (RO)",
                "ZALICZKA PREPAID",
                "",
            };

            var internalPartners = new List<string>
            {
                "BP ONE TIME ECOM",
                "BP ONE TIME (RO)",
                "EMPLOYEE ONE TIME (RO)"
            };

            if(excludedPartners.FirstOrDefault(n => n == name) != null)
            {
                return "0030490303";
            }

            if(internalPartners.FirstOrDefault(n => n == name) != null)
            {
                return "0030490303";
            }

            if (fiscalCode == null)
            {
                return "0030490303";
            }

            if (fiscalCode.ToUpper().StartsWith("RO"))
            {
                return "00" + fiscalCode.ToUpper().Substring(2);
            }

            if(country.ToUpper() == "RO")
            {
                return "00" + fiscalCode;
            }

            var EUCountry = eUCountries.FirstOrDefault(e => e.CountryCode == country.ToUpper());

            if(EUCountry != null)
            {
                if(fiscalCode.Substring(0,2) == EUCountry.CountryCode)
                {
                    return "01" + fiscalCode;
                } else
                {
                    return "01" + EUCountry.CountryCode + fiscalCode;
                }
            }

            

            return "02" + country.ToUpper() + fiscalCode;
        }

        public void UpdateDecimalSeparatorInTransactions(List<Transaction> transactions)
        {
            var transactionToUpdate = transactions.Where(t => t.Value != null && t.Value.Contains(","));

            foreach(var t in transactionToUpdate) 
            {
                if (t.Value != null)
                {
                    t.Value = t.Value.Replace(',', '.');
                }
            }

            db.SaveChanges();
        }
    }
}
