using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DailyAgriSupplyChain.DAL.Helper.Interfaces
{
    public interface IDatabaseHelper
    {

        // --- Hàm thực thi chung cho SQL/Stored Procedure ---

        /// <summary>
        /// Thực thi câu lệnh SQL/Stored Procedure không trả về tập dữ liệu (INSERT, UPDATE, DELETE).
        /// </summary>
        /// <returns>Số dòng bị ảnh hưởng.</returns>
        Task<int> ExecuteNonQueryAsync(
            string sql,
            CommandType commandType = CommandType.Text,
            DbParameter[] parameters = null);

        /// <summary>
        /// Thực thi câu lệnh SQL/Stored Procedure trả về giá trị đơn lẻ (COUNT, MAX).
        /// </summary>
        /// <returns>Đối tượng giá trị trả về.</returns>
        Task<object> ExecuteScalarAsync(
            string sql,
            CommandType commandType = CommandType.Text,
            DbParameter[] parameters = null);

        /// <summary>
        /// Thực thi câu lệnh SQL/Stored Procedure trả về tập dữ liệu.
        /// </summary>
        /// <returns>DataTable chứa kết quả.</returns>
        Task<DataTable> ExecuteQueryToDataTableAsync(
            string sql,
            CommandType commandType = CommandType.Text,
            DbParameter[] parameters = null);

        // --- Hàm hỗ trợ ---

        /// <summary>
        /// Tạo một tham số DB an toàn cho câu lệnh SQL.
        /// </summary>
        DbParameter CreateParameter(string name, object value, DbType dbType);

        // *Ghi chú: Để đơn giản hóa, các hàm xử lý Transaction phức tạp (như List<StoreParameterInfo>) 
        // nên được xử lý trong Repository bằng cách inject DatabaseHelper và tự quản lý Transaction nếu cần.
    }
}