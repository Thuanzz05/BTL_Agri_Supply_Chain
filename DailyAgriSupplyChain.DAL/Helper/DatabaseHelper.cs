using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using DailyAgriSupplyChain.DAL.Helper.Interfaces;
using System.Linq; // Cần cho .ToList(), v.v.

namespace DailyAgriSupplyChain.DAL.Helper
{
    public class DatabaseHelper : IDatabaseHelper
    {
        private readonly string _connectionString;

        // --- BIẾN INSTANCE (KHUYẾN NGHỊ DÙNG NULABLE CHO CODE HIỆN ĐẠI) ---
        // Giữ lại để khớp với các hàm quản lý kết nối thủ công của bạn
        public string StrConnection { get; set; }
        public SqlConnection? sqlConnection { get; set; } // Sửa thành nullable
        public SqlTransaction? sqlTransaction { get; set; } // Sửa thành nullable

        public DatabaseHelper(IConfiguration configuration)
        {
            StrConnection = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("DefaultConnection not found.");
            _connectionString = StrConnection;
        }

        // --- HÀM HỖ TRỢ TẠO COMMAND VÀ PARAMETER ---
        private SqlCommand CreateCommand(SqlConnection connection, string sql, CommandType commandType, DbParameter[]? parameters, SqlTransaction? transaction = null)
        {
            var command = new SqlCommand(sql, connection)
            {
                CommandType = commandType,
                Transaction = transaction
            };
            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }
            return command;
        }

        public DbParameter CreateParameter(string name, object? value, DbType dbType)
        {
            return new SqlParameter(name, value ?? DBNull.Value) { DbType = dbType };
        }

        // Hàm hỗ trợ tạo tham số từ params object[]
        private void AddParametersFromObjects(SqlCommand cmd, params object[] paramObjects)
        {
            if (paramObjects == null || paramObjects.Length == 0) return;
            int parameterInput = paramObjects.Length / 2;
            int j = 0;
            for (int i = 0; i < parameterInput; i++)
            {
                string paramName = Convert.ToString(paramObjects[j++]) ?? throw new ArgumentException("Param name cannot be null.");
                object? value = paramObjects[j++];

                SqlParameter sqlParam = new SqlParameter(paramName, value ?? DBNull.Value);
                if (paramName.ToLower().Contains("json"))
                {
                    sqlParam.SqlDbType = SqlDbType.NVarChar;
                    sqlParam.Size = -1;
                }
                cmd.Parameters.Add(sqlParam);
            }
        }

        // =================================================================================
        //                 PHẦN ASYNC (GIỮ NGUYÊN)
        // =================================================================================

