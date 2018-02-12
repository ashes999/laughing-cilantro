using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Data.SqlClient;
using System.Configuration;
using LaughingCilantro.ObjectModel.Interfaces;

namespace LaughingCilantro.ObjectModel.Repository
{
    public class UserRepository : IUserRepository
    {
        private ConnectionStringSettings connectionString;
        public UserRepository(ConnectionStringSettings connectionString)
        {
            this.connectionString = connectionString;
        }

        public string GetUserId(string userName)
        {
            var userId = "";

            using (var connection = new SqlConnection(connectionString.ConnectionString))
            {
                connection.Open();
                userId = connection.ExecuteScalar<string>("SELECT Id FROM AspNetUsers WHERE email = @email", new { email = userName });
            }

            return userId;
        }
    }
}
