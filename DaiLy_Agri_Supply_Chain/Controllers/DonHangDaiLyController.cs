using DailyAgriSupplyChain.BLL.Interfaces;
using DailyAgriSupplyChain.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DaiLy_Agri_Supply_Chain.Controllers
{
    [Route("api/donhangdaily")]
    [ApiController]
    public class DonHangDaiLyController : ControllerBase
    {
        private readonly IDonHangDaiLyBusiness _business;

        public DonHangDaiLyController(IDonHangDaiLyBusiness business)
        {
            _business = business;
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            try
            {
                var data = _business.GetAll();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi lấy dữ liệu: " + ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var data = _business.GetById(id);
                if (data == null) return NotFound("Mã đơn hàng không tìm thấy.");
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest("Lỗi khi tìm kiếm: " + ex.Message);
            }
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] DonHangDaiLy model)
        {
            try
            {
                _business.Create(model);
                return CreatedAtAction(nameof(GetById), new { id = model.MaDonHang }, model);
            }
            catch (Exception ex)
            {
                return BadRequest("Tạo đơn hàng thất bại: " + ex.Message);
            }
        }

        [HttpPost("update")]
        public IActionResult Update([FromBody] DonHangDaiLy model)
        {
            try
            {
                _business.Update(model);
                return NoContent(); // 204 Success, no content
            }
            catch (Exception ex)
            {
                return BadRequest("Cập nhật thất bại: " + ex.Message);
            }
        }

        [HttpDelete("delete/{id:int}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _business.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest("Xóa thất bại: " + ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string searchType, [FromQuery] int searchId)
        {
            try
            {
                var data = _business.Search(searchType, searchId);
                return Ok(data);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi server: " + ex.Message);
            }
        }
    }
}