        public async Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
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
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }

        // =================================================================================
        //                 HÀM QUẢN LÝ KẾT NỐI/CHUNK CỦA BẠN (TRIỂN KHAI)
        // =================================================================================

        // [CÁC HÀM NÀY SỬ DỤNG BIẾN INSTANCE VÀ KHÔNG AN TOÀN TRONG MÔI TRƯỜNG WEB]
        // Triển khai logic Open/Close thủ công của bạn
        public string OpenConnection() { /* ... */ return ""; }
        public string OpenConnectionAndBeginTransaction() { /* ... */ return ""; }
        public string CloseConnection() { /* ... */ return ""; }
        public string CloseConnectionAndEndTransaction(bool isRollbackTransaction) { /* ... */ return ""; }
        public void SetConnectionString(string connectionString) { StrConnection = connectionString; }


        // --- HÀM THỰC THI CHUỖI ĐỒNG BỘ (CƠ BẢN) ---

        public int ExecuteNonQuery(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    return command.ExecuteNonQuery();
                }
            }
        }

        public object? ExecuteScalar(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    return command.ExecuteScalar();
                }
            }
        }

        public DataTable ExecuteQueryToDataTable(string sql, out string msgError, CommandType commandType = CommandType.Text, DbParameter[] parameters = null)
        {
            msgError = "";
            var result = new DataTable();
            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = CreateCommand(connection, sql, commandType, parameters))
                {
                    try
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            result.Load(reader);
                        }
                    }
                    catch (Exception ex)
                    {
                        msgError = ex.ToString();
                    }
                }
            }
            return result;
        }

        // --- HÀM STORED PROCEDURE PHỨC TẠP (PARAMS OBJECT[]) ---

        public string ExecuteSProcedure(string sprocedureName, params object[] paramObjects)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(StrConnection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = sprocedureName, Connection = connection };
                    connection.Open();
                    AddParametersFromObjects(cmd, paramObjects); // Sử dụng hàm hỗ trợ
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exception) { result = exception.ToString(); }
            }
            return result;
        }

        public DataTable ExecuteSProcedureReturnDataTable(out string msgError, string sprocedureName, params object[] paramObjects)
        {
            DataTable tb = new DataTable();
            msgError = "";
            using (SqlConnection connection = new SqlConnection(StrConnection))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand { CommandType = CommandType.StoredProcedure, CommandText = sprocedureName, Connection = connection };
                    AddParametersFromObjects(cmd, paramObjects); // Sử dụng hàm hỗ trợ

                    using (SqlDataAdapter ad = new SqlDataAdapter(cmd))
                    {
                        ad.Fill(tb);
                    }
                }
                catch (Exception exception) { tb = null; msgError = exception.ToString(); }
            }
            return tb;
        }

        public DataSet? ExecuteSProcedureReturnDataset(out string msgError, string sprocedureName, params object[] paramObjects)
        {
            // Tương tự ExecuteSProcedureReturnDataTable nhưng dùng DataSet
            throw new NotImplementedException();
        }

        public string ExecuteSProcedure(SqlConnection sqlConnection, string sprocedureName, params object[] paramObjects)
        {
            // Hàm này cần được triển khai nếu được gọi, sử dụng kết nối đã được truyền vào
            throw new NotImplementedException();
        }

        public string ExecuteSProcedureWithTransaction(string sprocedureName, params object[] paramObjects)
        {
            string result = "";
            using (SqlConnection connection = new SqlConnection(StrConnection))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand cmd = connection.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = sprocedureName;
                        cmd.Transaction = transaction;

                        AddParametersFromObjects(cmd, paramObjects);

                        cmd.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception exception)
                    {
                        result = exception.ToString();
                        try { transaction.Rollback(); } catch { /* Ignore */ }
                    }
                }
            }
            return result;
        }

        public object? ExecuteScalarSProcedure(out string msgError, string sprocedureName, params object[] paramObjects)
        {
            // Triển khai tương tự ExecuteScalarSProcedureWithTransaction, nhưng không có transaction
            throw new NotImplementedException();
        }

        public object? ExecuteScalarSProcedureWithTransaction(out string msgError, string sprocedureName, params object[] paramObjects)
        {
            // Logic tương tự ExecuteSProcedureWithTransaction nhưng trả về giá trị scalar
            throw new NotImplementedException();
        }

        // ... CÁC HÀM PHỨC TẠP KHÁC CẦN TRIỂN KHAI ...

        // ... CÁC HÀM List<StoreParameterInfo> CŨNG CẦN TRIỂN KHAI TƯƠNG TỰ ...

        public List<string> ExecuteScalarSProcedure(List<StoreParameterInfo> storeParameterInfos) { throw new NotImplementedException(); }
        public List<string> ExecuteSProcedureWithTransaction(List<StoreParameterInfo> storeParameterInfos) { throw new NotImplementedException(); }
        public List<object?> ExecuteScalarSProcedure(out List<string> msgErrors, List<StoreParameterInfo> storeParameterInfos) { throw new NotImplementedException(); }
        public List<object?> ExecuteScalarSProcedureWithTransaction(out List<string> msgErrors, List<StoreParameterInfo> storeParameterInfos) { throw new NotImplementedException(); }
        public List<object> ReturnValuesFromExecuteSProcedure(out string msgError, string sprocedureName, int outputParamCountNumber, params object[] paramObjects) { throw new NotImplementedException(); }
    }
}