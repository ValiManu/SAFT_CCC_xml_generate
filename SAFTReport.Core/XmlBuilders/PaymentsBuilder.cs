using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Models;
using SAFTReport.Core.Utility;
using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.XmlBuilders
{
    public class PaymentsBuilder : IPaymentsBuilder
    {
        private readonly SAFTDbContext dbContext;
        private readonly DateUtility utility;

        public PaymentsBuilder(SAFTDbContext context, DateUtility dateUtility)
        {
            dbContext = context;
            utility = dateUtility; 
        }
        public XElement BuildSection(int month, int year)
        {
            
            var transactionPayments = dbContext.Transactions
                .Where(t => t.ReportingMonth == month &&
                       t.ReportingYear == year &&
                       t.AccountId == "1401107010" ||
                       t.AccountId == "1433107010" ||
                       t.AccountId == "1010000100" ||
                       t.AccountId == "1403102010" ||
                       t.AccountId == "1403107010")
                .Join(dbContext.Payments.Where(p => p.ReportingMonth == month && p.ReportingYear == year), 
                       transaction => transaction.TransactioId,
                       payment => payment.TransactionId,
                       (transaction, payment) => new {Transaction = transaction, Payment = payment}).ToList();

            var accounts = dbContext.Accounts.ToList();

            var euCountrie = dbContext.EUCountries.ToList();

            var suppliers = dbContext.Suppliers
                .Select(s => new {s.Name, s.Country, s.AccountCCC, s.AccountId, s.City, s.FiscalCode })
                .GroupBy(s => s.AccountId)
                .ToDictionary(s => s.Key, s => s.First());

            var customers = dbContext.Customers
                .Select(s => new {s.Name, s.Country, s.AccountCCC, s.AccountId, s.City, s.FiscalCode })
                .GroupBy(s => s.AccountId)
                .ToDictionary(s => s.Key, s => s.First());

            int dicKey = 0;

            Dictionary<int, PaymentModel> payments = new Dictionary<int, PaymentModel>();

            foreach(var tp in transactionPayments)
            {
                var transaction = tp.Transaction;
                var payment = tp.Payment;
                var paymentModel = new PaymentModel();

                var amount = double.TryParse(transaction.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0;
                paymentModel.PaymentRefNo = transaction.TransactioId;
                paymentModel.TransactionDate = DateTime.ParseExact(transaction.PostingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                paymentModel.PaymentMothod = "03";
                paymentModel.Description = transaction.Description;
                paymentModel.PaymentLine.AccountId = accounts.FirstOrDefault(a => a.AccountCCC == transaction.AccountCCC).AccountSAFT;
                paymentModel.PaymentLine.DebitCreditIndicator = amount > 0 ? "D" : "C";
                paymentModel.PaymentLine.Amount = amount;
                paymentModel.PaymentLine.CurrencyCode = transaction.Ccy;
                paymentModel.PaymentLine.CurrencyAmount = amount.ToString("F2", CultureInfo.InvariantCulture);
                paymentModel.PaymentLine.ExchangeRate = "1";
                paymentModel.PaymentLine.TaxType = "000";
                paymentModel.PaymentLine.TaxCode = "000000";
                paymentModel.PaymentLine.TaxAmount = "0.00";
                paymentModel.PaymentLine.TaxCurrencyCode = transaction.Ccy;
                paymentModel.PaymentLine.TaxCurrencyAmount = "0.00";
                paymentModel.PaymentLine.TaxExchangeRate = "1";

                if (transaction.AccountId == "1401107010")
                {
                    if (payment.ExTt != "66" && payment.ExTt != "72")
                    {
                        paymentModel.PaymentLine.CustomerId = "0030490303";
                        paymentModel.PaymentLine.SupplierId = "0030490303";
                        payments.Add(dicKey++, paymentModel);

                    }

                    if (payment.ExTt == "66")
                    {
                        if (payment.BankNumber == "/OB//RO49BTRL03")
                        {
                            paymentModel.PaymentLine.CustomerId = "0021611392";
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }

                        if (payment.BankNumber == "/OB//RO80BACX00")
                        {
                            paymentModel.PaymentLine.CustomerId = "01PL7792308495";
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }
                        if (payment.BankNumber == "/OB/RZBRROBU RO")
                        {

                            paymentModel.PaymentLine.CustomerId = "0013838336";
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }

                        if (payment.BankNumber == "/OB/BTRLRO22 RO")
                        {
                            paymentModel.PaymentLine.CustomerId = "0013838336";
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }
                    }

                    if (payment.ExTt == "72")
                    {
                        var supplier = suppliers[payment.Account];
                        paymentModel.PaymentLine.CustomerId = "0";
                        paymentModel.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);
                        payments.Add(dicKey++, paymentModel);
                    }
                }
                if (transaction.AccountId == "1433107010")
                {

                    if (payment.HouseBk == "UNE01")
                    {
                        if (payment.A == "D")
                        {
                            var customer = customers[payment.Account];
                            paymentModel.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }
                        else
                        {
                            if (payment.PostRule == "+N7")
                            {
                                paymentModel.PaymentLine.CustomerId = "0013838336";
                                paymentModel.PaymentLine.SupplierId = "0";
                                payments.Add(dicKey++, paymentModel);
                            }
                            else
                            {
                                paymentModel.PaymentLine.CustomerId = "0030490303";
                                paymentModel.PaymentLine.SupplierId = "0030490303";
                                payments.Add(dicKey++, paymentModel);
                            }

                        }
                    }

                }
                if (transaction.AccountId == "1010000100")
                {
                    paymentModel.PaymentLine.CustomerId = "0030490303";
                    paymentModel.PaymentLine.SupplierId = "0030490303";
                    payments.Add(dicKey++, paymentModel);

                }

                if (transaction.AccountId == "1403102010")
                {
                    if (payment.HouseBk == "UNW01")
                    {
                        if (payment.A == "D")
                        {
                            var customer = customers[payment.Account];
                            paymentModel.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }
                        else if (payment.A == "K")
                        {
                            var supplier = suppliers[payment.Account];
                            paymentModel.PaymentLine.CustomerId = "0";
                            paymentModel.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);
                            payments.Add(dicKey++, paymentModel);
                        }
                        else
                        {
                            paymentModel.PaymentLine.CustomerId = "0030490303";
                            paymentModel.PaymentLine.SupplierId = "0030490303";
                            payments.Add(dicKey++, paymentModel);
                        }
                    }
                }
               
                if (transaction.AccountId == "1403107010")
                {
                    if (payment.HouseBk == "UNB01")
                    {
                        if (payment.ExTt == "85" || payment.ExTt == "82")
                        {
                            paymentModel.PaymentLine.CustomerId = "00361536";
                            paymentModel.PaymentLine.SupplierId = "0";
                            payments.Add(dicKey++, paymentModel);
                        }

                        if (payment.ExTt == "808" || payment.ExTt == "6" || payment.ExTt == "814" || payment.ExTt == "835" || payment.ExTt == "401")
                        {
                            paymentModel.PaymentLine.CustomerId = "0030490303";
                            paymentModel.PaymentLine.SupplierId = "0030490303";
                            payments.Add(dicKey++, paymentModel);
                        }

                        if (payment.ExTt == "201")
                        {
                            var supplier = suppliers.Values.FirstOrDefault(s => s.AccountId == payment.Account);
                            paymentModel.PaymentLine.CustomerId = "0";
                            paymentModel.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);
                            payments.Add(dicKey++, paymentModel);
                        }
                  
                       if (payment.ExTt == "51" || payment.ExTt == "20")
                       {
                           if (payment.A == "D")
                           {
                               var customer = customers[payment.Account];
                               paymentModel.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                               paymentModel.PaymentLine.SupplierId = "0";
                               payments.Add(dicKey++, paymentModel);
                           }
                           else if (payment.A == "K" && payment.Account != "8400" && payment.Account != "400")
                           {
                               var supplier = suppliers[payment.Account];
                               paymentModel.PaymentLine.CustomerId = "0";
                               paymentModel.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);
                               payments.Add(dicKey++, paymentModel);
                           }
                           else
                           {
                               paymentModel.PaymentLine.CustomerId = "0030490303";
                               paymentModel.PaymentLine.SupplierId = "0030490303";
                               payments.Add(dicKey++, paymentModel);
                           }
                       }
                    }
                }
            }

            var debit = payments.Values.Where(p => p.PaymentLine.DebitCreditIndicator == "D").Sum(p => p.PaymentLine.Amount);
            var credit = (payments.Values.Where(p => p.PaymentLine.DebitCreditIndicator == "C").Sum(p => p.PaymentLine.Amount)) * -1;
            XElement section = new XElement("Payments",
                new XElement("NumberOfEntries", payments.Count()),
                new XElement("TotalDebit", debit.ToString("F2", CultureInfo.InvariantCulture)),
                new XElement("TotalCredit", credit.ToString("F2", CultureInfo.InvariantCulture))
                );

            foreach (var item in payments.Values)
            {
                XElement paymentSection = new XElement("Payment",
                    new XElement("PaymentRefNo", item.PaymentRefNo),
                    new XElement("TransactionDate", item.TransactionDate),
                    new XElement("PaymentMethod", item.PaymentMothod),
                    new XElement("Description", item.Description),
                    new XElement("PaymentLine",
                        new XElement("AccountID", item.PaymentLine.AccountId),
                        new XElement("CustomerID", item.PaymentLine.CustomerId),
                        new XElement("SupplierID", item.PaymentLine.SupplierId),
                        new XElement("DebitCreditIndicator", item.PaymentLine.DebitCreditIndicator),
                        new XElement("PaymentLineAmount",
                            new XElement("Amount", item.PaymentLine.Amount.ToString("F2", CultureInfo.InvariantCulture)),
                            new XElement("CurrencyCode", item.PaymentLine.CurrencyCode),
                            new XElement("CurrencyAmount", item.PaymentLine.CurrencyAmount),
                            new XElement("ExchangeRate", item.PaymentLine.ExchangeRate)
                            ),
                        new XElement("TaxInformation",
                            new XElement("TaxType", item.PaymentLine.TaxType),
                            new XElement("TaxCode", item.PaymentLine.TaxCode),
                            new XElement("TaxAmount",
                                new XElement("Amount", item.PaymentLine.TaxAmount),
                                new XElement("CurrencyCode", item.PaymentLine.TaxCurrencyCode),
                                new XElement("CurrencyAmount", item.PaymentLine.TaxCurrencyAmount),
                                new XElement("ExchangeRate", item.PaymentLine.TaxExchangeRate)
                                )
                            )
                        )
                    );
                section.Add(paymentSection);
            }
            return section;
        }
    }

    public class PaymentModel
    {
        public PaymentModel()
        {
            PaymentLine = new PaymentLine();
        }

        public string? PaymentRefNo { get; set; }
        public string? TransactionDate { get; set; }
        public string? PaymentMothod { get; set; }
        public string? Description { get; set; }
        public PaymentLine? PaymentLine { get; set; }
    }

    public class PaymentLine
    {
        public string? AccountId { get; set; }
        public string? CustomerId { get; set; }
        public string? SupplierId { get; set; }
        public string? DebitCreditIndicator { get; set; }
        public double Amount { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencyAmount { get; set; }
        public string? ExchangeRate { get; set; }
        public string? TaxType { get; set; }
        public string? TaxCode { get; set; }
        public string? TaxAmount { get; set; }
        public string? TaxCurrencyCode { get; set; }
        public string? TaxCurrencyAmount { get; set; }
        public string? TaxExchangeRate { get; set; }
    }
}
