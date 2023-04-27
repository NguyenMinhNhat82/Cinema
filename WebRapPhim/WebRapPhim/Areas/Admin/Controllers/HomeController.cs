using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRapPhim.Models;
using System.Data.Entity;
using System.Globalization;
using System.Data.SqlClient;
using WebRapPhim.Service;
using System.Data.Entity.Validation;
using System.Data;
using System.Security.Cryptography;

namespace WebRapPhim.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        AppXemPhimEntities1 db = new AppXemPhimEntities1();

        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            GetCart().Items.ToList().Clear();
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(NhanVien nv)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                string pw = Service.Service.GetMd5Hash(md5Hash, nv.Password.Trim());
                var checkLogin = db.NhanVien.Where(x => x.TenTK.Trim() == nv.TenTK.Trim() && x.Password.Trim() == pw).ToList().FirstOrDefault();
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

        }

        // CRUD ADMIN X CLIENT
        public ActionResult AdminAccount()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.NhanVien.ToList());
        }
        public ActionResult ClientAccount()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Customer.ToList());
        }
        public ActionResult DetailClientAccount(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult DetailAdminAccount(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult CreateAdminAccount()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateAdminAccount(NhanVien nv)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var checkUser = db.NhanVien.Any(x => x.TenTK == nv.TenTK);
                if (checkUser)
                {
                    ViewBag.Message = "Tên tài khoản đã tồn tại";
                    return View();
                }
                else
                {
                    nv.Password = Service.Service.GetMd5Hash(md5Hash, nv.Password);

                    var lastacc = db.NhanVien.OrderByDescending(a => a.ID).FirstOrDefault();
                    var id = lastacc != null ? lastacc.ID + 1 : 1;
                    nv.ID = id;

                    
                    var sex = Request.Form["sex"];
                    nv.GioiTinh = nv.setGioiTinh(sex);
                    db.NhanVien.Add(nv);
                    db.SaveChanges();
                    return RedirectToAction("AdminAccount", "Home");

                }
            }
        }
        [HttpPost]
        public ActionResult CreateClientAccount(Customer cus)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                var checkUser = db.Customer.Any(x => x.Phone == cus.Phone);
                if (checkUser)
                {
                    ViewBag.Message = "Số điện thoại đăng kí tài khoản đã tồn tại";
                    return View();
                }
                else
                {
                    try
                    {
                        cus.Password = Service.Service.GetMd5Hash(md5Hash, cus.Password);

                        var lastacc = db.Customer.OrderByDescending(a => a.ID).FirstOrDefault();
                        var id = lastacc != null ? lastacc.ID + 1 : 1;
                        cus.ID = id;

                        
                        cus.Confirm = cus.Password;
                        cus.NgayDangKi = DateTime.Now;
                        cus.DiemThuong = 0;
                        cus.LoaiThanhVien = 3;
                        var sex = Request.Form["sex"];
                        cus.GioiTinh = cus.setGioiTinh(sex);
                        db.Customer.Add(cus);
                        db.SaveChanges();
                        if (GetCart().Items.Count() != 0)
                            return Redirect("~/Admin/Home/ChonKhach/" + Session["SuatChieu"]);
                        return RedirectToAction("ClientAccount", "Home");
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
        }
        public ActionResult CreateClientAccount()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }
        public ActionResult EditClientAccount(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditClientAccount(int id, Customer cus)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                try
                {
                    string updatepw = Request.Form["post"];
                    cus.Password = Service.Service.GetMd5Hash(md5Hash, updatepw);
                    cus.Confirm = cus.Password;
                    db.Entry(cus).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Thay đổi thông tin thành công";
                    return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {

                    return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
                }
            }
        }
        public ActionResult EditAdminAccount(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditAdminAccount(int id, NhanVien nv)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                try
                {
                    string updatepw = Request.Form["post"];
                    nv.Password = Service.Service.GetMd5Hash(md5Hash, updatepw);
                    
                    
                    db.Entry(nv).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    ViewBag.Message = "Thay đổi thông tin thành công";
                    return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    return View(db.NhanVien.Where(x => x.ID == id).FirstOrDefault());
                }
            }
        }
        public ActionResult DeleteClientAccount(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Customer.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteClientAccount(int id, FormCollection collection)
        {

            Customer customer = db.Customer.Where(x => x.ID == id).FirstOrDefault();
            db.Customer.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("ClientAccount", "Home");
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
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.NhaCungCap.ToList());
        }
        public ActionResult CreateNhaCungCap()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
                var lastacc = db.NhaCungCap.OrderByDescending(a => a.ID).FirstOrDefault();
                var id = lastacc != null ? lastacc.ID + 1 : 1;
                ncc.ID = id;
                
               
                db.NhaCungCap.Add(ncc);
                db.SaveChanges();
                return RedirectToAction("NhaCungCap", "Home");

            }
        }
        public ActionResult EditNhaCungCap(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.TheLoai.ToList());
        }
        public ActionResult CreateTheLoai()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
                var lastacc = db.TheLoai.OrderByDescending(a => a.id).FirstOrDefault();
                var id = lastacc != null ? lastacc.id + 1 : 1;
                tl.id = id;
                
                tl.id = id;
                db.TheLoai.Add(tl);
                db.SaveChanges();
                return RedirectToAction("TheLoai", "Home");

            }
        }
        public ActionResult EditTheLoai(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
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
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Film.ToList());
        }
        public ActionResult DetailPhim(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Film.Where(x => x.ID == id).FirstOrDefault());
        }
        public ActionResult CreatePhim()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreatePhim(Film f)
        {
            var check = db.Film.Any(x => x.ten == f.ten);
            if (check)
            {
                ViewBag.Message = "Tên đã tồn tại";
                return View();
            }
            else
            {
                string[] theloai = Request.Form.GetValues("the_loai");
                int[] selectedTheLoai = Array.ConvertAll(theloai, int.Parse);
                var lastFilm = db.Film.OrderByDescending(a => a.ID).FirstOrDefault();
                var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                f.ID = id;
                var trangthai = Request.Form["trang_thai"];
                f.trang_thai = f.setTrangThai(trangthai);
                var ncc = int.Parse(Request.Form["nha_cung_cap"]);
                f.nha_cung_cap = ncc;
                db.Film.Add(f);
                db.SaveChanges();
                if (selectedTheLoai != null)
                {
                    foreach (int theloai_id in selectedTheLoai)
                    {
                        TheLoai_Phim tl_phim = new TheLoai_Phim
                        {
                            PhimID = f.ID,
                            LoaiPhimID = theloai_id
                        };
                        db.TheLoai_Phim.Add(tl_phim);
                        db.SaveChanges();
                    }
                }
                return RedirectToAction("Phim", "Home");

            }
        }
        public ActionResult EditPhim(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            Film f = db.Film.Where(x => x.ID == id).FirstOrDefault();
            if (f == null)
            {
                return HttpNotFound();
            }
            // Lấy danh sách thể loại phim
            var allTheLoai = db.TheLoai.ToList();
            ViewBag.AllTheLoai = allTheLoai;

            // Lấy danh sách thể loại phim đã được chọn
            var selectedTheLoai = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
            ViewBag.SelectedTheLoai = selectedTheLoai;

            // Lấy danh sách NCC
            var allNCC = db.NhaCungCap.ToList();
            ViewBag.AllNCC = allNCC;

            var selectedNCC = db.Film.Where(p => p.ID == id).FirstOrDefault().NhaCungCap;
            ViewBag.SelectedNCC = selectedNCC;

            return View(f);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPhim(Film f)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật thông tin phim
                    f.ten = f.ten.Trim();
                    db.Entry(f).State = EntityState.Modified;
                    db.SaveChanges();
                    // Cập nhật danh sách thể loại phim
                    if (Request.Form.GetValues("the_loai") != null && int.Parse(Request.Form["ncc"]) != null)
                    {
                        // Lấy danh sách thể loại phim đã được chọn
                        string[] theloai = Request.Form.GetValues("the_loai");
                        int[] selectedTheLoai = Array.ConvertAll(theloai, int.Parse);

                        int nccID = int.Parse(Request.Form["ncc"]);
                        if (nccID != f.nha_cung_cap)
                        {
                            // Lấy thông tin nhà cung cấp mới từ cơ sở dữ liệu
                            var newNCC = db.NhaCungCap.FirstOrDefault(n => n.ID == nccID);

                            // Nếu tìm thấy nhà cung cấp mới, thực hiện cập nhật
                            if (newNCC != null)
                            {
                                f.nha_cung_cap = newNCC.ID;
                                db.SaveChanges();
                            }
                            ViewBag.SelectedNCC = newNCC;
                            ViewBag.AllNCC = db.NhaCungCap.ToList();
                        }
                        // Xóa toàn bộ thể loại phim đã liên kết với phim này
                        var tl_phim = db.TheLoai_Phim.Where(t => t.PhimID == f.ID);
                        db.TheLoai_Phim.RemoveRange(tl_phim);
                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();
                        // Thêm thể loại phim mới vào database
                        if (selectedTheLoai != null)
                        {
                            foreach (int theloai_id in selectedTheLoai)
                            {
                                TheLoai_Phim tl_phim_new = new TheLoai_Phim
                                {
                                    PhimID = f.ID,
                                    LoaiPhimID = theloai_id
                                };
                                db.TheLoai_Phim.Add(tl_phim_new);
                            }
                            db.SaveChanges();
                        }
                        // Load lại dữ liệu Phim từ database
                        f = db.Film.Where(x => x.ID == f.ID).FirstOrDefault();

                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();

                        // Lấy danh sách thể loại phim đã được chọn
                        var selectedTl = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
                        ViewBag.SelectedTheLoai = selectedTl;

                        ViewBag.Message = "Thay đổi thông tin thành công";
                        return View();
                    }
                    else if (Request.Form.GetValues("the_loai") != null && int.Parse(Request.Form["ncc"]) == null)
                    {
                        // Lấy danh sách thể loại phim đã được chọn
                        string[] theloai = Request.Form.GetValues("the_loai");
                        int[] selectedTheLoai = Array.ConvertAll(theloai, int.Parse);

                        // Xóa toàn bộ thể loại phim đã liên kết với phim này
                        var tl_phim = db.TheLoai_Phim.Where(t => t.PhimID == f.ID);
                        db.TheLoai_Phim.RemoveRange(tl_phim);
                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();
                        // Thêm thể loại phim mới vào database
                        if (selectedTheLoai != null)
                        {
                            foreach (int theloai_id in selectedTheLoai)
                            {
                                TheLoai_Phim tl_phim_new = new TheLoai_Phim
                                {
                                    PhimID = f.ID,
                                    LoaiPhimID = theloai_id
                                };
                                db.TheLoai_Phim.Add(tl_phim_new);
                            }
                            db.SaveChanges();
                        }
                        // Load lại dữ liệu Phim từ database
                        f = db.Film.Where(x => x.ID == f.ID).FirstOrDefault();

                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();

                        // Lấy danh sách thể loại phim đã được chọn
                        var selectedtl = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
                        ViewBag.SelectedTheLoai = selectedtl;

                        // Lấy danh sách NCC
                        var allncc = db.NhaCungCap.ToList();
                        ViewBag.AllNCC = allncc;
                        var selectedncc = db.Film.Where(p => p.ID == f.ID).FirstOrDefault().NhaCungCap;
                        ViewBag.SelectedNCC = selectedncc;
                        ViewBag.Message = "Thay đổi thông tin thành công";
                        return View();
                    }
                    else if (Request.Form.GetValues("the_loai") == null && int.Parse(Request.Form["ncc"]) != null)
                    {
                        int nccID = int.Parse(Request.Form["ncc"]);
                        if (nccID != f.nha_cung_cap)
                        {
                            // Lấy thông tin nhà cung cấp mới từ cơ sở dữ liệu
                            var newNCC = db.NhaCungCap.FirstOrDefault(n => n.ID == nccID);

                            // Nếu tìm thấy nhà cung cấp mới, thực hiện cập nhật
                            if (newNCC != null)
                            {
                                f.nha_cung_cap = newNCC.ID;
                                db.SaveChanges();
                            }
                            ViewBag.SelectedNCC = newNCC;
                            ViewBag.AllNCC = db.NhaCungCap.ToList();
                        }
                        // Load lại dữ liệu Phim từ database
                        f = db.Film.Where(x => x.ID == f.ID).FirstOrDefault();

                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();

                        // Lấy danh sách thể loại phim đã được chọn
                        var selectedtl = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
                        ViewBag.SelectedTheLoai = selectedtl;

                        ViewBag.Message = "Thay đổi thông tin thành công";
                        return View();
                    }
                    else // (Request.Form.GetValues("the_loai") == null && int.Parse(Request.Form["ncc"]) == null)
                    {
                        // Load lại dữ liệu Phim từ database
                        f = db.Film.Where(x => x.ID == f.ID).FirstOrDefault();

                        // Lấy danh sách thể loại phim
                        ViewBag.AllTheLoai = db.TheLoai.ToList();

                        // Lấy danh sách thể loại phim đã được chọn
                        var selectedtl = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
                        ViewBag.SelectedTheLoai = selectedtl;

                        // Lấy danh sách NCC
                        var allncc = db.NhaCungCap.ToList();
                        ViewBag.AllNCC = allncc;
                        var selectedncc = db.Film.Where(p => p.ID == f.ID).FirstOrDefault().NhaCungCap;
                        ViewBag.SelectedNCC = selectedncc;
                        ViewBag.Message = "Thay đổi thông tin thành công";
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    //ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                    ViewBag.Message = "Lỗi thông tin khi nhập, vui lòng nhập lại !!";
                }
            }

            // Nếu thông tin không hợp lệ, hiển thị lại trang với thông tin cũ
            // Load lại dữ liệu Phim từ database
            f = db.Film.Where(x => x.ID == f.ID).FirstOrDefault();

            // Lấy danh sách thể loại phim
            ViewBag.AllTheLoai = db.TheLoai.ToList();

            // Lấy danh sách thể loại phim đã được chọn
            var selectedTL = f.TheLoai_Phim.Select(t => t.LoaiPhimID).ToList();
            ViewBag.SelectedTheLoai = selectedTL;

            // Lấy danh sách NCC
            var allNCC = db.NhaCungCap.ToList();
            ViewBag.AllNCC = allNCC;
            var selectedNCC = db.Film.Where(p => p.ID == f.ID).FirstOrDefault().NhaCungCap;
            ViewBag.SelectedNCC = selectedNCC;
            ViewBag.Message = "Thay đổi thông tin thành công";
            return View();
        }
        public ActionResult DeletePhim(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Film.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeletePhim(int id, FormCollection collection)
        {
            Film f = db.Film.Find(id);
            if (f == null)
            {
                return HttpNotFound();
            }
            // Xóa các bản ghi trong bảng TheLoai_Phim có liên kết với phim này
            var tl_phim = db.TheLoai_Phim.Where(t => t.PhimID == id);
            db.TheLoai_Phim.RemoveRange(tl_phim);
            db.SaveChanges();
            // Xóa phim
            db.Film.Remove(f);
            db.SaveChanges();
            return RedirectToAction("Phim", "Home");
        }

        // CRUD PHÒNG
        public ActionResult Phong()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Phong.ToList());
        }
        public ActionResult CreatePhong()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreatePhong(Phong p)
        {
            var check = db.Phong.Any(x => x.ID == p.ID);
            if (check)
            {
                ViewBag.Message = "Phòng đã tồn tại";
                return View();
            }
            else
            {
                db.Phong.Add(p);
                db.SaveChanges();
                return RedirectToAction("Phong", "Home");

            }
        }
        public ActionResult DeletePhong(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Phong.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeletePhong(int id, FormCollection collection)
        {
            Phong p = db.Phong.Where(x => x.ID == id).FirstOrDefault();
            db.Phong.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Phong", "Home");
        }

        // CRUD Ghe
        public ActionResult Ghe()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Ghe.ToList());
        }
        public ActionResult CreateGhe()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }
        [HttpPost]
        public ActionResult CreateGhe(Ghe g)
        {
            var check = db.Ghe.Any(x => x.ID == g.ID);
            if (check)
            {
                ViewBag.Message = "Ghế đã tồn tại";
                return View();
            }
            else
            {
                Phong phong = db.Phong.First(x => x.ID == g.PhongID);
                List<Ghe> ghe = db.Ghe.Where(x => x.PhongID == phong.ID).ToList();
                if (ghe.Count == int.Parse(phong.SoLuongGhe)) {
                    ViewBag.Message = "Phòng đã đầy ghế";
                    return View();
                }
                var status = Request.Form["trang_thai"];
                g.TrangThai = g.setTrangThai(status);
                db.Ghe.Add(g);
                db.SaveChanges();
                return RedirectToAction("Ghe", "Home");
            }
        }
        public ActionResult DeleteGhe(string id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.Ghe.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteGhe(string id, FormCollection collection)
        {
            Ghe g = db.Ghe.Where(x => x.ID == id).FirstOrDefault();
            db.Ghe.Remove(g);
            db.SaveChanges();
            return RedirectToAction("Ghe", "Home");
        }

        // CRUD SuatChieu
        public ActionResult SuatChieu()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.SuatChieu.Where(x => x.PhimID!=null && x.PhimID != null).ToList());
        }
        public ActionResult CreateSuatChieu()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View();
        }

        //Bán vé
        [HttpGet]
        public ActionResult BanVe() {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            string kw = Request.QueryString["kw"];
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;

            int year = DateTime.Now.Year;
            var suatchieu = new List<SuatChieu>();
            DateTime Today = new DateTime(year,month,day);
            DateTime Tomorrow = Today.AddDays(1);
            if (kw != null)
            {
                var a  = db.SuatChieu.Where(x => x.GioBatDau > DateTime.Now && x.GioBatDau < Tomorrow && x.PhimID!=null && x.PhimID!=null).ToList();
                var phim = db.Film.Where(x => x.ten.Contains(kw)).ToList();
                List<int> idphim = new List<int>();
                foreach (Film f in phim) {
                    idphim.Add(f.ID);
                }
                foreach (SuatChieu s in a) {
                    if (idphim.Contains(s.ID))
                        suatchieu.Add(s);
                }
               
            }
            else {
                suatchieu = db.SuatChieu.Where(x => x.PhimID != null && x.PhimID != null && x.GioBatDau > DateTime.Now && x.GioBatDau < Tomorrow).ToList();
                
            }

            return View(suatchieu);

        }

        public ActionResult ChonKhach(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            List<Customer> customers = new List<Customer>();
            string sdt = Request.QueryString["sdt"];
            string ten = Request.QueryString["ten"];

            if (sdt == "")
                sdt = null;
            if (ten == "")
                ten = null;
            String sql = "Select ID from dbo.Customer";
            if (sdt != null || ten != null)
                sql += " where";
            if(sdt != null)
            {
                sql += " Phone = '" + sdt+"'";
            }
            if (ten != null && sdt == null)
            {
                sql += " Ten like '%" + ten+"%'";
            }
            if (ten != null && sdt !=null ) {
                sql += " and Ten like '%" + ten+"%'";
            }

            SqlDataReader reader;
            string strConnect = "Data Source=D23159H2;Initial Catalog=AppXemPhim;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
            SqlConnection cnn = new SqlConnection(strConnect);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Connection = cnn;

            cmd.CommandText = sql;
            cmd.Connection = cnn;
            if (cnn.State == System.Data.ConnectionState.Closed)
            {
                cnn.Open();
            }

            reader = cmd.ExecuteReader();
            while (reader.Read()) {
                int cusid = reader.GetInt32(0);
                Customer cus = db.Customer.First(x => x.ID == cusid);
                customers.Add(cus);
            }

            Session["SuatChieu"] = id;
            return View(customers);
        }

        [HttpPost]
        public ActionResult ChonKhach_post() {
            var iduser = Request.Form["userpick"];
            if (iduser == "")
            {
                Session["MaKhach"] = null;
                return RedirectToAction("ChonSoLuong", "Home");
            }
            else {
                Session["MaKhach"] = iduser;
                return RedirectToAction("ChonSoLuong","Home");
            }
        }

        public ActionResult ChonSoLuong() {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            int id = int.Parse(Session["SuatChieu"].ToString());
            return View(db.SuatChieu.Where(x => x.ID == id && x.PhimID != null && x.PhimID != null).ToList());
        }

        public Cart GetCart()
        {
            Cart cart = Session["Cart"] as Cart;
            if (cart == null || Session["Cart"] == null)
            {
                cart = new Cart();
                Session["Cart"] = cart;
            }
            return cart;
        }

        [HttpPost]
        public ActionResult ChonSoLuong_post() {
            List<LoaiVe> ve = db.LoaiVe.ToList();
            List<int> SlLoaive = new List<int>();
            foreach (LoaiVe l in ve)
            {

                int sl = int.Parse(Request.Form[string.Concat("sl", l.ID)]);
                SlLoaive.Add(sl);
            }
            int idsuatchieu = int.Parse(Request.Form["suatchieu"]);
            SuatChieu suatChieu = db.SuatChieu.First(x => x.ID == idsuatchieu && x.PhimID != null && x.PhimID != null);
            Film phim = db.Film.First(x => x.ID == suatChieu.PhimID);
            List<Ve> Listve = db.Ve.ToList();
            List<String> ghedadat = new List<String>();
            foreach (Ve i in Listve)
            {
                Ghe ghei = db.Ghe.First(x => x.ID == i.Ghe);
                ghedadat.Add(ghei.ID);
            }
            List<Ghe> ghe = new List<Ghe>();
            foreach (Ghe i in db.Ghe.Where(x => x.PhongID == suatChieu.PhongID))
            {
                if (!ghedadat.Contains(i.ID))
                {
                    ghe.Add(i);
                }
            }
            int demve = 0;
            for (int i = 0; i < ve.Count; i++)
            {
                for (int j = 0; j < SlLoaive[i]; j++)
                {
                    GetCart().Add(ve[i], phim, suatChieu, ghe[demve]);
                    demve++;
                }
            }
            return RedirectToAction("ChonGhe", "Home");
        }

        public ActionResult ChonGhe()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            if (Session["Cart"] == null)
                return RedirectToAction("Home", "App");
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        [HttpPost]
        public ActionResult ChonGhe_post_update()
        {
            for (int i = 0; i < GetCart().Items.Count(); i++)
            {
                int row = int.Parse(Request.Form[string.Concat("row", i)]);
                int col = int.Parse(Request.Form[string.Concat("col", i)]);
                int idphong = int.Parse(GetCart().Items.First().ghe.PhongID.ToString());
                String idghe = Service.Service.getGhe(row + 1, col + 1, idphong);
                GetCart().UpdateGhe(i, idghe);
            }
            return RedirectToAction("Confirm", "Home");
        }

        public ActionResult Confirm()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            if (Session["Cart"] == null)
                return RedirectToAction("Home", "App");
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }

        [HttpPost]
        public ActionResult Pay()
        {
            string amount = Request.Form["tongtien"];
            int getIDVeCuoi = db.Ve.ToList().LastOrDefault().ID;
            String email = Request.Form["email"];
            String KH = Request.Form["name"];
            String sdt = Request.Form["sdt"];
            string strConnect = "Data Source=D23159H2;Initial Catalog=AppXemPhim;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
            SqlConnection cnn = new SqlConnection(strConnect);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            int cusId = 0;
            if (Session["MaKhach"] != null)
                cusId = int.Parse(Session["MaKhach"].ToString());
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }
            for (int i = 0; i < GetCart().Items.Count(); i++)
            {
                //Xử lý chuỗi sinh ra mã QR
                getIDVeCuoi += 1;
                string k = "";
                if (cusId == 0) k = "NULL";
                else k = cusId.ToString();
                cmd.Parameters.Clear();
                cmd.CommandText = "Insert into dbo.Ve values(@id,@date,@gheid,NULL," + k + ",@name,@sdt,@email,@gia,NULL,@suatchieuid,@loaive) ";
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = (getIDVeCuoi);
                cmd.Parameters.Add("@date", SqlDbType.DateTime2).Value = DateTime.Now;
                cmd.Parameters.Add("@gheid", SqlDbType.NChar).Value = GetCart().Items.ToList()[i].ghe.ID;


                cmd.Parameters.Add("@name", SqlDbType.NChar).Value = KH;
                cmd.Parameters.Add("@sdt", SqlDbType.NChar).Value = sdt;
                cmd.Parameters.Add("@email", SqlDbType.NChar).Value = email;
                cmd.Parameters.Add("@gia", SqlDbType.Float).Value = GetCart().Items.ToList()[i].getGia();

                cmd.Parameters.Add("@suatchieuid", SqlDbType.Int).Value = GetCart().Items.ToList()[i].SuatChieu.ID;
                cmd.Parameters.Add("@loaive", SqlDbType.Int).Value = GetCart().Items.ToList()[i].loaiVe.ID;
                cmd.ExecuteNonQuery();
            }

            //Diem thưởng
            if (cusId != 0)
            {
                Customer cus = db.Customer.First(x => x.ID == cusId);
                LoaiThanhVien loaithanhvien = db.LoaiThanhVien.First(x => x.ID == cus.LoaiThanhVien);
                double tongtien = double.Parse(Request.Form["tongtien"]);


                var diemcong = (int)(cus.DiemThuong + (tongtien * loaithanhvien.PhanTramDoiDiem / 100000));

                double SoTienDaMua = 0;
                List<Ve> ves = db.Ve.Where(x => x.CusId == cus.ID).ToList();
                foreach (Ve i in ves)
                {
                    SoTienDaMua += double.Parse(i.Gia.ToString());
                }
                int thanhvien = 4;
                if (SoTienDaMua >= 1000000 && SoTienDaMua < 5000000)
                    thanhvien = 3;
                if (SoTienDaMua >= 5000000 && SoTienDaMua < 15000000)
                    thanhvien = 2;
                if (SoTienDaMua >= 15000000)
                    thanhvien = 1;
                cmd.Parameters.Clear();
                cmd.CommandText = "update dbo.Customer set DiemThuong = @diemthuong, LoaiThanhVien = @loaithanhvien where ID =@id";
                cmd.Parameters.Add("@diemthuong", SqlDbType.Int).Value = diemcong;
                cmd.Parameters.Add("@loaithanhvien", SqlDbType.Int).Value = thanhvien;
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = cus.ID;
                cmd.ExecuteNonQuery();
                //Cập nhập lại số điểm thưởng đã sử dụng
                cmd.Parameters.Clear();
                int diemthuongsudung = int.Parse(Request.Form["diemthuongsudung"]);
                cmd.CommandText = cmd.CommandText = "update dbo.Customer set DiemThuong -= @diemthuong where ID =@id";
                cmd.Parameters.Add("@diemthuong", SqlDbType.Int).Value = diemthuongsudung;
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return RedirectToAction("Index","Home");
        }
            [HttpPost]
        public ActionResult CreateSuatChieu(SuatChieu sc, int phongId, int PhimID, DateTime GioBatDau)
        {
            var phim = db.Film.SingleOrDefault(p => p.ID == PhimID);
            DateTime NgayCongChieu = (DateTime)phim.ngay_cong_chieu;
            int result = GioBatDau.CompareTo(NgayCongChieu);
            if (result < 0)
            {
                ViewBag.Message = "Giờ bắt đầu phải sau Ngày Công Chiếu của Phim";
                return View();
            }
            else
            {                 // Lấy thông tin phòng đang được chọn
                var phong = db.Phong.FirstOrDefault(p => p.ID == phongId);
                var trangthai = Request.Form["trang_thai"];
                if (phong == null)
                {
                    ViewBag.Message = "Không có tồn tại phòng !";
                    return View();
                }
                if (db.SuatChieu.FirstOrDefault(p => p.PhongID == phongId && p.PhimID!=null) == null)
                {
                    var lastFilm = db.SuatChieu.OrderByDescending(a => a.ID).FirstOrDefault();
                    var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                    // Tạo suất phim mới
                    var suatPhim = new SuatChieu
                    {
                        GioBatDau = GioBatDau,
                        PhimID = PhimID,
                        PhongID = phongId,
                        TrangThai = sc.setTrangThai(trangthai),
                        Gia = sc.Gia,
                        ID = id,
                    };
                    db.SuatChieu.Add(suatPhim);
                    db.SaveChanges();
                    return RedirectToAction("SuatChieu", "Home");
                }
                else
                {
                    var existedSuatChieu = db.SuatChieu.Where(p => p.PhongID == phongId && p.PhimID!=null).ToList();
                    existedSuatChieu = existedSuatChieu.OrderBy(p => p.GioBatDau).ToList();
                    var existedPhim = new List<Film>();
                    if (existedSuatChieu.Count != 0)
                    {
                        foreach (SuatChieu i in existedSuatChieu)
                        {
                            Film f = db.Film.First(x => x.ID == i.PhimID);
                            existedPhim.Add(f);
                        }
                        if (existedPhim.Count == 1)
                        {
                            DateTime temp = (DateTime)existedSuatChieu[0].GioBatDau;
                            DateTime GioKetThucexistedPhim = temp.AddMinutes((double)existedPhim[0].thoi_luong);
                            int r = GioBatDau.CompareTo(GioKetThucexistedPhim);
                            if (r < 0)
                            {
                                ViewBag.Message = "Suất phim ở phòng và giờ bắt đầu mà bạn muốn tạo đang có Phim đang chiếu !";
                                return View();
                            }
                            else
                            {
                                var lastFilm = db.SuatChieu.OrderByDescending(a => a.ID).FirstOrDefault();
                                var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                                // Tạo suất phim mới
                                var suatPhim = new SuatChieu
                                {
                                    GioBatDau = GioBatDau,
                                    PhimID = PhimID,
                                    PhongID = phongId,
                                    TrangThai = sc.setTrangThai(trangthai),
                                    Gia = sc.Gia,
                                    ID = id,
                                };
                                db.SuatChieu.Add(suatPhim);
                                db.SaveChanges();
                                return RedirectToAction("SuatChieu", "Home");
                            }
                        }
                        else
                        {
                            DateTime a = ((DateTime)existedSuatChieu[existedSuatChieu.Count - 1].GioBatDau).AddMinutes((double)existedPhim[existedPhim.Count - 1].thoi_luong);
                            if (GioBatDau < existedSuatChieu[0].GioBatDau || GioBatDau > a)
                            {
                                var lastFilm = db.SuatChieu.OrderByDescending(b => b.ID).FirstOrDefault();
                                var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                                // Tạo suất phim mới
                                var suatPhim = new SuatChieu
                                {
                                    GioBatDau = GioBatDau,
                                    PhimID = PhimID,
                                    PhongID = phongId,
                                    TrangThai = sc.setTrangThai(trangthai),
                                    Gia = sc.Gia,
                                    ID = id,
                                };
                                db.SuatChieu.Add(suatPhim);
                                db.SaveChanges();
                                return RedirectToAction("SuatChieu", "Home");
                            }
                            for (int i = 0; i < existedPhim.Count-1; i++)
                            {
                                DateTime GioKetThucCuaI = ((DateTime)existedSuatChieu[i].GioBatDau).AddMinutes((double)existedPhim[i].thoi_luong);
                                DateTime GioBatDauCuaJ = (DateTime)existedSuatChieu[i+1].GioBatDau;
                                if (GioBatDau > GioKetThucCuaI && GioBatDau < GioBatDauCuaJ)
                                {
                                    var lastFilm = db.SuatChieu.OrderByDescending(c => c.ID).FirstOrDefault();
                                    var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                                    // Tạo suất phim mới
                                    var suatPhim = new SuatChieu
                                    {
                                        GioBatDau = GioBatDau,
                                        PhimID = PhimID,
                                        PhongID = phongId,
                                        TrangThai = sc.setTrangThai(trangthai),
                                        Gia = sc.Gia,
                                        ID = id,
                                    };
                                    db.SuatChieu.Add(suatPhim);
                                    db.SaveChanges();
                                    return RedirectToAction("SuatChieu", "Home");
                                }

                            }
                            ViewBag.Message = "Suất phim ở phòng và giờ bắt đầu mà bạn muốn tạo đang có Phim đang chiếu !";
                            return View();
                        }
                    }
                    else
                    {
                        var lastFilm = db.SuatChieu.OrderByDescending(a => a.ID).FirstOrDefault();
                        var id = lastFilm != null ? lastFilm.ID + 1 : 1;
                        // Tạo suất phim mới
                        var suatPhim = new SuatChieu
                        {
                            GioBatDau = GioBatDau,
                            PhimID = PhimID,
                            PhongID = phongId,
                            TrangThai = sc.setTrangThai(trangthai),
                            Gia = sc.Gia,
                            ID = id,
                        };
                        db.SuatChieu.Add(suatPhim);
                        db.SaveChanges();
                        return RedirectToAction("SuatChieu", "Home");
                    }

                }
            }
        }    
        public ActionResult DeleteSuatChieu(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.NhanVien.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Login", "Home");
                }
            }
            return View(db.SuatChieu.Where(x => x.ID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult DeleteSuatChieu(int id, FormCollection collection)
        {
            SuatChieu sc = db.SuatChieu.Where(x => x.ID == id).FirstOrDefault();
            db.SuatChieu.Remove(sc);
            db.SaveChanges();
            return RedirectToAction("SuatChieu", "Home");
        }
    }
}