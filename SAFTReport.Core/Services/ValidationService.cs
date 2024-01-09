using SAFTReport.Core.DataAcces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAFTReport.Core.Services
{
    public class ValidationService
    {
        private readonly SAFTDbContext dbContext;

        public ValidationService(SAFTDbContext context)
        {
            dbContext = context;
        }

        public bool ValidateBalanceSheetAccounts()
        {
            var isValid = dbContext.BalanceSheet.Any(b => 
                !dbContext.Accounts.Any(a => a.AccountCCC == b.AccountCCC) &&
                (b.AccountCCC != null && !b.AccountCCC.StartsWith("8")) &&
                !b.AccountCCC.StartsWith("9") &&
                !string.IsNullOrEmpty(b.AccountCCC));

            if (isValid)
            {
                throw new Exception("!!! There are accounts in the balance sheet that are not defined in the accounts. !!!");
            }

            return isValid;
        }


    }
}
