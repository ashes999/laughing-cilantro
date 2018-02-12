using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using LaughingCilantro.ObjectModel.Interfaces;
using LaughingCilantro.ObjectModel.Objects;

namespace LaughingCilantro.ObjectModel.Repository
{
    public class FinancialAccountRepository : IFinancialAccountRepository
    {
        private ConnectionStringSettings connectionString;

        public FinancialAccountRepository(ConnectionStringSettings connectionString)
        {
            this.connectionString = connectionString;
        }

        public void CreateAccount(FinancialAccount account)
        {
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                connection.Execute("INSERT INTO dbo.FinancialAccounts (Name, OwnerId, AccountType) VALUES (@name, @ownerId, @accountType)", new
                {
                    name = account.Name,
                    ownerId = account.OwnerId,
                    AccountType = account.AccountType.ToString()
                });
            }
        }

        public IEnumerable<FinancialAccount> GetAllAccounts(string userId)
        {
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                return connection.Query<FinancialAccount>("SELECT * FROM dbo.FinancialAccounts WHERE OwnerId = @ownerId", new { ownerId = userId });
            }
        }
        public IEnumerable<Transaction> GetAccountTransactions(Guid accountId)
        {
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                return connection.Query<Transaction>("SELECT * FROM dbo.Transactions WHERE AccountId = @accountId", new { accountId = accountId });
            }
        }

        public void SaveTransactions(Guid accountId, List<Transaction> transactions)
        {
            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                var dtos = transactions.Select(t => new { fitId = t.ForeignId, text = t.OriginalText, accountId = accountId, amount = t.Amount, transactionDate = t.TransactionDateUtc }).ToArray();
                // UPSERT: update if it exists
                foreach (var dto in dtos)
                {
                    var transactionExists = connection.ExecuteScalar<int>("SELECT COUNT(*) FROM Transactions WHERE ForeignId = @foreignId AND AccountId = @accountId",
                        new { foreignId = dto.fitId, accountId = dto.accountId });

                    if (transactionExists == 0)
                    {
                        connection.Execute("INSERT INTO Transactions (ForeignId, OriginalText, AccountId, Amount, TransactionDateUtc) VALUES (@fitId, @text, @accountId, @amount, @transactionDate)", dto);
                    }
                    else
                    {
                        connection.Execute("UPDATE Transactions SET OriginalText=@text, amount=@amount, TransactionDateUtc = @transactionDate WHERE ForeignId = @fitId AND accountId = @accountId",
                            new { text = dto.text, amount = dto.amount, transactionDate = dto.transactionDate, fitId = dto.fitId, accountId = dto.accountId });
                    }
                }
            }
        }
    }
}
