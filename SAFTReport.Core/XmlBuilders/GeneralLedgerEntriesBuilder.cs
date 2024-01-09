using SAFTReport.Core.DataAcces;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml.Linq;
using SAFTReport.Core.Models;
using Transaction = SAFTReport.Core.Models.Transaction;
using System.ComponentModel.DataAnnotations;
using SAFTReport.Core.Utility;
using Microsoft.EntityFrameworkCore;

namespace SAFTReport.Core.XmlBuilders
{
    public class GeneralLedgerEntriesBuilder : IGeneralLedgerEntriesBuilder
    {
        private readonly SAFTDbContext dbContext;
        private readonly DateUtility utility;

        public GeneralLedgerEntriesBuilder(SAFTDbContext context, DateUtility dateUtility)
        {
            dbContext = context;
            utility = dateUtility;
        }
        public XElement BuildSection(int month, int year)
        {

            var transactionsList = dbContext.Transactions.Where(t => t.ReportingMonth == month && t.ReportingYear == year && t.AccountCCC !=null && !t.AccountCCC.StartsWith("8") && !t.AccountCCC.StartsWith("9")).ToList();
            
            var customersInfo = dbContext.Customers
                .Select(c => new {c.Name, c.Country, c.AccountCCC, c.AccountId, c.City, c.FiscalCode, c.Id})
                .ToDictionary(c => c.Id, c => c);
           
            var suppliersInfo = dbContext.Suppliers
                .Select(s => new {s.Id, s.Name, s.Country, s.AccountCCC, s.AccountId, s.City, s.FiscalCode})
                .ToDictionary(s => s.Id, s => s);
           
            var euCountries = dbContext.EUCountries.ToList();
           
            var accounts = dbContext.Accounts.ToList();

            var transactionsDict = transactionsList
                .GroupBy(t => t.TransactioId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var taxPayments = dbContext.TaxPayments
                .Where(t => t.ReportingMonth == month &&  t.ReportingYear == year)
                .GroupBy(t => t.TransactionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var taxes = dbContext.Taxes.ToList();

            double totalDebit = 0;
            double totalCredit = 0;

            foreach (var tran in transactionsList)
            {

                if (double.TryParse(tran.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                {
                    if (result > 0) totalDebit += result;
                    if (result < 0) totalCredit += result;
                }
            }

            totalCredit = totalCredit * -1;

            XElement section = new XElement("Section",
                new XElement("NumberOfEntries", transactionsDict.Count()),
                new XElement("TotalDebit", totalDebit.ToString("F2", CultureInfo.InvariantCulture)),
                new XElement("TotalCredit", totalCredit.ToString("F2", CultureInfo.InvariantCulture))
                );

            XElement journal = new XElement("Journal",
                new XElement("JournalID", "HO"), // exemplu de ID, trebuie ajustat conform logicii dumneavoastră
                new XElement("Description", "Sediu central"),
                new XElement("Type", "HO")
                );


            var p = 0;
            foreach (var tranId in transactionsDict.Keys)
            {


                var transaction = transactionsDict[tranId].First();
                var transactionList = transactionsDict[tranId];
                var postingDate = DateTime.ParseExact(transaction.PostingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                var customer = customersInfo.Values.FirstOrDefault(c => c.AccountId == transaction.CustomerId);
                var supplier = suppliersInfo.Values.FirstOrDefault(s => s.AccountId == transaction.SupplierId);

                var customerID = "0";
                var supplierID = "0";

                if (customer == null && supplier == null)
                {
                    customerID = "0030490303";
                    supplierID = "0030490303";

                }

                if (customer != null && customer.Name != null && customer.FiscalCode != null && customer.Country != null)
                {
                    customerID = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountries);
                }

                if (supplier != null && supplier.Name != null && supplier.FiscalCode != null && supplier.Country != null)
                {
                    supplierID = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountries);
                }

                XElement transactionElement = new XElement("Transaction",
                       new XElement("TransactionID", transaction.TransactioId),
                       new XElement("Period", transaction.ReportingMonth),
                       new XElement("PeriodYear", transaction.ReportingYear),
                       new XElement("TransactionDate", postingDate),
                       new XElement("Description", transaction.Description),
                       new XElement("SystemEntryDate", postingDate),
                       new XElement("GLPostingDate", postingDate),
                       new XElement("CustomerID", customerID),
                       new XElement("SupplierID", supplierID)
                    );


                foreach (var line in transactionList)
                {

                    var accountID = accounts.FirstOrDefault(a => a.AccountCCC == line.AccountCCC).AccountSAFT;

                    XElement transactionLine = new XElement("TransactionLine",
                        new XElement("RecordID", line.Id),
                        new XElement("AccountID", accountID),
                        new XElement("CustomerID", customerID),
                        new XElement("SupplierID", supplierID),
                        new XElement("Description", line.Description)
                        );

                    var value = double.TryParse(line.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0;
                    XElement amount;

                    if (value >= 0)
                    {
                        amount = new XElement("DebitAmount",
                            new XElement("Amount", value.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("CurrencyCode", line.Ccy),
                            new XElement("CurrencyAmount", value.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("ExchangeRate", "1")
                            );
                    }
                    else
                    {

                        amount = new XElement("CreditAmount",
                            new XElement("Amount", (value * -1).ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("CurrencyCode", line.Ccy),
                            new XElement("CurrencyAmount", (value * -1).ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("ExchangeRate", "1")
                            );

                    }

                    transactionLine.Add(amount);


                    XElement taxInformation;

                    if (taxPayments.TryGetValue(line.TransactioId, out var taxLines))
                    {
                        var taxLine = taxLines.FirstOrDefault();

                        double taxAmount = 0;

                        foreach (var i in taxLines)
                        {
                            if (double.TryParse(i.VatAmout, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultAmount))
                            {
                                if (resultAmount > 0) taxAmount += resultAmount;
                            }
                        }

                        if (taxLine != null && (line.AccountType == "D" || line.AccountType == "K"))
                        {
                            var taxCode = taxes.FirstOrDefault(tx => tx.sapId == taxLine.TaxCode);
                            if (taxCode != null)
                            {
                                taxInformation = new XElement("TaxInformation",
                                    new XElement("TaxType", taxCode.ParentTaxCode),
                                    new XElement("TaxCode", taxCode.ChildTaxCode),
                                    new XElement("TaxAmount",
                                        new XElement("Amount", taxAmount.ToString("F2", CultureInfo.InvariantCulture)),
                                        new XElement("CurrencyCode", line.Ccy),
                                        new XElement("CurrencyAmount", taxAmount.ToString("F2", CultureInfo.InvariantCulture))
                                    )
                                );
                            }
                            else
                            {
                                taxInformation = CreateDefaultTaxInformation();

                            }
                        } else
                        {
                            taxInformation = CreateDefaultTaxInformation();
                        }


                    }
                    else
                    {
                        taxInformation = CreateDefaultTaxInformation();
                    }


                    transactionLine.Add(taxInformation);
                    transactionElement.Add(transactionLine);
                }


                journal.Add(transactionElement);
                p = p + 1;
                Console.WriteLine("Id " + p);


            }

            section.Add(journal);

            return section;
        }

        private XElement CreateDefaultTaxInformation()
        {
            return new XElement("TaxInformation",
                new XElement("TaxType", "000"),
                new XElement("TaxCode", "000000"),
                new XElement("TaxAmount",
                    new XElement("Amount", "0.00"),
                    new XElement("CurrencyCode", "RON"),
                    new XElement("CurrencyAmount", "0.00")
                )
            );
        }
    }
}
