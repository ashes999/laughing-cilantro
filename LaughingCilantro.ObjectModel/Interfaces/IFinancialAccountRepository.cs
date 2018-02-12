using LaughingCilantro.ObjectModel.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaughingCilantro.ObjectModel.Interfaces
{
    public interface IFinancialAccountRepository
    {
        void CreateAccount(FinancialAccount account);
        IEnumerable<FinancialAccount> GetAllAccounts(string userId);
        IEnumerable<Transaction> GetAccountTransactions(Guid accountId);
        void SaveTransactions(Guid accountId, List<Transaction> transactions);
    }
}
