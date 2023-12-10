using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;
using System.Web.UI.WebControls;

namespace SachOnline.Areas.Admin.Controllers
{
	public class KhachHangController : Controller
	{
		private string connection;
		dbSachOnlineDataContext db;

		public KhachHangController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}
		// GET: Admin/khachhang
		public ActionResult Index(int? page)
		{
			int iPageNum = (page ?? 1);
			int iPageSize = 7;
			return View(db.KHACHHANGs.ToList().OrderBy(n => n.KhachHangID).ToPagedList(iPageNum, iPageSize));
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.MaKH = new SelectList(db.KHACHHANGs.ToList().OrderBy(n => n.KhachHangID), "MaKH", "HoTen");
			return View();
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Create(KHACHHANG khachhang, FormCollection f)
		{
			if (ModelState.IsValid)
			{
				khachhang.TenKhachHang = f["sHoTen"];
				khachhang.TenDN = f["sTaiKhoan"];
				khachhang.MatKhau = f["sMatKhau"];
				khachhang.Email = f["sEmail"];
				khachhang.DiaChi = f["sDiaChi"];
				khachhang.SoDienThoai = f["sDienThoai"];
				
				db.KHACHHANGs.InsertOnSubmit(khachhang);
				db.SubmitChanges();
				//Vé trang Quån sach
				return RedirectToAction("Index");
			}

			return View(khachhang);
		}

		public ActionResult Details(int id)
		{
			var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.KhachHangID == id);
			if (khachhang == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(khachhang);
		}

		[HttpGet]
		public ActionResult Delete(int id)
		{
			var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.KhachHangID == id);
			if (khachhang == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(khachhang);
		}
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirm(int id, FormCollection f)
		{
			var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.KhachHangID == id);
			if (khachhang == null)
			{
				Response.StatusCode = 404;
				return null;
			}

			var ddh = db.KHACHHANGs.Where(s => s.KhachHangID == id);
			if (ddh.Count() > 0)
			{
				ViewBag.ThongBao = "Không thể xóa khách hàng này vì có Đơn đặt hàng liên quan đến khách hàng này. Hãy yêu cầu khách hàng thanh toán các đơn đặt hàng trước khi xóa khách hàng.";
				return View(khachhang);
			}

			db.KHACHHANGs.DeleteOnSubmit(khachhang);
			db.SubmitChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var khachhang = db.KHACHHANGs.SingleOrDefault(n => n.KhachHangID == id);
			if (khachhang == null)
			{
				return HttpNotFound();
			}

			return View(khachhang);
		}

		[HttpPost]
		public ActionResult Edit(KHACHHANG khachhang)
		{
			if (ModelState.IsValid)
			{
				var existingkhachhang = db.KHACHHANGs.SingleOrDefault(n => n.KhachHangID == khachhang.KhachHangID);

				if (existingkhachhang != null)
				{
					existingkhachhang.TenKhachHang = khachhang.TenKhachHang;
					existingkhachhang.TenDN = khachhang.TenDN;
					existingkhachhang.MatKhau = khachhang.MatKhau;
					existingkhachhang.Email = khachhang.Email;
					existingkhachhang.DiaChi = khachhang.DiaChi;
					existingkhachhang.SoDienThoai = khachhang.SoDienThoai;

					db.SubmitChanges();
					return RedirectToAction("Index");
				}
			}

			return View(khachhang);
		}

	}
}
