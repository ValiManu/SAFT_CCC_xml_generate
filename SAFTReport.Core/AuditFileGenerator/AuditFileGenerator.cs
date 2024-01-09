using SAFTReport.Core.XmlBuilders.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SAFTReport.Core.AuditFileGenerator
{
    public class AuditFileGenerator
    {
        private readonly IHeaderBuilder header;
        private readonly IGeneralLedgerBuilder generalLedger;
        private readonly ICustomersBuilder customers;
        private readonly ISupplierBuilders suppliers;
        private readonly ITaxesBuilder taxes;
        private readonly IUOMBuilder uOM;
        private readonly IAnalysisBuilder analysis;
        private readonly IMovementsBuilder movements;
        private readonly IProductsBuilder products;
        private readonly IOwnersBuilder owners;
        private readonly IAssetsBuilder assets;
        private readonly IGeneralLedgerEntriesBuilder generalLedgerEntries;
        private readonly ISalesInvoicesBuilder salesInvoices;
        private readonly IPurchaseInvoicesBuilder purchaseInvoices;
        private readonly IAccountsBuilder accounts;
        private readonly IPaymentsBuilder payments;


        public AuditFileGenerator(
            IHeaderBuilder headerBuilder, 
            IGeneralLedgerBuilder generalLedgerBuilder, 
            ICustomersBuilder customersBuilder, 
            ISupplierBuilders supplierBuilder,
            ITaxesBuilder taxesBuilder,
            IUOMBuilder uOMBuilder,
            IAnalysisBuilder analysisBuilder,
            IMovementsBuilder movementsBuilder,
            IProductsBuilder productsBuilder,
            IOwnersBuilder ownersBuilder,
            IAssetsBuilder assetsBuilder,
            IGeneralLedgerEntriesBuilder generalLedgerEntriesBuilder,
            ISalesInvoicesBuilder salesInvoicesBuilder,
            IPurchaseInvoicesBuilder purchaseInvoicesBuilder,
            IAccountsBuilder accountsBuilder,
            IPaymentsBuilder paymentsBuilder)
        {
            header = headerBuilder;
            generalLedger = generalLedgerBuilder;
            customers = customersBuilder;
            suppliers = supplierBuilder;
            taxes = taxesBuilder;
            uOM = uOMBuilder;
            analysis = analysisBuilder;
            movements = movementsBuilder;
            products = productsBuilder;
            owners = ownersBuilder;
            assets = assetsBuilder;
            generalLedgerEntries = generalLedgerEntriesBuilder;
            salesInvoices = salesInvoicesBuilder;
            purchaseInvoices = purchaseInvoicesBuilder;
            accounts = accountsBuilder;
            payments = paymentsBuilder;

        }

        public XDocument GenerateAuditFile(int month, int year)
        {
            XNamespace xmlns = "mfp:anaf:dgti:d406:declaratie:v1";
            XNamespace xmlnsEmpty = "";
            var auditFile = new XDocument(
                new XElement("AuditFile",
                    header.BuildSection(month, year), 
                    new XElement("MasterFiles",
                        generalLedger.BuildSection(month, year),
                        customers.BuildSection(month, year),
                        suppliers.BuildSection(month, year),
                        taxes.BuildSection(month, year),
                        uOM.BuildSection(month, year),
                        analysis.BuildSection(month, year),
                        movements.BuildSection(month, year),
                        products.BuildSection(month, year),
                        owners.BuildSection(month, year),
                        assets.BuildSection(month, year)
                        ),
                   new XElement("GeneralLedgerEntries",
                        generalLedgerEntries.BuildSection(month, year).Elements()
                        ),
                    new XElement("SourceDocuments",
                        salesInvoices.BuildSection(month, year),
                        purchaseInvoices.BuildSection(month, year),
                        payments.BuildSection(month, year),
                        new XElement("MovementOfGoods")
                        )
                )
            );

            return auditFile;
        }


        public void SaveAuditFile(string filePath, int month, int year)
        {
            var auditFile = GenerateAuditFile(month, year);
            auditFile.Save(filePath);
        }
    }
}
