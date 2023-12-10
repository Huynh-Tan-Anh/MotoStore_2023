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
	public class XuatXuController : Controller
	{
		private string connection;
		dbSachOnlineDataContext db;

		public XuatXuController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}
		// GET: Admin/NhaXuatBan
		public ActionResult Index(int? page)
		{
			int iPageNum = (page ?? 1);
			int iPageSize = 7;
			return View(db.NHAXUATBANs.ToList().OrderBy(n => n.NhaXuatBanID).ToPagedList(iPageNum, iPageSize));
		}


		[HttpGet]
		public ActionResult Create()
		{
			ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.NhaXuatBanID), "NhaXuatBanID", "TenNXB");
			return View();
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Create(NHAXUATBAN nhaxuatban, FormCollection f)
		{
			ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan));
			if (ModelState.IsValid)
			{
				nhaxuatban.TenNhaXuatBan = f["sTenNXB"];
				nhaxuatban.DiaChi = f["sDiaChi"];

				db.NHAXUATBANs.InsertOnSubmit(nhaxuatban);
				db.SubmitChanges();
				//Vé trang Quån sach
				return RedirectToAction("Index");
			}

			return View(nhaxuatban);
		}

		public ActionResult Details(int id)
		{
			var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.NhaXuatBanID == id);
			if (nhaxuatban == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(nhaxuatban);
		}

		[HttpGet]
		public ActionResult Delete(int id)
		{
			var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.NhaXuatBanID == id);
			if (nhaxuatban == null)
			{
				return HttpNotFound();
			}
			return View(nhaxuatban);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id, FormCollection f)
		{
			var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.NhaXuatBanID == id);
			if (nhaxuatban == null)
			{
				Response.StatusCode = 404;
				return null;
			}

			var sach = db.SACHes.Where(s => s.NhaXuatBanID == id);
			if (sach.Count() > 0)
			{
				ViewBag.ThongBao = "Không thể xóa nhà xuất bản này vì có sách liên quan đến nó. Hãy xóa các sách trước khi xóa nhà xuất bản.";
				return View(nhaxuatban);
			}

			db.NHAXUATBANs.DeleteOnSubmit(nhaxuatban);
			db.SubmitChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var nhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.NhaXuatBanID == id);
			if (nhaxuatban == null)
			{
				return HttpNotFound();
			}

			return View(nhaxuatban);
		}

		[HttpPost]
		public ActionResult Edit(NHAXUATBAN nhaxuatban)
		{
			if (ModelState.IsValid)
			{
				var existingnhaxuatban = db.NHAXUATBANs.SingleOrDefault(n => n.NhaXuatBanID == nhaxuatban.NhaXuatBanID);

				if (existingnhaxuatban != null)
				{
					existingnhaxuatban.TenNhaXuatBan = nhaxuatban.TenNhaXuatBan;

					db.SubmitChanges();
					return RedirectToAction("Index");
				}
			}

			return View(nhaxuatban);
		}
	}

}