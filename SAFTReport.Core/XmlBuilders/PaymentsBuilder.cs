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
            var transactionsDictionary = dbContext.Transactions.Where(t =>
                t.ReportingMonth == month &&
                t.ReportingYear == year &&
                t.AccountId == "1401107010" || 
                t.AccountId == "1433107010" || 
                t.AccountId == "1010000100" || 
                t.AccountId == "1403102010" || 
                t.AccountId == "1403107010").ToDictionary(t => t.Id, t => t);
           
            var paymentsDictionary = dbContext.Payments.Where(p => p.ReportingMonth == month && p.ReportingYear == year).ToDictionary(p => p.Id, p => p);

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

            foreach(var transaction in transactionsDictionary.Values)
            {
                var payment = new PaymentModel();

                var amount = double.TryParse(transaction.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out double result) ? result : 0;
                payment.PaymentRefNo = transaction.TransactioId;
                payment.TransactionDate = DateTime.ParseExact(transaction.PostingDate, "dd.MM.yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd");
                payment.PaymentMothod = "03";
                payment.Description = transaction.Description;
                payment.PaymentLine.AccountId = accounts.FirstOrDefault(a => a.AccountCCC == transaction.AccountCCC).AccountSAFT;
                payment.PaymentLine.DebitCreditIndicator = amount > 0 ? "D" : "C";
                payment.PaymentLine.Amount = amount;
                payment.PaymentLine.CurrencyCode = transaction.Ccy;
                payment.PaymentLine.CurrencyAmount = amount.ToString("F2", CultureInfo.InvariantCulture);
                payment.PaymentLine.ExchangeRate = "1";
                payment.PaymentLine.TaxType = "000";
                payment.PaymentLine.TaxCode = "000000";
                payment.PaymentLine.TaxAmount = "0.00";
                payment.PaymentLine.TaxCurrencyCode = transaction.Ccy;
                payment.PaymentLine.TaxCurrencyAmount = "0.00";
                payment.PaymentLine.TaxExchangeRate = "1";

                if (transaction.AccountId == "1401107010")
                {
                    var paymentBs = paymentsDictionary.Values.FirstOrDefault(p => p.TransactionId == transaction.TransactioId);

                    if (paymentBs != null)
                    {

                        if (paymentBs.ExTt != "66" && paymentBs.ExTt != "72")
                        {
                            payment.PaymentLine.CustomerId = "0030490303";
                            payment.PaymentLine.SupplierId = "0030490303";

                            payments.Add(dicKey++, payment);

                        }

                        if (paymentBs.ExTt == "66")
                        {
                            if (paymentBs.BankNumber == "/OB//RO49BTRL03")
                            {
                              
                                payment.PaymentLine.CustomerId = "0021611392";
                                payment.PaymentLine.SupplierId = "0";

                                payments.Add(dicKey++, payment);
                            }

                            if (paymentBs.BankNumber == "/OB//RO80BACX00")
                            {

                                payment.PaymentLine.CustomerId = "01PL7792308495";
                                payment.PaymentLine.SupplierId = "0";

                                payments.Add(dicKey++, payment);
                            }

                            if (paymentBs.BankNumber == "/OB/RZBRROBU RO")
                            {

                                payment.PaymentLine.CustomerId = "0013838336";
                                payment.PaymentLine.SupplierId = "0";

                                payments.Add(dicKey++, payment);
                            }

                            if (paymentBs.BankNumber == "/OB/BTRLRO22 RO")
                            {

                                payment.PaymentLine.CustomerId = "0013838336";
                                payment.PaymentLine.SupplierId = "0";

                                payments.Add(dicKey++, payment);
                            }
                        }

                        if(paymentBs.ExTt == "72")
                        {
                            var supplier = suppliers[paymentBs.Account];

                            payment.PaymentLine.CustomerId = "0";
                            payment.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);

                            payments.Add(dicKey++, payment);
                        }


                    }
                }

                if (transaction.AccountId == "1433107010")
                {
                    var paymentBs = paymentsDictionary.Values.FirstOrDefault(p => p.TransactionId == transaction.TransactioId);
                    
                    if (paymentBs != null && paymentBs.HouseBk == "UNE01")
                    {
                        if (paymentBs.A == "D")
                        {
                            var customer = customers[paymentBs.Account];
                            payment.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                            payment.PaymentLine.SupplierId = "0";

                            payments.Add(dicKey++, payment);

                        } else
                        {
                            if(paymentBs.PostRule == "+N7")
                            {
                                //trebuie furnizata logica
                                //payment.PaymentLine.CustomerId = "0013838336";
                                //payment.PaymentLine.SupplierId = "0";

                                //payments.Add(dicKey++, payment);

                            } else
                            {
                                payment.PaymentLine.CustomerId = "0030490303";
                                payment.PaymentLine.SupplierId = "0030490303";

                                payments.Add(dicKey++, payment);
                            }

                        }
                    }

                }

                if (transaction.AccountId == "1010000100")
                {
                    payment.PaymentLine.CustomerId = "0030490303";
                    payment.PaymentLine.SupplierId = "0030490303";

                    payments.Add(dicKey++, payment);

                }
                

                if (transaction.AccountId == "1403102010")
                {
                    var paymentBs = paymentsDictionary.Values.FirstOrDefault(p => p.TransactionId == transaction.TransactioId);

                    if (paymentBs != null && paymentBs.HouseBk == "UNW01")
                    {
                        if (paymentBs.A == "D")
                        {
                            var customer = customers[paymentBs.Account];
                            payment.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                            payment.PaymentLine.SupplierId = "0";

                            payments.Add(dicKey++, payment);
                        } else if (paymentBs.A == "K")
                        {
                            var supplier = suppliers[paymentBs.Account];

                            payment.PaymentLine.CustomerId = "0";
                            payment.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);

                            payments.Add(dicKey++, payment);
                        } else
                        {
                            payment.PaymentLine.CustomerId = "0030490303";
                            payment.PaymentLine.SupplierId = "0030490303";

                            payments.Add(dicKey++, payment);
                        }
                    }
                }
                
                 if (transaction.AccountId == "1403107010")
                {
                    var paymentBs = paymentsDictionary.Values.FirstOrDefault(p => p.TransactionId == transaction.TransactioId);

                    if (paymentBs != null && paymentBs.HouseBk == "UNB01")
                    {
                        if(paymentBs.ExTt == "85" || paymentBs.ExTt == "82")
                        {
                            payment.PaymentLine.CustomerId = "00361536";
                            payment.PaymentLine.SupplierId = "0";

                            payments.Add(dicKey++, payment);
                        }

                        if (paymentBs.ExTt == "808" || paymentBs.ExTt == "6" || paymentBs.ExTt == "814" || paymentBs.ExTt == "835" || paymentBs.ExTt == "401")
                        {
                            payment.PaymentLine.CustomerId = "0030490303";
                            payment.PaymentLine.SupplierId = "0030490303";

                            payments.Add(dicKey++, payment);
                        }

                        if (paymentBs.ExTt == "201")
                        {
                            var supplier = suppliers.Values.FirstOrDefault(s => s.AccountId == paymentBs.Account);

                            payment.PaymentLine.CustomerId = "0";
                            payment.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);

                            payments.Add(dicKey++, payment);
                        }

                        if (paymentBs.ExTt == "51" ||  paymentBs.ExTt == "20")
                        {
                            if (paymentBs.A == "D")
                            {
                                var customer = customers[paymentBs.Account];
                                payment.PaymentLine.CustomerId = utility.MapFiscalCode(customer.Name, customer.FiscalCode, customer.Country, euCountrie);
                                payment.PaymentLine.SupplierId = "0";
                            }
                            else if (paymentBs.A == "K" && paymentBs.Account != "8400" && paymentBs.Account != "400")
                            {
                                var supplier = suppliers[paymentBs.Account];

                                payment.PaymentLine.CustomerId = "0";
                                payment.PaymentLine.SupplierId = utility.MapFiscalCode(supplier.Name, supplier.FiscalCode, supplier.Country, euCountrie);

                                payments.Add(dicKey++, payment);

                            }
                            else
                            {
                                payment.PaymentLine.CustomerId = "0030490303";
                                payment.PaymentLine.SupplierId = "0030490303";

                                payments.Add(dicKey++, payment);

                            }


                        }
                    }
                }
                
                Console.WriteLine(dicKey);
                

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
