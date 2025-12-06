using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Data.SqlClient; // Cần thiết để tham chiếu SqlConnection

namespace DailyAgriSupplyChain.DAL.Helper.Interfaces
{
    // Đảm bảo class này được định nghĩa ở đâu đó trong namespace này
    public class StoreParameterInfo
    {
        public string StoreProcedureName { get; set; } = null!;
        public List<object> StoreProcedureParams { get; set; } = null!;
    }

    public interface IDatabaseHelper
    {
        // --- PHẦN ASYNC (MODERN) ---

        Task<int> ExecuteNonQueryAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null);
        Task<object?> ExecuteScalarAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null);
        Task<DataTable> ExecuteQueryToDataTableAsync(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null);

        // --- PHẦN SYNC CƠ BẢN (TRADITIONAL) ---

        int ExecuteNonQuery(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null);
        object? ExecuteScalar(string sql, CommandType commandType = CommandType.Text, DbParameter[] parameters = null);
        DataTable ExecuteQueryToDataTable(string sql, out string msgError, CommandType commandType = CommandType.Text, DbParameter[] parameters = null); // Đổi tên để tránh xung đột

        // --- HÀM SP PHỨC TẠP (PARAMS OBJECT[]) ---
        string ExecuteSProcedure(string sprocedureName, params object[] paramObjects);
        DataTable ExecuteSProcedureReturnDataTable(out string msgError, string sprocedureName, params object[] paramObjects);
        DataSet? ExecuteSProcedureReturnDataset(out string msgError, string sprocedureName, params object[] paramObjects);
        string ExecuteSProcedure(SqlConnection sqlConnection, string sprocedureName, params object[] paramObjects); // Hàm này yêu cầu SqlConnection
        string ExecuteSProcedureWithTransaction(string sprocedureName, params object[] paramObjects);
        object? ExecuteScalarSProcedure(out string msgError, string sprocedureName, params object[] paramObjects);
        object? ExecuteScalarSProcedureWithTransaction(out string msgError, string sprocedureName, params object[] paramObjects);
        List<object> ReturnValuesFromExecuteSProcedure(out string msgError, string sprocedureName, int outputParamCountNumber, params object[] paramObjects);

        // --- HÀM SP PHỨC TẠP (LIST<STOREPARAMETERINFO>) ---
        List<string> ExecuteScalarSProcedure(List<StoreParameterInfo> storeParameterInfos);
        List<string> ExecuteSProcedureWithTransaction(List<StoreParameterInfo> storeParameterInfos);
        List<object?> ExecuteScalarSProcedure(out List<string> msgErrors, List<StoreParameterInfo> storeParameterInfos);
        List<object?> ExecuteScalarSProcedureWithTransaction(out List<string> msgErrors, List<StoreParameterInfo> storeParameterInfos);

        // --- HÀM HỖ TRỢ ---
        DbParameter CreateParameter(string name, object? value, DbType dbType);
    }
}