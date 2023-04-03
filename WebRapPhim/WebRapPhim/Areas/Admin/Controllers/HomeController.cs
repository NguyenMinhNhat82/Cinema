using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRapPhim.Models;

namespace WebRapPhim.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        AppXemPhimEntities1 db = new AppXemPhimEntities1();

        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(NhanVien nv)
        {
            var checkLogin = db.NhanVien.Where(x => x.TenTK == nv.TenTK && x.Password == nv.Password).ToList().FirstOrDefault();
            if (checkLogin != null)
            {
                Session["UserId"] = checkLogin.ID;
                Session["UserName"] = checkLogin.Ten;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Message = "Tên tài khoản hoặc mật khẩu sai";
                return View();
            }

        }
        // CRUD ADMIN X CLIENT
        public ActionResult AdminAccount()
        {
            return View(db.NhanVien.ToList());
        }
        public ActionResult ClientAccount()
        {
            return View(db.Customer.ToList());
        }
        public ActionResult DetailClientAccount(int id)
        {
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult DetailAdminAccount(int id)
        {
            return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult CreateAdminAccount()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdminAccount(NhanVien nv)
        {
            var checkUser = db.NhanVien.Any(x => x.TenTK == nv.TenTK);
            if (checkUser)
            {
                ViewBag.Message = "Tên tài khoản đã tồn tại";
                return View();
            }
            else
            {
                var id = db.NhanVien.Count() + 1;
                nv.ID = id;
                var sex = Request.Form["sex"];
                nv.GioiTinh = nv.setGioiTinh(sex);
                db.NhanVien.Add(nv);
                db.SaveChanges();
                return RedirectToAction("AdminAccount", "Home");

            }
        }
        [HttpPost]
        public ActionResult CreateClientAccount(Customer cus)
        {
            var checkUser = db.Customer.Any(x => x.Phone == cus.Phone);
            if (checkUser)
            {
                ViewBag.Message = "Số điện thoại đăng kí tài khoản đã tồn tại";
                return View();
            }
            else
            {
                var id = db.Customer.Count() + 1;
                cus.ID = id;
                cus.NgayDangKi = DateTime.Now;
                cus.DiemThuong = 0;
                cus.LoaiThanhVien = 3;
                var sex = Request.Form["sex"];
                cus.GioiTinh = cus.setGioiTinh(sex);
                db.Customer.Add(cus);
                db.SaveChanges();
                return RedirectToAction("ClientAccount", "Home");
            }
        }
        public ActionResult CreateClientAccount()
        {
            return View();
        }
        public ActionResult EditClientAccount(int id)
        {
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditClientAccount(int id, Customer cus)
        {
            try
            {
                db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Thay đổi thông tin thành công";
                return View();
            }
            catch (Exception ex)
            {

                return View();
            }
        }
        public ActionResult EditAdminAccount(int id)
        {
            return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditAdminAccount(int id, NhanVien nv)
        {
            try
            {
                db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Thay đổi thông tin thành công";
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }
        public ActionResult DeleteClientAccount(int id)
        {
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteClientAccount(int id, FormCollection collection)
        {
            Customer customer = db.Customer.Where(x => x.ID == id).FirstOrDefault();
            db.Customer.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("EditClientAccount", "Home");
        }
        public ActionResult DeleteAdminAccount(int id)
        {
            return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteAdminAccount(int id, FormCollection collection)
        {
            NhanVien nhanvien = db.NhanVien.Where(x => x.ID == id).FirstOrDefault();
            db.NhanVien.Remove(nhanvien);
            db.SaveChanges();
            return RedirectToAction("AdminAccount", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }
        // CRUD NHA CUNG CAP
        public ActionResult NhaCungCap()
        {
            return View(db.NhaCungCap.ToList());
        }
        public ActionResult CreateNhaCungCap()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateNhaCungCap(NhaCungCap ncc)
        {
            var checkUser = db.NhaCungCap.Any(x => x.Ten == ncc.Ten);
            if (checkUser)
            {
                ViewBag.Message = "Tên đã tồn tại";
                return View();
            }
            else
            {
                var id = db.NhaCungCap.Count() + 1;
                ncc.ID = id;
                db.NhaCungCap.Add(ncc);
                db.SaveChanges();
                return RedirectToAction("NhaCungCap", "Home");

            }
        }
        public ActionResult EditNhaCungCap(int id)
        {
            return View(db.NhaCungCap.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditNhaCungCap(int id, NhaCungCap ncc)
        {
            try
            {
                db.Entry(ncc).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Thay đổi thông tin thành công";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Tên nhà cung cấp quá dài";
                return View();
            }
        }
        public ActionResult DeleteNhaCungCap(int id)
        {
            return View(db.NhaCungCap.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteNhaCungCap(int id, FormCollection collection)
        {
            NhaCungCap ncc = db.NhaCungCap.Where(x => x.ID == id).FirstOrDefault();
            db.NhaCungCap.Remove(ncc);
            db.SaveChanges();
            return RedirectToAction("NhaCungCap", "Home");
        }
        // CRUD THE LOAI
        public ActionResult TheLoai()
        {
            return View(db.TheLoai.ToList());
        }
        public ActionResult CreateTheLoai()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateTheLoai(TheLoai tl)
        {
            var check = db.TheLoai.Any(x => x.ten_the_loai == tl.ten_the_loai);
            if (check)
            {
                ViewBag.Message = "Tên đã tồn tại";
                return View();
            }
            else
            {
                var id = db.TheLoai.Count() + 1;
                tl.id = id;
                db.TheLoai.Add(tl);
                db.SaveChanges();
                return RedirectToAction("TheLoai", "Home");

            }
        }
        public ActionResult EditTheLoai(int id)
        {
            return View(db.TheLoai.Where(x => x.id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditTheLoai(int id, TheLoai tl)
        {
            try
            {
                db.Entry(tl).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Thay đổi thông tin thành công";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Tên thể loại ít hơn 40 ký tự";
                return View();
            }
        }
        public ActionResult DeleteTheLoai(int id)
        {
            return View(db.TheLoai.Where(x => x.id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteTheLoai(int id, FormCollection collection)
        {
            TheLoai tl = db.TheLoai.Where(x => x.id == id).FirstOrDefault();
            db.TheLoai.Remove(tl);
            db.SaveChanges();
            return RedirectToAction("TheLoai", "Home");
        }

        // CRUD PHIM
        public ActionResult Phim()
        {
            return View(db.Film.ToList());
        }
        public ActionResult DetailPhim(int id)
        {
            return View(db.Film.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult CreatePhim()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreatePhim(TheLoai tl)
        {
            var check = db.TheLoai.Any(x => x.ten_the_loai == tl.ten_the_loai);
            if (check)
            {
                ViewBag.Message = "Tên đã tồn tại";
                return View();
            }
            else
            {
                var id = db.TheLoai.Count() + 1;
                tl.id = id;
                db.TheLoai.Add(tl);
                db.SaveChanges();
                return RedirectToAction("TheLoai", "Home");

            }
        }
        public ActionResult EditPhim(int id)
        {
            return View(db.TheLoai.Where(x => x.id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditPhim(int id, TheLoai tl)
        {
            try
            {
                db.Entry(tl).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                ViewBag.Message = "Thay đổi thông tin thành công";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Tên thể loại ít hơn 40 ký tự";
                return View();
            }
        }
        public ActionResult DeletePhim(int id)
        {
            return View(db.TheLoai.Where(x => x.id == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeletePhim(int id, FormCollection collection)
        {
            TheLoai tl = db.TheLoai.Where(x => x.id == id).FirstOrDefault();
            db.TheLoai.Remove(tl);
            db.SaveChanges();
            return RedirectToAction("TheLoai", "Home");

            // CRUD THE LOAI
        }
    }
}