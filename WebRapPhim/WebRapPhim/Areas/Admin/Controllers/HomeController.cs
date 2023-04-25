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
            return View(db.Phong.ToList());
        }
        public ActionResult CreatePhong()
        {
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
            return View(db.Ghe.ToList());
        }
        public ActionResult CreateGhe()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateGhe(Ghe g)
        {
            var check = db.Ghe.Any(x => x.ID == g.ID);
            if (check)
            {
                ViewBag.Message = "Phòng đã tồn tại";
                return View();
            }
            else
            {
                var status = Request.Form["trang_thai"];
                g.TrangThai = g.setTrangThai(status);
                db.Ghe.Add(g);
                db.SaveChanges();
                return RedirectToAction("Ghe", "Home");
            }
        }
        public ActionResult DeleteGhe(string id)
        {
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
            return View(db.SuatChieu.ToList());
        }
        public ActionResult CreateSuatChieu()
        {
            return View();
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
                if (db.SuatChieu.FirstOrDefault(p => p.PhongID == phongId) == null)
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
                    var existedSuatChieu = db.SuatChieu.Where(p => p.PhongID == phongId).ToList();
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