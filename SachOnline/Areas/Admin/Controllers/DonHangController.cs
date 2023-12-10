using SachOnline.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SachOnline.Areas.Admin.Controllers
{
	public class DonHangController : Controller
	{
		// GET: Admin/DonHang
		private string connection;
		dbSachOnlineDataContext db;

		public DonHangController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}
		public ActionResult Index(int? page)
		{
			int iPageNum = (page ?? 1);
			int iPageSize = 7;
			return View(db.DONDATHANGs.ToList().OrderBy(n => n.DonDatHangID).ToPagedList(iPageNum, iPageSize));
		}

		public ActionResult Details(int id)
		{
			var donhang = db.DONDATHANGs.SingleOrDefault(n => n.DonDatHangID == id);
			if (donhang == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(donhang);
		}

		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.MaNXB = new SelectList(db.DONDATHANGs.ToList().OrderBy(n => n.DonDatHangID), "MaNXB", "TenNXB");
			return View();
		}

		
		[HttpGet]
		public ActionResult Edit(int id)
		{
			var dh = db.DONDATHANGs.SingleOrDefault(n => n.DonDatHangID == id);
			if (dh == null)
			{
				return HttpNotFound();
			}

			return View(dh);
		}

		[HttpPost]
		public ActionResult Edit(DONDATHANG dh)
		{
			if (ModelState.IsValid)
			{
				var existingdonhang = db.DONDATHANGs.SingleOrDefault(n => n.DonDatHangID == dh.DonDatHangID);

				if (existingdonhang != null)
				{
					existingdonhang.DaThanhToan = dh.DaThanhToan;
					existingdonhang.TinhTrangGiaoHang = dh.TinhTrangGiaoHang;
					existingdonhang.NgayDat = dh.NgayDat;
					existingdonhang.NgayGiao = dh.NgayGiao;

					db.SubmitChanges();
					return RedirectToAction("Index");
				}
			}

			return View(dh);
		}

		[HttpGet]
		public ActionResult Delete(int id)
		{
			var ddh = db.DONDATHANGs.SingleOrDefault(n => n.DonDatHangID == id);
			if (ddh == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(ddh);
		}
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirm(int id)
		{
			var ddh = db.DONDATHANGs.SingleOrDefault(n => n.DonDatHangID == id);
			if (ddh == null)
			{
				Response.StatusCode = 404;
				return null;
			}

			var ctdh = db.CHITIETDATHANGs.Where(s => s.DonDatHangID == id);
			if (ctdh.Count() > 0)
			{
				// Xóa chi tiết đặt hàng trước
				foreach (var chiTiet in ctdh)
				{
					db.CHITIETDATHANGs.DeleteOnSubmit(chiTiet);
				}
				db.SubmitChanges();
			}

			// Sau đó mới xóa đơn đặt hàng
			db.DONDATHANGs.DeleteOnSubmit(ddh);
			db.SubmitChanges();

			return RedirectToAction("Index");
		}
	}
}