using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebBanDoCongNghe.Models;

namespace WebBanDoCongNghe.Controllers
{
    public class LoginController : Controller
    {
        DBQuanLyBanDoCongNgheEntities db = new DBQuanLyBanDoCongNgheEntities();
       
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(tb_Customer model)
        {
            if (ModelState.IsValid)
            {
                var check = db.tb_Customer.FirstOrDefault(x => x.TaiKhoan == model.TaiKhoan);
                if(check==null)
                {
                    model.MatKhau = GetMD5(model.MatKhau);
                    model.IsAdmin = false;
                    model.IsActive = true;
                    model.CreateDate = DateTime.Now;
                    model.UpdatedDate = DateTime.Now;
                    db.tb_Customer.Add(model);
                    db.SaveChanges();
                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.error= "Tài khoản đã tồn tại";
                    return this.Register();
                }
            }
            ViewBag.error = "Tạo tài khoản thất bại xin vui lòng thử lại";
            return this.Register();
        }
        public static string GetMD5(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] bHash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder sbHash = new StringBuilder();

            foreach (byte b in bHash)
            {
                sbHash.Append(String.Format("{0:x2}", b));
            }
            return sbHash.ToString();
        }

        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string taikhoan, string matkhau)
        {
            if(ModelState.IsValid)
            {
                var Password = GetMD5(matkhau);
                var customer = db.tb_Customer.Where(x => x.TaiKhoan.Equals(taikhoan) && x.MatKhau.Equals(Password)).ToList();
                tb_Customer kh=db.tb_Customer.SingleOrDefault(x => x.TaiKhoan.Equals(taikhoan) && x.MatKhau.Equals(Password));
                if (customer.Count>0)
                {
                    if (kh.IsActive == true)
                    {
                        Session["taikhoan"] = kh;
                        Session["MaKH"] = customer.FirstOrDefault().MaKH;
                        Session["HoTen"] = customer.FirstOrDefault().HoTen;
                        Session["Email"] = customer.FirstOrDefault().Email;

                        return RedirectToAction("IndexHome", "Home");
                    }
                    else
                    {
                        ViewBag.error = "Tài khoản của bạn đã bị khóa";
                        return this.Login();
                    }
                  
                }
                else
                {
                    ViewBag.error = "Tài khoản hoặc mật khẩu không đúng";
                    return this.Login();
                }
               
            }
            ViewBag.error = "Đăng nhập thất bại xin vui lòng thử lại";
            return this.Login();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("indexHome","Home");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                    db.Dispose();
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}