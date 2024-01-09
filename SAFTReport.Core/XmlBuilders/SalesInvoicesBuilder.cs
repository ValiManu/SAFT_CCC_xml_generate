using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Models;
using SAFTReport.Core.Utility;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace SAFTReport.Core.XmlBuilders
{
    public class SalesInvoicesBuilder : ISalesInvoicesBuilder
    {
        private readonly SAFTDbContext dbContext;
        private readonly DateUtility utility;

        public SalesInvoicesBuilder(SAFTDbContext context, DateUtility dateUtility)
        {
            dbContext = context;
            utility = dateUtility;
        }
        public XElement BuildSection(int month, int year)
        {
            var salesInvoicesList = dbContext.Transactions
                .Where(s => s.ReportingMonth == month
                && s.ReportingYear == year
                && s.DocumentType != "E4"
                && s.DocumentType != "AB"
                && s.DocumentType != "WB"
                && s.DocumentType != "SB"
                && s.DocumentType != "SA"
                && s.DocumentType != "SK"
                && s.DocumentType != "R5"
                && s.DocumentType != "E5"
                && s.AccountType == "D"
                && s.CustomerName != "TECHNICAL STORE (RO)")
                .ToList();

            var salesInvoices = salesInvoicesList
                .GroupBy(s => s.InvoiceNo)
                .ToDictionary(g => g.Key, g => g.ToList());

            var customers = dbContext.Customers
                .Select(c => new { c.Name, c.Country, c.AccountCCC, c.AccountId, c.City, c.FiscalCode, c.Id })
                .ToDictionary(c => c.Id, c => c);

            var euCountries = dbContext.EUCountries.ToList();

            var accounts = dbContext.Accounts.ToList();

            var taxPayments = dbContext.TaxPayments
                .Where(t => t.ReportingMonth == month && t.ReportingYear == year)
                .GroupBy(t => t.TransactionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var taxes = dbContext.Taxes.ToList();

            double totalDebit = 0;
            double totalCredit = 0;

            foreach (var item in salesInvoices.Values)
            {
                foreach (var item2 in item)
                {
                    if (double.TryParse(item2.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                    {
                        if (result > 0) totalDebit += result;
                        if (result < 0) totalCredit += result;
                    }
                }

                
            }

            XElement salesInvoicesElement = new XElement("SalesInvoices",
                new XElement("NumberOfEntries", salesInvoices.Count()),
                new XElement("TotalDebit", totalDebit.ToString("F2", CultureInfo.InvariantCulture)),
                new XElement("TotalCredit", (totalCredit * -1).ToString("F2", CultureInfo.InvariantCulture)));

            foreach (var item in salesInvoices.Values)
            {
                var firstItem = item.FirstOrDefault();
                var invoiceNo = firstItem.InvoiceNo;
                var customerInfo = customers.Values.FirstOrDefault(c => c.AccountId == firstItem.CustomerId);
                var customerId = utility.MapFiscalCode(customerInfo.Name, customerInfo.FiscalCode, customerInfo.Country, euCountries);
                var account = accounts.FirstOrDefault(a => a.AccountCCC == customerInfo.AccountCCC);
                var invoiceDate = DateTime.ParseExact(firstItem.DocumentDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                var postingDate = DateTime.ParseExact(firstItem.PostingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");

                XElement invoice = new XElement("Invoice",
                    new XElement("InvoiceNo", invoiceNo + "/" + firstItem.TransactioId),
                    new XElement("CustomerInfo",
                        new XElement("CustomerID", customerId),
                        new XElement("BillingAddress",
                            new XElement("City", customerInfo.City),
                            new XElement("Country", customerInfo.Country)
                        )
                    ),
                    new XElement("AccountID", account.AccountSAFT),
                    new XElement("InvoiceDate", invoiceDate),
                    new XElement("InvoiceType", "380"),
                    new XElement("SelfBillingIndicator", "0"),
                    new XElement("GLPostingDate", postingDate)
                );

                foreach (var i in item)
                {
                    double amount = 0;
                    double taxAmount = 0;
                    XElement taxInformation;

                    if (double.TryParse(i.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
                    {
                        amount += result;
                    }

                    if (taxPayments.TryGetValue(i.TransactioId, out var taxLines))
                    {
                        foreach (var taxLineItem in taxLines)
                        {
                            if (double.TryParse(taxLineItem.VatAmout, NumberStyles.Any, CultureInfo.InvariantCulture, out double resultAmount))
                            {
                                taxAmount += resultAmount;
                            }
                        }

                        var taxLine = taxLines.FirstOrDefault();
                        if (taxLine != null)
                        {
                            var taxCode = taxes.FirstOrDefault(tx => tx.sapId == taxLine.TaxCode);
                            if (taxCode != null)
                            {
                                taxInformation = new XElement("TaxInformation",
                                    new XElement("TaxType", taxCode.ParentTaxCode),
                                    new XElement("TaxCode", taxCode.ChildTaxCode),
                                    new XElement("TaxAmount",
                                        new XElement("Amount", taxAmount.ToString("F2", CultureInfo.InvariantCulture)),
                                        new XElement("CurrencyCode", i.Ccy),
                                        new XElement("CurrencyAmount", taxAmount.ToString("F2", CultureInfo.InvariantCulture)),
                                        new XElement("ExchangeRate", "1")
                                    )
                                );
                            }
                            else
                            {
                                taxInformation = CreateDefaultTaxInformation();
                            }
                        }
                        else
                        {
                            taxInformation = CreateDefaultTaxInformation();
                        }
                    }
                    else
                    {
                        taxInformation = CreateDefaultTaxInformation();
                    }

                    XElement invoiceLine = new XElement("InvoiceLine",
                        new XElement("LineNumber", i.Id),
                        new XElement("AccountID", account.AccountSAFT),
                        new XElement("Quantity", "1"),
                        new XElement("UnitPrice", amount.ToString("F2", CultureInfo.InvariantCulture)),
                        new XElement("TaxPointDate", postingDate),
                        new XElement("Description", i.Description),
                        new XElement("InvoiceLineAmount",
                            new XElement("Amount", amount.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("CurrencyCode", i.Ccy),
                            new XElement("CurrencyAmount", amount.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("ExchangeRate", "1")
                        ),
                        new XElement("DebitCreditIndicator", amount >= 0 ? "D" : "C"),
                        taxInformation
                    );

                    invoice.Add(invoiceLine);
                }

                salesInvoicesElement.Add(invoice);
            }

            return salesInvoicesElement;
        }

        private XElement CreateDefaultTaxInformation()
        {
            return new XElement("TaxInformation",
                new XElement("TaxType", "000"),
                new XElement("TaxCode", "000000"),
                new XElement("TaxAmount",
                    new XElement("Amount", "0.00"),
                    new XElement("CurrencyCode", "RON"),
                    new XElement("CurrencyAmount", "0.00"),
                    new XElement("ExchangeRate", "1.0000")
                )
            );
        }
    }
}
