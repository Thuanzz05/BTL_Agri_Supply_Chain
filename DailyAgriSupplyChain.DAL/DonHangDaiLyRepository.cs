// File: DailyAgriSupplyChain.DAL\DonHangDaiLyRepository.cs

using DailyAgriSupplyChain.DAL.Interfaces;
using DailyAgriSupplyChain.DAL.Models;
using DailyAgriSupplyChain.DAL.Helper.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;

namespace DailyAgriSupplyChain.DAL
{
    public partial class DonHangDaiLyRepository : IDonHangDaiLyRepository
    {
        private readonly IDatabaseHelper _dbHelper;

        public DonHangDaiLyRepository(IDatabaseHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        // Hàm hỗ trợ ánh xạ 
        private DonHangDaiLy MapDataRowToModel(DataRow row)
        {
            return new DonHangDaiLy
            {
                MaDonHang = Convert.ToInt32(row["MaDonHang"]),
                MaDaiLy = Convert.ToInt32(row["MaDaiLy"]),
                MaNongDan = Convert.ToInt32(row["MaNongDan"]),
            };
        }

        private List<DonHangDaiLy> MapDataTableToList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return new List<DonHangDaiLy>();
            return dt.AsEnumerable().Select(row => MapDataRowToModel(row)).ToList();
        }

        // --- TRIỂN KHAI CÁC PHƯƠNG THỨC ---

        public bool Create(DonHangDaiLy model)
        {
            string msgError = "";
            try
            {
                var result = _dbHelper.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_DonHangDaiLy_Create",
                    "@MaDonHang", model.MaDonHang,
                    "@MaDaiLy", model.MaDaiLy,
                    "@MaNongDan", model.MaNongDan);

                if (result != null && !string.IsNullOrEmpty(result.ToString()) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception("Lỗi DAL khi tạo đơn hàng: " + Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception) { throw; }
        }

        public bool Update(DonHangDaiLy model)
        {
            string msgError = "";
            try
            {
                var result = _dbHelper.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_DonHangDaiLy_Update",
                    "@MaDonHang", model.MaDonHang,
                    "@MaDaiLy", model.MaDaiLy,
                    "@MaNongDan", model.MaNongDan);

                if (result != null && !string.IsNullOrEmpty(result.ToString()) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception("Lỗi DAL khi cập nhật đơn hàng: " + Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception) { throw; }
        }

        public DonHangDaiLy? GetById(int maDonHang)
        {
            string msgError = "";
            try
            {
                var dt = _dbHelper.ExecuteSProcedureReturnDataTable(out msgError, "sp_DonHangDaiLy_GetById",
                    "@MaDonHang", maDonHang);

                if (!string.IsNullOrEmpty(msgError)) throw new Exception(msgError);

                return MapDataTableToList(dt).FirstOrDefault();
            }
            catch (Exception) { throw; }
        }

        public bool Delete(int maDonHang)
        {
            string msgError = "";
            try
            {
                var result = _dbHelper.ExecuteScalarSProcedureWithTransaction(out msgError, "sp_DonHangDaiLy_Delete",
                    "@MaDonHang", maDonHang);

                if (result != null && !string.IsNullOrEmpty(result.ToString()) || !string.IsNullOrEmpty(msgError))
                {
                    throw new Exception("Lỗi DAL khi xóa đơn hàng: " + Convert.ToString(result) + msgError);
                }
                return true;
            }
            catch (Exception) { throw; }
        }

        public List<DonHangDaiLy> GetAll()
        {
            string msgError = "";
            try
            {
                var dt = _dbHelper.ExecuteSProcedureReturnDataTable(out msgError, "sp_DonHangDaiLy_GetAll");

                if (!string.IsNullOrEmpty(msgError)) throw new Exception(msgError);

                return MapDataTableToList(dt);
            }
            catch (Exception) { throw; }
        }

        public List<DonHangDaiLy> Search(string searchType, int searchId)
        {
            string msgError = "";
            try
            {
                var dt = _dbHelper.ExecuteSProcedureReturnDataTable(out msgError, "sp_DonHangDaiLy_Search",
                    "@SearchType", searchType,
                    "@SearchId", searchId);

                if (!string.IsNullOrEmpty(msgError)) throw new Exception(msgError);

                return MapDataTableToList(dt);
            }
            catch (Exception) { throw; }
        }
    }
}