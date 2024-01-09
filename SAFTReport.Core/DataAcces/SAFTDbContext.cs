using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SAFTReport.Core.Models;

namespace SAFTReport.Core.DataAcces
{
    public class SAFTDbContext: DbContext
    {

        public SAFTDbContext(DbContextOptions<SAFTDbContext> options)
        : base(options)
        {
        }

        public DbSet<Constant> Constants { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BalanceSheetEntry> BalanceSheet {  get; set; }
        public DbSet<Customer> Customers {  get; set; }
        public DbSet<Supplier> Suppliers {  get; set; }
        public DbSet<Tax> Taxes {  get; set; }
        public DbSet<EUCountry> EUCountries {  get; set; }
        public DbSet<Analysis> Analyses {  get; set; }
        public DbSet<Transaction> Transactions {  get; set; }
        public DbSet<TaxPayment> TaxPayments {  get; set; }
        public DbSet<Payment> Payments {  get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Constants 
            modelBuilder.Entity<Constant>().ToTable("Constante");
            modelBuilder.Entity<Constant>().Property(c => c.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<Constant>().Property(c => c.ReportingYear).HasColumnName("AnRaportare");
            modelBuilder.Entity<Constant>().Property(c => c.StartDate).HasColumnName("StartDate");
            modelBuilder.Entity<Constant>().Property(c => c.EndDate).HasColumnName("EndDate");
            modelBuilder.Entity<Constant>().Property(c => c.Id).HasColumnName("Id");
            #endregion

            #region Accounts
            modelBuilder.Entity<Account>().ToTable("PlanConturi");
            modelBuilder.Entity<Account>().Property(a => a.Id).HasColumnName("Id");
            modelBuilder.Entity<Account>().Property(a => a.AccountId).HasColumnName("ContPlanConturi");
            modelBuilder.Entity<Account>().Property(a => a.AccountCCC).HasColumnName("ContCCC");
            modelBuilder.Entity<Account>().Property(a => a.AccountSAFT).HasColumnName("ContSAFT");
            modelBuilder.Entity<Account>().Property(a => a.Name).HasColumnName("DescriereCont");
            modelBuilder.Entity<Account>().Property(a => a.Type).HasColumnName("TipCont");
            #endregion

            #region BalanceSheet
            modelBuilder.Entity<BalanceSheetEntry>().ToTable("Balanta");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.Id).HasColumnName("Id");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.AccountCCC).HasColumnName("ContCCC");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.InitialDebit).HasColumnName("SoldInitialDebitor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.InitialCredit).HasColumnName("SoldInitialCreditor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.DebitTurnover).HasColumnName("RulajDebitor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.CreditTurnover).HasColumnName("RulajCreditor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.FinalDebit).HasColumnName("SoldFinalDebitor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.FinalCredit).HasColumnName("SoldFinalCreditor");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<BalanceSheetEntry>().Property(b => b.ReportingYear).HasColumnName("AnRaportare");
            #endregion

            #region Customers
            modelBuilder.Entity<Customer>().ToTable("Clienti");
            modelBuilder.Entity<Customer>().Property(c => c.Id).HasColumnName("Id");
            modelBuilder.Entity<Customer>().Property(c => c.AccountCCC).HasColumnName("ContCCC");
            modelBuilder.Entity<Customer>().Property(c => c.AccountId).HasColumnName("ClientIDCCCC");
            modelBuilder.Entity<Customer>().Property(c => c.FiscalCode).HasColumnName("CodFiscal");
            modelBuilder.Entity<Customer>().Property(c => c.Name).HasColumnName("Nume");
            modelBuilder.Entity<Customer>().Property(c => c.City).HasColumnName("Oras");
            modelBuilder.Entity<Customer>().Property(c => c.Country).HasColumnName("Tara");
            modelBuilder.Entity<Customer>().Property(c => c.InitialDebit).HasColumnName("SoldInitialDebitor");
            modelBuilder.Entity<Customer>().Property(c => c.InitialCredit).HasColumnName("SoldInitialCreditor");
            modelBuilder.Entity<Customer>().Property(c => c.FinalDebit).HasColumnName("SoldFinalDebitor");
            modelBuilder.Entity<Customer>().Property(c => c.FinalCredit).HasColumnName("SoldFinalCreditor");
            modelBuilder.Entity<Customer>().Property(c => c.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<Customer>().Property(c => c.ReportingYear).HasColumnName("AnRaportare");
            #endregion

            #region EUCountries
            modelBuilder.Entity<EUCountry>().ToTable("Tari_UE");
            modelBuilder.Entity<EUCountry>().HasNoKey();
            modelBuilder.Entity<EUCountry>().Property(e => e.Name).HasColumnName("NumeTara");
            modelBuilder.Entity<EUCountry>().Property(e => e.CountryCode).HasColumnName("Acronim");
            #endregion

            #region Suppliers
            modelBuilder.Entity<Supplier>().ToTable("Furnizori");
            modelBuilder.Entity<Supplier>().Property(s => s.Id).HasColumnName("Id");
            modelBuilder.Entity<Supplier>().Property(s => s.AccountCCC).HasColumnName("ContCCC");
            modelBuilder.Entity<Supplier>().Property(s => s.AccountId).HasColumnName("FurnizorIDCCCC");
            modelBuilder.Entity<Supplier>().Property(s => s.FiscalCode).HasColumnName("CodFiscal");
            modelBuilder.Entity<Supplier>().Property(s => s.Name).HasColumnName("Nume");
            modelBuilder.Entity<Supplier>().Property(s => s.City).HasColumnName("Oras");
            modelBuilder.Entity<Supplier>().Property(s => s.Country).HasColumnName("Tara");
            modelBuilder.Entity<Supplier>().Property(s => s.InitialDebit).HasColumnName("SoldInitialDebitor");
            modelBuilder.Entity<Supplier>().Property(s => s.InitialCredit).HasColumnName("SoldInitialCreditor");
            modelBuilder.Entity<Supplier>().Property(s => s.FinalDebit).HasColumnName("SoldFinalDebitor");
            modelBuilder.Entity<Supplier>().Property(s => s.FinalCredit).HasColumnName("SoldFinalCreditor");
            modelBuilder.Entity<Supplier>().Property(s => s.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<Supplier>().Property(s => s.ReportingYear).HasColumnName("AnRaportare");
            #endregion

            #region Taxes
            modelBuilder.Entity<Tax>().ToTable("Taxe");
            modelBuilder.Entity<Tax>().Property(t => t.Id).HasColumnName("Id");
            modelBuilder.Entity<Tax>().Property(t => t.sapId).HasColumnName("cod_SAP");
            modelBuilder.Entity<Tax>().Property(t => t.VATRate).HasColumnName("cota_TVA");
            modelBuilder.Entity<Tax>().Property(t => t.ParentTaxCode).HasColumnName("cod_taxa_parinte");
            modelBuilder.Entity<Tax>().Property(t => t.ChildTaxCode).HasColumnName("cod_taxa_copil");
            modelBuilder.Entity<Tax>().Property(t => t.ParentTaxName).HasColumnName("denumire_taxa_parinte");
            modelBuilder.Entity<Tax>().Property(t => t.ChildTaxName).HasColumnName("denumire_taxa_copil");
            #endregion

            #region Analyses
            modelBuilder.Entity<Analysis>().ToTable("analysis_type");
            modelBuilder.Entity<Analysis>().Property(a => a.Id).HasColumnName("Id");
            modelBuilder.Entity<Analysis>().Property(a => a.Type).HasColumnName("analysis_type");
            modelBuilder.Entity<Analysis>().Property(a => a.TypeDescription).HasColumnName("analysis_type_description");
            modelBuilder.Entity<Analysis>().Property(a => a.AnalysisID).HasColumnName("analysisID");
            modelBuilder.Entity<Analysis>().Property(a => a.Name).HasColumnName("analysisID_description");
            #endregion

            #region Transactions
            modelBuilder.Entity<Transaction>().ToTable("general_ledger_entries");
            modelBuilder.Entity<Transaction>().Property(t => t.Id).HasColumnName("Id");
            modelBuilder.Entity<Transaction>().Property(t => t.DocumentType).HasColumnName("doc_type");
            modelBuilder.Entity<Transaction>().Property(t => t.SupplierId).HasColumnName("vendor_id");
            modelBuilder.Entity<Transaction>().Property(t => t.CustomerId).HasColumnName("customer_id");
            modelBuilder.Entity<Transaction>().Property(t => t.AccountCCC).HasColumnName("account_id");
            modelBuilder.Entity<Transaction>().Property(t => t.AccountType).HasColumnName("account_type");
            modelBuilder.Entity<Transaction>().Property(t => t.AccountId).HasColumnName("gl_account");
            modelBuilder.Entity<Transaction>().Property(t => t.CustomerName).HasColumnName("customer_name");
            modelBuilder.Entity<Transaction>().Property(t => t.SupplierName).HasColumnName("vendor_name");
            modelBuilder.Entity<Transaction>().Property(t => t.Description).HasColumnName("transaction_description");
            modelBuilder.Entity<Transaction>().Property(t => t.TransactioId).HasColumnName("tranzaction_id");
            modelBuilder.Entity<Transaction>().Property(t => t.InvoiceNo).HasColumnName("invoice_no");
            modelBuilder.Entity<Transaction>().Property(t => t.PostingDate).HasColumnName("posting_date");
            modelBuilder.Entity<Transaction>().Property(t => t.DocumentDate).HasColumnName("document_date");
            modelBuilder.Entity<Transaction>().Property(t => t.Value).HasColumnName("cc_value");
            modelBuilder.Entity<Transaction>().Property(t => t.Ccy).HasColumnName("ccy");
            modelBuilder.Entity<Transaction>().Property(t => t.ProfitCenter).HasColumnName("profit_center");
            modelBuilder.Entity<Transaction>().Property(t => t.TaxCode).HasColumnName("tax_code");
            modelBuilder.Entity<Transaction>().Property(t => t.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<Transaction>().Property(t => t.ReportingYear).HasColumnName("AnRaportare");
            modelBuilder.Entity<Transaction>().Property(t => t.FileNo).HasColumnName("fisier");
            #endregion

            #region TaxPayments
            modelBuilder.Entity<TaxPayment>().ToTable("tax_line");
            modelBuilder.Entity<TaxPayment>().Property(t => t.Id).HasColumnName("id");
            modelBuilder.Entity<TaxPayment>().Property(t => t.AccountId).HasColumnName("gl_account");
            modelBuilder.Entity<TaxPayment>().Property(t => t.Description).HasColumnName("transaction_description");
            modelBuilder.Entity<TaxPayment>().Property(t => t.TransactionId).HasColumnName("tranzaction_id");
            modelBuilder.Entity<TaxPayment>().Property(t => t.InvoiceNo).HasColumnName("invoice_no");
            modelBuilder.Entity<TaxPayment>().Property(t => t.PostingDate).HasColumnName("posting_date");
            modelBuilder.Entity<TaxPayment>().Property(t => t.DocumentDate).HasColumnName("document_date");
            modelBuilder.Entity<TaxPayment>().Property(t => t.TaxBaseAmount).HasColumnName("tax_base_amount");
            modelBuilder.Entity<TaxPayment>().Property(t => t.VatAmout).HasColumnName("vat_amount");
            modelBuilder.Entity<TaxPayment>().Property(t => t.VatRate).HasColumnName("rate");
            modelBuilder.Entity<TaxPayment>().Property(t => t.TaxCode).HasColumnName("tax_code");
            modelBuilder.Entity<TaxPayment>().Property(t => t.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<TaxPayment>().Property(t => t.ReportingYear).HasColumnName("AnRaportare");
            modelBuilder.Entity<TaxPayment>().Property(t => t.FileNo).HasColumnName("fisier");
            #endregion

            #region Payments
            modelBuilder.Entity<Payment>().ToTable("Payments");
            modelBuilder.Entity<Payment>().Property(p => p.Id).HasColumnName("Id");
            modelBuilder.Entity<Payment>().Property(p => p.CoCd).HasColumnName("co_cd");
            modelBuilder.Entity<Payment>().Property(p => p.HouseBk).HasColumnName("house_bk");
            modelBuilder.Entity<Payment>().Property(p => p.AcctId).HasColumnName("acct_id");
            modelBuilder.Entity<Payment>().Property(p => p.StmtId).HasColumnName("stmt_id");
            modelBuilder.Entity<Payment>().Property(p => p.StmtDate).HasColumnName("stmt_date");
            modelBuilder.Entity<Payment>().Property(p => p.ShrtKey).HasColumnName("shrt_key");
            modelBuilder.Entity<Payment>().Property(p => p.MrNo).HasColumnName("mr_no");
            modelBuilder.Entity<Payment>().Property(p => p.Type).HasColumnName("type");
            modelBuilder.Entity<Payment>().Property(p => p.ImpDate).HasColumnName("imp_date");
            modelBuilder.Entity<Payment>().Property(p => p.ImpTime).HasColumnName("imp_time");
            modelBuilder.Entity<Payment>().Property(p => p.EbUser).HasColumnName("eb_user");
            modelBuilder.Entity<Payment>().Property(p => p.DC).HasColumnName("d_c");
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasColumnName("amount");
            modelBuilder.Entity<Payment>().Property(p => p.Ac).HasColumnName("ac");
            modelBuilder.Entity<Payment>().Property(p => p.TransTyp).HasColumnName("trans_typ");
            modelBuilder.Entity<Payment>().Property(p => p.TxTk).HasColumnName("tx_tk");
            modelBuilder.Entity<Payment>().Property(p => p.ExTt).HasColumnName("ex_tt");
            modelBuilder.Entity<Payment>().Property(p => p.PostRule).HasColumnName("post_rule");
            modelBuilder.Entity<Payment>().Property(p => p.Text).HasColumnName("text");
            modelBuilder.Entity<Payment>().Property(p => p.ProfitCtr).HasColumnName("profit_ctr");
            modelBuilder.Entity<Payment>().Property(p => p.TransactionId).HasColumnName("transaction_id");
            modelBuilder.Entity<Payment>().Property(p => p.SubDocNo).HasColumnName("sub_doc_no");
            modelBuilder.Entity<Payment>().Property(p => p.A).HasColumnName("a");
            modelBuilder.Entity<Payment>().Property(p => p.Account).HasColumnName("account");
            modelBuilder.Entity<Payment>().Property(p => p.BankNumber).HasColumnName("bank_number");
            modelBuilder.Entity<Payment>().Property(p => p.ReportingMonth).HasColumnName("LunaRaportare");
            modelBuilder.Entity<Payment>().Property(p => p.ReportingYear).HasColumnName("AnRaportare");
            #endregion
        }


    }
}
