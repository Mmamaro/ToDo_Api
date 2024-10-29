using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace ToDo_Api.DapperContext
{
    public class DapperContext
    {
        private readonly IConfiguration _config;
        private readonly ILogger<DapperContext> _logger;
        private string _connectionString;

        public DapperContext(IConfiguration config, ILogger<DapperContext> logger)
        {
            _config = config;
            _logger = logger;
            _connectionString = _config.GetConnectionString("DefaultConnection");
        }

        public async Task<List<T>> QueryMultipleRows<T>(string query)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_connectionString);

                var data =  await connection.QueryAsync<T>(query);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the DAPPER CONTEXT while trying to query multiple rows: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<List<T>> QueryMultipleRowsWithParams<T>(string query, DynamicParameters parameters)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_connectionString);

                var data = await connection.QueryAsync<T>(query, parameters);

                return data.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the DAPPER CONTEXT while trying to query multiple rows with parameters: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<T> QuerySingleRow<T>(string query, DynamicParameters parameters)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_connectionString);

                var data = await connection.QueryFirstOrDefaultAsync<T>(query, parameters);

                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the DAPPER CONTEXT while trying to query a single row: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> ExecuteQuery(string query, DynamicParameters parameters)
        {
            try
            {
                using IDbConnection connection = new SqlConnection(_connectionString);

                var rowsAffected = await connection.ExecuteAsync(query,parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the DAPPER CONTEXT while trying to execute a query: {ex}", ex.Message);
                return default;
            }
        }

    }
}
