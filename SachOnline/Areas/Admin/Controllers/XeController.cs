using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SachOnline.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace SachOnline.Areas.Admin.Controllers
{
    public class XeController : Controller
    {
		private string connection;
		dbSachOnlineDataContext db;

		public XeController()
		{
			// Khởi tạo chuỗi kết nối
			connection = "Data Source=LAPTOP-VRO7LLTN\\SQLEXPRESS;Initial Catalog=SachOnline;Integrated Security=True";
			db = new dbSachOnlineDataContext(connection);
		}
		// GET: Admin/Sach
		public ActionResult Index(int ? page)
        {
			int iPageNum = (page ?? 1);
			int iPageSize = 7;
            return View(db.SACHes.ToList().OrderBy( n => n.SachID).ToPagedList(iPageNum, iPageSize));
        }
		[HttpGet]
		public ActionResult Create() 
		{
			//Lay ds tu cac table ChuDe, NXB. Show ten, khi chon se lay ID
			ViewBag.ChuDeID = new SelectList(db.CHUDEs.ToList().OrderBy(n=>n.TenChuDe), "ChuDeID", "TenChuDe");
			ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan), "NhaXuatBanID", "TenNhaXuatBan"); 
			return View();
		}
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Create(SACH sach, FormCollection f, HttpPostedFileBase fFileUpload)
		{
			//Dua du lieu vao bang DropDown
			ViewBag.ChuDeID = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe");
			ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan), "MaNXB", "TenNXB");

			if (fFileUpload == null)
			{
				ViewBag.ThongBao = "Hãy chọn ảnh bìa cho sách!";
				//luu thong tin khi load lai trang
				ViewBag.TenSach = f["sTenSach"];
				ViewBag.Mota = f["sMoTa"];
				ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
				ViewBag.GiaBan = decimal.Parse(f["mGiaBan"]);
				ViewBag.ChuDeID = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", int.Parse(f["MaCD"]));
				ViewBag.NhaXuatBanID = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan), "MaNXB", "TenNXB", int.Parse(f["MaNXB"]));

				return View();
			}
			else
			{
				if (ModelState.IsValid)
				{
					//Lay ten file 
					var sFileName = Path.GetFileName(fFileUpload.FileName);
					//Lay duong dan
					var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
					//Kiem tra anh bia ton tai?
					if (!System.IO.File.Exists(path))
					{
						fFileUpload.SaveAs(path);
					}
					//Luu vao CSDL
					sach.TenSach = f["sTenSach"];
					sach.Mota = f["sMoTa"];
					sach.anhSP = sFileName;
					sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
					sach.SoLuong = int.Parse(f["iSoLuong"]);
					sach.GiaBan = double.Parse(f["mGiaBan"]);
					sach.ChuDeID = int.Parse(f["ChuDeID"]);
					sach.NhaXuatBanID = int.Parse(f["NhaXuatBanID"]);
					db.SACHes.InsertOnSubmit(sach);
					db.SubmitChanges();
					//Vé trang Quan ly sach
					return RedirectToAction("Index");
				}
			return View();
			}
		}
		public ActionResult Details(int id)
		{
			var sach = db.SACHes.SingleOrDefault(n => n.SachID == id);
			if (sach == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(sach);
		}
		[HttpGet]
		public ActionResult Delete(int id)
		{
			var sach = db.SACHes.SingleOrDefault(n => n.SachID == id);
			if (sach == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			return View(sach);
		}
		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirm(int id, FormCollection f)
		{
			var sach = db.SACHes.SingleOrDefault(n => n.SachID == id);
			if (sach == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			var ctdh = db.CHITIETDATHANGs.Where(ct => ct.SachID == id);
			if (ctdh.Count() > 0)
			{
				ViewBag.ThongBao = "Sách này đang có trong bảng Chi tiết đặt hàng <br>" + "Nếu muốn xóa thì phải xóa hết mã sách này trong bảng Chi tiết đặt hàng";
				return View(sach);
			}
			var vietsach = db.VIETSACHes.Where(vs => vs.SachID == id).ToList();
			if (vietsach != null)
			{
				db.VIETSACHes.DeleteAllOnSubmit(vietsach);
				db.SubmitChanges();
			}
			db.SACHes.DeleteOnSubmit(sach);
			db.SubmitChanges();

			return RedirectToAction("Index");
		}

		[HttpGet]
		public ActionResult Edit(int id)
		{
			var sach = db.SACHes.SingleOrDefault(n => n.SachID == id);
			if (sach == null)
			{
				Response.StatusCode = 404;
				return null;
			}
			ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "ChuDeID", "TenChuDe", sach.ChuDeID);
			ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan), "NhaXuatBanID", "TenNhaXuatBan", sach.NhaXuatBanID);

			return View(sach);
		}
		[HttpPost]
		[ValidateInput(false)]
		public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
		{
			var sach = db.SACHes.SingleOrDefault(n => n.SachID == int.Parse(f["iMaSach"]));
			ViewBag.MaCD = new SelectList(db.CHUDEs.ToList().OrderBy(n => n.TenChuDe), "MaCD", "TenChuDe", sach.ChuDeID);
			ViewBag.MaNXB = new SelectList(db.NHAXUATBANs.ToList().OrderBy(n => n.TenNhaXuatBan), "NhaXuatBanID", "TenNXB", sach.NhaXuatBanID);

			if (ModelState.IsValid)
			{
				if (fFileUpload != null)
				{
					var sFileName = Path.GetFileName(fFileUpload.FileName);
					var path = Path.Combine(Server.MapPath("~/Images"), sFileName);

					if (!System.IO.File.Exists(path))
					{
						fFileUpload.SaveAs(path);
					}
					sach.anhSP = sFileName;
				}

				sach.TenSach = f["sTenSach"];
				sach.Mota = f["sMoTa"];
				// Kiểm tra xem ngày cập nhật có giá trị hợp lệ hay không
				if (f["dNgayCapNhat"] != null)
				{
					// Chuyển ngày cập nhật từ chuỗi sang kiểu DateTime
					sach.NgayCapNhat = DateTime.Now;
				}
				else
				{
					// Xử lý trường hợp ngày cập nhật không hợp lệ (nếu cần)
					sach.NgayCapNhat = Convert.ToDateTime(f["dNgayCapNhat"]);
				}

				sach.SoLuong = int.Parse(f["iSoLuong"]);
				sach.GiaBan = int.Parse(f["mGiaBan"]);
				sach.ChuDeID = int.Parse(f["MaCD"]);
				sach.NhaXuatBanID = int.Parse(f["MaNXB"]);

				db.SubmitChanges();
				//Vé trang QL SACH
				return RedirectToAction("Index");
			}
			return View(sach);
		}
	}
}