using System;
using System.Data.SqlClient;

namespace CyberBot
{
    public class Database
    {
        private readonly string connectionString =
            @"Data Source=(LocalDB)\MSSQLLocalDB;
              Initial Catalog=CyberBotDB;
              Integrated Security=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
