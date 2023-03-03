using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetApi.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;

        private IDbConnection GetConnection() => new SqlConnection(_config.GetConnectionString("Default"));

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string query)
        {
            IDbConnection DbConnection = GetConnection();
            IEnumerable<T> list = DbConnection.Query<T>(query);

            return list;
        }

        public T LoadSingleData<T>(string query)
        {
            Console.WriteLine("query: " + query);
            IDbConnection DbConnection = GetConnection();
            T user = DbConnection.QuerySingle<T>(query);

            return user;
        }

        public bool ExecuteSql(string query)
        {
            IDbConnection DbConnection = GetConnection();

            return DbConnection.Execute(query) > 0;
        }

        public int ExecuteSqlWithRowCount(string query)
        {
            IDbConnection DbConnection = GetConnection();

            return DbConnection.Execute(query);
        }

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            SqlCommand commandWithParams = new SqlCommand(sql);

            foreach(SqlParameter parameter in parameters)
            {
                commandWithParams.Parameters.Add(parameter);
            }

            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("Default"));
            dbConnection.Open();

            commandWithParams.Connection = dbConnection;

            int rowsAffected = commandWithParams.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }

    }
}