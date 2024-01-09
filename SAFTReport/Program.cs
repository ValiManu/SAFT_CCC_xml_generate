using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SAFTReport.Core.XmlBuilders;
using SAFTReport.Core.AuditFileGenerator;
using SAFTReport.Core.DataAcces;
using SAFTReport.Core.Services;
using SAFTReport.Core.Utility;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using SAFTReport.Core.XmlBuilders.Interfaces;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
        {
            services.AddScoped<IHeaderBuilder, HeadeBuilder>();
            services.AddScoped<IGeneralLedgerBuilder, GeneralLedgerBuilder>();
            services.AddScoped<ICustomersBuilder, CustomersBuilder>();
            services.AddScoped<ISupplierBuilders, SuppliersBuilder>();
            services.AddScoped<ITaxesBuilder, TaxesBuilder>();
            services.AddScoped<IUOMBuilder, UOMBuilder>();
            services.AddScoped<IAnalysisBuilder, AnalysisBuilder>();
            services.AddScoped<IMovementsBuilder, MovementsBuilder>();
            services.AddScoped<IProductsBuilder, ProductsBuilder>();
            services.AddScoped<IOwnersBuilder, OwnersBuilder>();
            services.AddScoped<IAssetsBuilder, AssetsBuilder>();
            services.AddScoped<ISalesInvoicesBuilder, SalesInvoicesBuilder>();
            services.AddScoped<IGeneralLedgerEntriesBuilder, GeneralLedgerEntriesBuilder>();
            services.AddScoped<IPurchaseInvoicesBuilder, PurchaseInvoicesBuilder>();
            services.AddScoped<IAccountsBuilder, AccountsBuilders>();
            services.AddScoped<IPaymentsBuilder, PaymentsBuilder>();
            services.AddScoped<AuditFileGenerator>();
            services.AddScoped<ValidationService>();
            services.AddScoped<DateUtility>();
            services.AddDbContext<SAFTDbContext>(options =>
                options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));
        });

var host = builder.Build();
using var serviceScope = host.Services.CreateScope();
var services = serviceScope.ServiceProvider;
Console.Write("Luna Raportare: ");
var month = int.Parse(Console.ReadLine());
Console.Write("An Raportare: ");
var year = int.Parse(Console.ReadLine());
Console.WriteLine("Procesarea a inceput.....");

try
{
    var path = $"C:\\Users\\Vali\\Outmost S.R.L\\OutMost - Documents\\SAFT CCC transfer files\\XML_Reports\\Shoe_{year}_{month}.xml";
    var validationService = services.GetRequiredService<ValidationService>();
    validationService.ValidateBalanceSheetAccounts();

    var auditFileGenerator = services.GetRequiredService<AuditFileGenerator>();
    auditFileGenerator.SaveAuditFile(path, month, year);
    
}
catch (Exception ex)
{
    
    Console.WriteLine($"Error: {ex}");
}

Console.WriteLine($"Fisierul a fost generat in C:\\Users\\Vali\\Desktop\\Repo\\SAFT_CCC_xml_generate\\auditFile_{month}_{year}.xml ");