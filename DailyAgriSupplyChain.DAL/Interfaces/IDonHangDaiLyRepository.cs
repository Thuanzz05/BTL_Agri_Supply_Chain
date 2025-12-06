
using DailyAgriSupplyChain.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DailyAgriSupplyChain.DAL.Interfaces
{
    public interface IDonHangDaiLyRepository
    {
        Task<List<DonHangDaiLy>> GetAllAsync();
        Task<DonHangDaiLy> GetByIdAsync(int maDonHang);
        Task<int> CreateAsync(DonHangDaiLy order);
        Task<int> UpdateAsync(DonHangDaiLy order);
        Task<int> DeleteAsync(int maDonHang);

        Task<List<DonHangDaiLy>> SearchAsync(string searchType, int searchId);
    }
}