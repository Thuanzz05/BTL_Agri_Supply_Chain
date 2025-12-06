
using DailyAgriSupplyChain.DAL.Helper.Interfaces; 
using DailyAgriSupplyChain.DAL.Interfaces;
using DailyAgriSupplyChain.DAL.Models; // Chứa class DonHangDaiLy
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace DailyAgriSupplyChain.DAL
{
    public class DonHangDaiLyRepository : IDonHangDaiLyRepository
    {
        private readonly IDatabaseHelper _dbHelper;

        // DI: Nhận IDatabaseHelper qua constructor
        public DonHangDaiLyRepository(IDatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // --- Hàm hỗ trợ Ánh xạ dữ liệu (Mapping) ---
        private List<DonHangDaiLy> MapDataTableToList(DataTable dt)
        {
            var list = new List<DonHangDaiLy>();
            if (dt == null || dt.Rows.Count == 0) return list;


            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DonHangDaiLy
                {
                    MaDonHang = Convert.ToInt32(row["MaDonHang"]),
                    MaDaiLy = Convert.ToInt32(row["MaDaiLy"]),
                    MaNongDan = Convert.ToInt32(row["MaNongDan"])
                });
            }
            return list;
        }

        // --- Triển khai các phương thức Repository ---

        public async Task<List<DonHangDaiLy>> GetAllAsync()
        {
            string storedProcedure = "sp_DonHangDaiLy_GetAll";

            // Gọi DbHelper để thực thi Stored Procedure
            var dt = await _dbHelper.ExecuteQueryToDataTableAsync(
                storedProcedure,
                CommandType.StoredProcedure);

            return MapDataTableToList(dt);
        }

        public async Task<DonHangDaiLy> GetByIdAsync(int maDonHang)
        {
            string storedProcedure = "sp_DonHangDaiLy_GetById";
            DbParameter[] parameters = new DbParameter[]
            {
                _dbHelper.CreateParameter("@MaDonHang", maDonHang, DbType.Int32)
            };

            var dt = await _dbHelper.ExecuteQueryToDataTableAsync(
                storedProcedure,
                CommandType.StoredProcedure,
                parameters);

            // Trả về phần tử đầu tiên (hoặc null nếu không tìm thấy)
            return MapDataTableToList(dt).FirstOrDefault();
        }

        public async Task<int> CreateAsync(DonHangDaiLy order)
        {
            string storedProcedure = "sp_DonHangDaiLy_Create";
            DbParameter[] parameters = new DbParameter[]
            {
                _dbHelper.CreateParameter("@MaDonHang", order.MaDonHang, DbType.Int32),
                _dbHelper.CreateParameter("@MaDaiLy", order.MaDaiLy, DbType.Int32),
                _dbHelper.CreateParameter("@MaNongDan", order.MaNongDan, DbType.Int32)
            };

            // ExecuteNonQueryAsync trả về số dòng bị ảnh hưởng (hoặc -1 nếu lỗi/không tìm thấy)
            return await _dbHelper.ExecuteNonQueryAsync(
                storedProcedure,
                CommandType.StoredProcedure,
                parameters);
        }

        public async Task<int> UpdateAsync(DonHangDaiLy order)
        {
            string storedProcedure = "sp_DonHangDaiLy_Update";
            DbParameter[] parameters = new DbParameter[]
            {
                _dbHelper.CreateParameter("@MaDonHang", order.MaDonHang, DbType.Int32),
                _dbHelper.CreateParameter("@MaDaiLy", order.MaDaiLy, DbType.Int32),
                _dbHelper.CreateParameter("@MaNongDan", order.MaNongDan, DbType.Int32)
            };

            return await _dbHelper.ExecuteNonQueryAsync(
                storedProcedure,
                CommandType.StoredProcedure,
                parameters);
        }

        public async Task<int> DeleteAsync(int maDonHang)
        {
            string storedProcedure = "sp_DonHangDaiLy_Delete";
            DbParameter[] parameters = new DbParameter[]
            {
                _dbHelper.CreateParameter("@MaDonHang", maDonHang, DbType.Int32)
            };

            return await _dbHelper.ExecuteNonQueryAsync(
                storedProcedure,
                CommandType.StoredProcedure,
                parameters);
        }

        public async Task<List<DonHangDaiLy>> SearchAsync(string searchType, int searchId)
        {
            string storedProcedure = "sp_DonHangDaiLy_Search";
            DbParameter[] parameters = new DbParameter[]
            {
                _dbHelper.CreateParameter("@SearchType", searchType, DbType.String),
                _dbHelper.CreateParameter("@SearchId", searchId, DbType.Int32)
            };

            var dt = await _dbHelper.ExecuteQueryToDataTableAsync(
                storedProcedure,
                CommandType.StoredProcedure,
                parameters);

            return MapDataTableToList(dt);
        }
    }
}