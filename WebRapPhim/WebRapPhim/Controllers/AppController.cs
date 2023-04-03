using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebRapPhim.Models;

namespace WebRapPhim.Controllers
{
    public class AppController : Controller
    {
        AppXemPhimEntities1 db = new AppXemPhimEntities1();
        // GET: App
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Customer cus)
        {
            var checkLogin = db.Customer.Where(x => x.Phone == cus.Phone && x.Password == cus.Password).ToList().FirstOrDefault();
            if (checkLogin !=null)
            {
                
                Session["UserId"] = checkLogin.ID;
                Session["UserName"] = checkLogin.Ten;
                return RedirectToAction("Home", "App");
            }
            else {
                ViewBag.Message = "Số điện thoại hoặc mật khẩu sai";
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult SignUp(Customer cus) {


            if (cus.Password == cus.Confirm)
            {
                
                var checkUser = db.Customer.Any(x => x.Phone == cus.Phone);
                if (checkUser)
                {
                    ViewBag.Message = "Số điện thoại đăng kí tài khoản đã tồn tại";
                    return View();
                }
                else
                {
                    var id = db.Customer.Count()+1;
                    cus.ID = id;
                    cus.NgayDangKi = DateTime.Now;
                    cus.DiemThuong = 0;
                    cus.LoaiThanhVien = 3;
                    var sex = Request.Form["sex"];
                    cus.GioiTinh = cus.setGioiTinh(sex);
                    db.Customer.Add(cus);
                    db.SaveChanges();
                    Session["UserId"] = cus.ID;
                    Session["UserName"] = cus.Ten;
                    return RedirectToAction("Home", "App");
                }
            }
            else {
                ViewBag.Message = "Mật khẩu không khớp";
                return View();
            }

            
        }

        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login","App");
        }

        public ActionResult Now_Showing()
        {
            return View();
        }

        public ActionResult Coming_soon()
        {
            return View();
        }
    }
}