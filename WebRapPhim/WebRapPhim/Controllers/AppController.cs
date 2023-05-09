using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using WebRapPhim.Models;
using System.IO;
using MoMo;
using Newtonsoft.Json.Linq;
using mvcDangNhap.common;
using CloudinaryDotNet;
using QRCoder;
using System.Drawing;
using CloudinaryDotNet.Actions;
using System.Data.SqlClient;
using System.Data;
using System.Web.Management;

namespace WebRapPhim.Controllers
{
    public class AppController : Controller
    {
        AppXemPhimEntities1 db = new AppXemPhimEntities1();
        // GET: App
        public ActionResult Home()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return View();
        }


        public ActionResult Phone() {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                    return RedirectToAction("Index","App");
                }
            }
            return View();
        }

        public ActionResult Phone_post() {
            String phone = Request.Form["phone"];
            TempData["PhoneErro"] = null;

            /*if (Session["repwID"] != null)
            {
                return RedirectToAction("ForgotPW", "App");
            }
            if(Session["repwID"] == null)
                return RedirectToAction("Phone", "App");*/


            List<Customer> c = db.Customer.Where(x => x.Phone == phone).ToList();
            if (c.Count == 0)
            {
                TempData["PhoneErro"] = 0;
                return RedirectToAction("Phone", "App");
            }
            else {
                Customer cus = c.FirstOrDefault();

                Session["repwID"] = cus.ID;
                return RedirectToAction("ForgotPW", "App");
            }
           
        }

         public ActionResult ForgotPW()
        {
            if (Session["repwID"] != null)
            {               
                return View();
            }
            else
                return RedirectToAction("Phone" ,"App");
        }

        [HttpPost]
        public ActionResult ForgotPW_post()
        {
            using (MD5 md5Hash = MD5.Create())
            {
                TempData.Clear();
                string oldpw = Request.Form["OldPassword"];
                string newpw = Request.Form["Password"];
                string confrim = Request.Form["ConfirmPassword"];
                if (Session["repwID"] != null)
                {
                    int iduser = int.Parse(Session["repwID"].ToString());
                    if (!db.Customer.Any(x => x.ID == iduser))
                    {
                        Session.Clear();
                        return RedirectToAction("Phone", "App");
                    }
                    else
                    {
                        Customer cus = db.Customer.First(x => x.ID == iduser);
                        if (Service.Service.GetMd5Hash(md5Hash, oldpw) == cus.Password.Trim())
                        {
                            if (newpw.Trim().Equals(confrim.Trim()))
                            {
                                string strConnect = "Data Source=D23159H2;Initial Catalog=AppXemPhim;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
                                SqlConnection cnn = new SqlConnection(strConnect);
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.Connection = cnn;
                                cmd.Parameters.Clear();
                                cmd.CommandText = "Update dbo.Customer set Password = @pw where ID = @id";
                                cmd.Parameters.Add("@pw", SqlDbType.Char).Value = Service.Service.GetMd5Hash(md5Hash, newpw.Trim());
                                cmd.Parameters.Add("@id", SqlDbType.Int).Value = iduser;
                                cmd.Connection = cnn;
                                if (cnn.State == System.Data.ConnectionState.Closed)
                                {
                                    cnn.Open();
                                }
                                cmd.ExecuteNonQuery();

                                TempData["Erro"] = 1;
                                return RedirectToAction("ForgotPW", "App");
                            }
                            else
                            {
                                TempData["Erro"] = -1;
                                return RedirectToAction("ForgotPW", "App");
                            }
                        }
                        else {
                            TempData["Erro"] = 0;
                            return RedirectToAction("ForgotPW", "App");
                        }
                    }
                }
                else
                {
                    TempData.Clear();
                    return RedirectToAction("Login", "App");
                }
               
            }
        }

        [HttpGet]
        public ActionResult Login()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Login(Customer cus)
        {

            using (MD5 md5Hash = MD5.Create())
            {
                string password = Service.Service.GetMd5Hash(md5Hash, cus.Password.Trim());
                var checkLogin = db.Customer.Where(x => x.Phone.Trim() == cus.Phone.Trim() && x.Password.Trim() == password).ToList().FirstOrDefault();
                if (checkLogin != null)
                {

                    Session["UserId"] = checkLogin.ID;
                    Session["UserName"] = checkLogin.Ten;
                    if (GetCart().Items.Count() == 0)
                        return RedirectToAction("Home", "App");
                    else
                        return RedirectToAction("Confirm", "App");
                }
                else
                {
                    ViewBag.Message = "Số điện thoại hoặc mật khẩu sai";
                    return View();
                }
            }

        }



        [HttpPost]
        public ActionResult SignUp(Customer cus)
        {
            using (MD5 md5Hash = MD5.Create())
            {


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
                        string password = Service.Service.GetMd5Hash(md5Hash, cus.Password.Trim());
                        cus.Password = password;
                        cus.Confirm = password;

                        var lastacc = db.Customer.OrderByDescending(a => a.ID).FirstOrDefault();
                        var id = lastacc != null ? lastacc.ID + 1 : 1;
                        cus.ID = id;

                        
                        
                        cus.NgayDangKi = DateTime.Now;
                        cus.DiemThuong = 0;
                        cus.LoaiThanhVien = 4;
                        var sex = Request.Form["sex"];
                        cus.GioiTinh = cus.setGioiTinh(sex);
                        db.Customer.Add(cus);
                        db.SaveChanges();




                        Session["UserId"] = cus.ID;
                        Session["UserName"] = cus.Ten;

                        if (GetCart().Items.Count() ==0)
                            return RedirectToAction("Home", "App");
                        else
                            return RedirectToAction("Confirm", "App");
                    }
                }
                else
                {
                    ViewBag.Message = "Mật khẩu không khớp";
                    return View();
                }


            }
        }




        public ActionResult SignUp()
        {
            return View();
        }

        public ActionResult Logout()
        {

            GetCart().Items.ToList().Clear();
            Session.Clear();
            return RedirectToAction("Login", "App");
        }

        public ActionResult CustomerDetail()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            if (Session["UserId"] != null)
            {
                return View();
            }
            else return RedirectToAction("Login", "App");
        }

        public ActionResult Now_Showing()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return View();
        }

        public ActionResult Coming_soon()
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return View();
        }

        public ActionResult MovieDetail(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return View(db.Film.Where(x => x.ID == id).ToList());
        }
        public ActionResult ChonVe(int id)
        {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            return View(db.SuatChieu.Where(x => x.ID == id && x.PhimID != null && x.PhimID != null).ToList());
        }

        public ActionResult CustomerMemberShip() {
            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            if (Session["UserId"] != null)
                return View();
            else
                return RedirectToAction("Login", "App");
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
        public ActionResult ChonVe_post()
        {
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
            foreach (Ve i in Listve) {
                Ghe ghei = db.Ghe.First(x => x.ID == i.Ghe);
                ghedadat.Add(ghei.ID);
            }
            List<Ghe> ghe = new List<Ghe>();
            foreach (Ghe i in db.Ghe.Where(x => x.PhongID == suatChieu.PhongID)) {
                if (!ghedadat.Contains(i.ID)) {
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
            return RedirectToAction("ChonGhe", "App");
        }


        public ActionResult ChonGhe()
        {
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
            return RedirectToAction("Confirm", "App");
        }

        public ActionResult Confirm()
        {

            if (Session["UserId"] != null)
            {
                int iduser = int.Parse(Session["UserId"].ToString());
                if (!db.Customer.Any(x => x.ID == iduser))
                {
                    Session.Clear();
                }
            }

            if (Session["Cart"] == null)
                return RedirectToAction("Home", "App");
            Cart cart = Session["Cart"] as Cart;
            return View(cart);
        }
        private static readonly HttpClient client = new HttpClient();
        public ActionResult Pay()
        {
            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMO5RGX20191128";
            string accessKey = "M8brj9K6E22vXoDB";
            string serectkey = "nqQiVSgDMy809JoPF6OzP5OdBUB550Y4";
            string orderInfo = "Thanh toán vé ngày " + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss");
            string redirectUrl = "https://localhost:44369/App";
            string ipnUrl = "https://localhost:44369/App";
            string requestType = "captureWallet";

            string amount = Request.Form["tongtien"];
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +   
                "&extraData=" + extraData +
                "&ipnUrl=" + ipnUrl +
                "&orderId=" + orderId +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + redirectUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());
            JObject jmessage = JObject.Parse(responseFromMomo);
            int cusId = 0;
            if (Session["UserId"] != null)
                cusId = int.Parse(Session["UserId"].ToString());

            String email = Request.Form["email"];
            String KH = Request.Form["name"];
            String sdt = Request.Form["sdt"];
            string strConnect = "Data Source=D23159H2;Initial Catalog=AppXemPhim;Integrated Security=True;MultipleActiveResultSets=True;Application Name=EntityFramework";
            SqlConnection cnn = new SqlConnection(strConnect);
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Connection = cnn;
            if (cnn.State == ConnectionState.Closed)
            {
                cnn.Open();
            }

            var lastacc = db.Ve.OrderByDescending(a => a.ID).FirstOrDefault();
            int getIDVeCuoi = lastacc != null ? lastacc.ID : 0;
            

            
            string QRTitle = "";
            for (int i = 0; i < GetCart().Items.Count(); i++) {
                //Xử lý chuỗi sinh ra mã QR
                getIDVeCuoi += 1;
                if (i == 0) QRTitle += (getIDVeCuoi);
                else QRTitle += "-" + (getIDVeCuoi);


                //Lưu hóa đơn

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
            //Sinh mã qr
            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(QRTitle, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);
            Bitmap img = code.GetGraphic(5);
            using (Bitmap bmb = (Bitmap)img.Clone()) {
                bmb.Save("C:\\Users\\DELL.000\\Desktop\\LTCSDL\\WebRapPhim\\WebRapPhim\\MyQR.jpg");
            }


            String cloudname = "dexbjwfjg";
            String apiKey = "575344324738563";
            String apisecret = "ibnB7XPQZBtyfTNsvr5KYTVwKzY";
            Account acc = new Account(cloudname, apiKey, apisecret);
            Cloudinary cloudinary = new Cloudinary(acc);
            var UpLoadParameter = new ImageUploadParams() {
                File = new FileDescription("C:\\Users\\DELL.000\\Desktop\\LTCSDL\\WebRapPhim\\WebRapPhim\\MyQR.jpg")
            };
            var res = cloudinary.Upload(UpLoadParameter);
            string link = res.Uri.ToString();




            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/Emailtemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", email);
            body = body.Replace("{Title}", "Vé đặt ngày " + DateTime.Now.ToString("dd-MM-yyyy"));
            body = body.Replace("{Url}", link);
            body = body.Replace("{Description}", "Đến quầy bán vé để nhân viên xác thực");


            new MailHelper().SendMail(email, "LetHimCook Cinema", body);

            var filePath = Server.MapPath("~/Images/" + "MyQR");
            if (System.IO.File.Exists("C:\\Users\\DELL.000\\Desktop\\LTCSDL\\WebRapPhim\\WebRapPhim\\MyQR.jpg"))
            {
                System.IO.File.Delete("C:\\Users\\DELL.000\\Desktop\\LTCSDL\\WebRapPhim\\WebRapPhim\\MyQR.jpg");
            }

            //Xử lý điểm thưởng

            if (cusId != 0) {
                Customer cus = db.Customer.First(x => x.ID == cusId);
                LoaiThanhVien loaithanhvien = db.LoaiThanhVien.First(x => x.ID == cus.LoaiThanhVien);
                double tongtien = double.Parse(Request.Form["tongtien"]);


                var diemcong = (int)(cus.DiemThuong + (tongtien * loaithanhvien.PhanTramDoiDiem / 100000));

                double SoTienDaMua = 0;
                List<Ve> ves = db.Ve.Where(x => x.CusId == cus.ID).ToList();
                foreach (Ve i in ves) {
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
            return Redirect(jmessage.GetValue("payUrl").ToString());

        }



        private static String getSignature(String text, String key)
        {
            // change according to your needs, an UTF8Encoding
            // could be more suitable in certain situations
            ASCIIEncoding encoding = new ASCIIEncoding();

            Byte[] textBytes = encoding.GetBytes(text);
            Byte[] keyBytes = encoding.GetBytes(key);

            Byte[] hashBytes;

            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
        public ActionResult MemberShip() {
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            return View();
        }

        public ActionResult notify()
        {
            return View();
        }

        public ActionResult BookingHistory(int id) {
            Session["Cart"] = null;
            GetCart().Items.ToList().Clear();
            if (Session["UserId"] != null)
            {
                return View();
            }
            else return RedirectToAction("Login", "App");
        }


        //báo cáo

        public List<report_year_Result> getReport(int year) {
            var list = db.report_year(year).ToList();
            return list;

        }

        public ActionResult loadBaocao(int year) {
            var list = getReport(year);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestBaoCao()
        {

            return View();
        }

    }

}
