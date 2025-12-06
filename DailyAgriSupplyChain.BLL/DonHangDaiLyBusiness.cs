using DailyAgriSupplyChain.DAL.Interfaces;
using DailyAgriSupplyChain.DAL.Models;
using DailyAgriSupplyChain.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DailyAgriSupplyChain.BLL
{
    public partial class DonHangDaiLyBusiness : IDonHangDaiLyBusiness
    {
        private IDonHangDaiLyRepository _res;

        public DonHangDaiLyBusiness(IDonHangDaiLyRepository res)
        {
            _res = res;
        }

        // --- TRIỂN KHAI BUSINESS LOGIC ---

        public bool Create(DonHangDaiLy model)
        {
            // Logic Nghiệp vụ: Kiểm tra input
            if (model.MaDaiLy <= 0 || model.MaNongDan <= 0)
            {
                throw new ArgumentException("Mã Đại Lý và Mã Nông Dân không hợp lệ.");
            }
            return _res.Create(model);
        }

        public bool Update(DonHangDaiLy model)
        {
            if (model.MaDonHang <= 0)
            {
                throw new ArgumentException("Mã Đơn Hàng không hợp lệ để cập nhật.");
            }
            return _res.Update(model);
        }

        public bool Delete(int maDonHang)
        {
            if (maDonHang <= 0)
            {
                throw new ArgumentException("Mã Đơn Hàng không hợp lệ để xóa.");
            }
            return _res.Delete(maDonHang);
        }

        public DonHangDaiLy? GetById(int maDonHang)
        {
            return _res.GetById(maDonHang);
        }

        public List<DonHangDaiLy> GetAll()
        {
            return _res.GetAll();
        }

        public List<DonHangDaiLy> Search(string searchType, int searchId)
        {
            if (searchType.ToLower() != "daily" && searchType.ToLower() != "nongdan")
            {
                throw new ArgumentException("Loại tìm kiếm không hợp lệ.");
            }

            return _res.Search(searchType, searchId);
        }
    }
}