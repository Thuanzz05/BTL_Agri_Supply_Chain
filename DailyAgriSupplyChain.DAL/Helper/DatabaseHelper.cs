using Microsoft.Data.SqlClient; 
using Microsoft.Extensions.Configuration; 
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using DailyAgriSupplyChain.DAL.Helper.Interfaces; 

namespace DailyAgriSupplyChain.DAL.Helper
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        // DI: Nhận IConfiguration để lấy chuỗi kết nối
        public DatabaseHelper(IConfiguration configuration)
        {
            // Lấy chuỗi kết nối từ appsettings.json
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("DefaultConnection 'DefaultConnection' not found in configuration.");
        }

        // Phương thức hỗ trợ tạo SqlCommand
        private SqlCommand CreateCommand(SqlConnection connection, string sql, CommandType commandType, DbParameter[] parameters)
        {
            var command = new SqlCommand(sql, connection)
            {
                CommandType = commandType
            };

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }

        /// <summary>
        /// Tạo một tham số DB an toàn (dùng SqlParameter).
        /// </summary>
        public DbParameter CreateParameter(string name, object value, DbType dbType)
        {
            // Dùng SqlParameter của SQL Server, xử lý DBNull
            return new SqlParameter(name, value ?? DBNull.Value)
            {
                DbType = dbType
            };
        }

        // 1. Execute NonQuery ASYNC
        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            // Dùng 'using' đảm bảo kết nối được đóng và hủy cho MỖI yêu cầu
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        // 2. Execute Scalar ASYNC
        public async Task<object> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    return await command.ExecuteScalarAsync();
                }
            }
        }

        // 3. Execute Query To DataTable ASYNC
        public async Task<DataTable> ExecuteQueryToDataTableAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var dataTable = new DataTable();
                        // Tải toàn bộ kết quả vào DataTable
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }
    }
}