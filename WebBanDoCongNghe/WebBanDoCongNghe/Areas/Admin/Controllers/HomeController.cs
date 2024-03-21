using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDoCongNghe.Models;

namespace WebBanDoCongNghe.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        DBQuanLyBanDoCongNgheEntities db = new DBQuanLyBanDoCongNgheEntities();
        // GET: Admin/Home
        public ActionResult Index()
        {
            if (Session["admin"] == null)
            {
                return RedirectToAction("IndexLoginAdmin", "LoginAdmin");
            }
           
            //thông kê tất cả doanh thu người dùng từ khi website thành lâp
            decimal tongDoanhThuCustomer = db.tb_ChiTietOrder.Sum(x => x.Quantity * x.Price).Value;
            ViewBag.tongDoanhThuCustomer = tongDoanhThuCustomer;
            //Thống kê daonh thu khách vãng lai từ khi website thành lập
            decimal tongDoanhThuTraveler= db.tb_ChiTietOrder_Traveler.Sum(x => x.Quantity * x.Price).Value;
            ViewBag.tongDoanhThuTraveler = tongDoanhThuTraveler;
            //Thống kê đơn hàng mới
            int SoDonHang = 0;
            int SoDonHangCustomer = db.tb_Order.Where(x=>x.IsHoanThanh==false && x.IsHuyDon==false).Count();
            int SoDonHangTraveler = db.tb_Traveler.Where(x => x.IsHoanThanh == false && x.IsHuyDon == false).Count();
            ViewBag.SoDonHang = SoDonHang = SoDonHangCustomer + SoDonHangTraveler;
            //Thống kê khách hàng mới
            int SoLuongKhachHang= db.tb_Customer.Count();
            ViewBag.SoLuongKhachHang = SoLuongKhachHang;
            //lấy số lượng người đã truy truy cập đc tạo ra từ  application đã đc tạo
            ViewBag.SoNguoiGheTham = HttpContext.Application["visit"].ToString();
            //lấy số lượng người đang truy truy cập đc tạo ra từ  application đã đc tạo
            ViewBag.SoNguoiOnline = HttpContext.Application["Online"].ToString();
            return View();
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