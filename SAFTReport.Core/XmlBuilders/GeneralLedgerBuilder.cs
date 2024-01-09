using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Models;
using SAFTReport.Core.Utility;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class GeneralLedgerBuilder: IGeneralLedgerBuilder
    {
        private readonly SAFTDbContext dbContext;

        public GeneralLedgerBuilder(SAFTDbContext context)
        {
            dbContext = context;
        }

        public XElement BuildSection(int month, int year)
        {
            var entries = from b in dbContext.BalanceSheet
                          where b.ReportingYear == year && b.ReportingMonth == month && (b.AccountCCC != null &&!b.AccountCCC.Contains("T"))
                          join a in dbContext.Accounts on b.AccountCCC equals a.AccountCCC
                          group b by new {a.AccountSAFT, a.Name, a.Type} into grouped
                          select new
                          {
                              AccountID = grouped.Key.AccountSAFT,
                              AccountType = grouped.Key.Type,
                              AccountDescription= grouped.Key.Name,
                              OpeningDebit = grouped.Key.AccountSAFT.StartsWith("6") ? 0 : (grouped.Key.Type == "Activ" || grouped.Key.Type == "Bifunctional" ? grouped.Sum(g => g.InitialDebit + g.InitialCredit) : 0),
                              OpeningCredit = grouped.Key.AccountSAFT.StartsWith("7") ? 0 : (grouped.Key.Type == "Pasiv" ? grouped.Sum((g => g.InitialCredit + g.InitialDebit)): 0),
                              ClosingDebit = grouped.Key.AccountSAFT.StartsWith("6") ? grouped.Sum(g => g.DebitTurnover - g.CreditTurnover) : (grouped.Key.Type == "Activ" || grouped.Key.Type == "Bifunctional" ? grouped.Sum(g => g.FinalDebit + g.FinalCredit) : 0),
                              ClosingCredit = grouped.Key.AccountSAFT.StartsWith("7") ? grouped.Sum(b => b.CreditTurnover - b.DebitTurnover) : (grouped.Key.Type == "Pasiv" ? grouped.Sum(b => b.FinalCredit + b.FinalDebit) : 0),
                          };

            XElement generalLedger = new XElement("GeneralLedgerAccounts");

            if (entries != null)
            {
                foreach (var  entry in entries)
                {
                    XElement accountElement = new XElement("Account",
                       new XElement("AccountID", entry.AccountID.ToString()),
                       new XElement("AccountDescription", entry.AccountDescription.ToString()),
                       new XElement("AccountType", entry.AccountType.ToString())
                       );

                    if(entry.AccountType == "Activ" || entry.AccountType == "Bifunctional" || entry.AccountID.StartsWith("6"))
                    {
                        accountElement.Add(new XElement("OpeningDebitBalance", entry.OpeningDebit.ToString("F2")));
                        accountElement.Add(new XElement("ClosingDebitBalance", entry.ClosingDebit.ToString("F2")));
                    }

                    if (entry.AccountType == "Pasiv" || entry.AccountID.StartsWith("7"))
                    {
                        accountElement.Add(new XElement("OpeningCreditBalance", entry.OpeningCredit.ToString("F2")));
                        accountElement.Add(new XElement("ClosingCreditBalance", entry.ClosingCredit.ToString("F2")));
                    }


                    generalLedger.Add(accountElement);
                    
                }
            }

            return generalLedger;
           
        }
    }
}
