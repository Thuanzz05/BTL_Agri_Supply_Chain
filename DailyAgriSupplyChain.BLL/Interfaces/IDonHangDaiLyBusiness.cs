using DailyAgriSupplyChain.DAL.Models;
using System.Collections.Generic;

namespace DailyAgriSupplyChain.BLL.Interfaces
{
    public interface IDonHangDaiLyBusiness
    {
        bool Create(DonHangDaiLy model);
        bool Update(DonHangDaiLy model);
        DonHangDaiLy? GetById(int maDonHang);
        bool Delete(int maDonHang);
        List<DonHangDaiLy> GetAll();
        List<DonHangDaiLy> Search(string searchType, int searchId);
    }
}