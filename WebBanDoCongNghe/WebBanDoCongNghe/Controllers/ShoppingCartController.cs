//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using WebBanDoCongNghe.Models;

//namespace WebBanDoCongNghe.Controllers
//{
//    public class ShoppingCartController : Controller
//    {
//        DBQuanLyBanDoCongNgheEntities db = new DBQuanLyBanDoCongNgheEntities();
//        // GET: GioHang
//        // GET: ShoppingCart
//        public ActionResult IndexShoppingCart()
//        {

//            return View();
//        }
//        public ActionResult Partial_Item_Cart()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                return PartialView(cart.Items);
//            }
//            return PartialView();
//        }
//        public ActionResult MaGiamGia(FormCollection form)
//        {   //hiển thị số tiền đã giảm
//            var MaGiamGia = form["txtMaGiamGia"].ToString();
//            ViewBag.GiamGia = db.tb_DiscountCode.FirstOrDefault(n => n.DiscountCode == MaGiamGia).Value;
//            //hiển thị tổng giá tiền sau khi giảm
//            decimal soTienGiamGia = (decimal)db.tb_DiscountCode.FirstOrDefault(n => n.DiscountCode == MaGiamGia).Value;
//            decimal tienSauKhiGiam = TongTien() - soTienGiamGia;
//            ViewBag.tienSauKhiGiam = tienSauKhiGiam;

//            tb_DiscountCode discountCode = db.tb_DiscountCode.FirstOrDefault(x => x.DiscountCode.Equals(MaGiamGia));
//            Session["discount"] = discountCode;


//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                return PartialView(cart.Items);
//            }
//            return View();
//        }
//        public decimal TongTien()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            decimal tongTien = 0;
//            foreach (var item in cart.Items)
//            {
//                tongTien = tongTien + item.iTongGia;
//            }
//            return tongTien;
//        }
//        public ActionResult GioHangPartial()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                return PartialView(cart.Items);
//            }
//            return PartialView();
//        }
//        public ActionResult CheckOut()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                ViewBag.CheckCart = cart;
//            }
//            ViewBag.PhuongThucThanhToan = new SelectList(db.tb_PhuongThucThanhToan.ToList(), "MaPhuongThucThanhToan", "TenPhuongThucThanhToan");
//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult CheckOut(tb_Traveler model)
//        {
//            if (ModelState.IsValid)
//            {
//                ShoppingCart cart = (ShoppingCart)Session["Cart"];
//                if (cart != null)
//                {
//                    tb_Traveler traveler = new tb_Traveler();

//                    foreach (var item in cart.Items)
//                    {
//                        tb_ChiTietOrder_Traveler chiTietOrder = new tb_ChiTietOrder_Traveler();
//                        chiTietOrder.MaDonHang = traveler.MaDonHang;
//                        chiTietOrder.MaSanPham = item.iMaSanPham;
//                        chiTietOrder.Quantity = item.iSoLuong;
//                        chiTietOrder.Price = (decimal)item.iGiaSanPham;
//                        db.tb_ChiTietOrder_Traveler.Add(chiTietOrder);
//                    }
//                    model.TotalPayment = cart.Items.Sum(x => x.iGiaSanPham * x.iSoLuong);
//                    model.IsThanhToan = false;
//                    model.IsHoanThanh = false;
//                    model.IsHuyDon = false;
//                    model.CreateDate = DateTime.Now;
//                    db.tb_Traveler.Add(model);
//                    db.SaveChanges();
//                    cart.ClearCart();
//                    return RedirectToAction("CheckOutSuccess");
//                }

//            }
//            return View(model);
//        }
//        public ActionResult CheckOutUser()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                ViewBag.CheckCart = cart;
//            }
//            ViewBag.PhuongThucThanhToan = new SelectList(db.tb_PhuongThucThanhToan.ToList(), "MaPhuongThucThanhToan", "TenPhuongThucThanhToan");
//            return View();
//        }
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult CheckOutUser(tb_Order model)
//        {
//            if (ModelState.IsValid)
//            {
//                ShoppingCart cart = (ShoppingCart)Session["Cart"];
//                if (cart != null)
//                {
//                    if (Session["MaKH"] != null)
//                    {
//                        tb_Customer customer = (tb_Customer)Session["taikhoan"];

//                        foreach (var item in cart.Items)
//                        {
//                            tb_ChiTietOrder chiTietOrder = new tb_ChiTietOrder();
//                            chiTietOrder.MaDonHang = model.MaDonHang;
//                            chiTietOrder.MaSanPham = item.iMaSanPham;
//                            chiTietOrder.Quantity = item.iSoLuong;
//                            chiTietOrder.Price = (decimal)item.iGiaSanPham;
//                            db.tb_ChiTietOrder.Add(chiTietOrder);
//                        }
//                        model.MaKH = customer.MaKH;
//                        model.TotalPayment = cart.Items.Sum(x => x.iGiaSanPham * x.iSoLuong);
//                        model.IsThanhToan = false;
//                        model.IsHoanThanh = false;
//                        model.IsHuyDon = false;
//                        model.CreateDate = DateTime.Now;
//                        db.tb_Order.Add(model);
//                        db.SaveChanges();
//                        cart.ClearCart();
//                        return RedirectToAction("CheckOutSuccess");
//                    }

//                }

//            }
//            return View(model);
//        }
//        public ActionResult CheckOutSuccess()
//        {
//            return View();
//        }

//        public ActionResult ShowCount()
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                return Json(new { Count = cart.GetTotalQuantity() }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
//        }
//        [HttpPost]
//        public ActionResult AddToCart(int id, int quantity)
//        {
//            var code = new { Success = false, msg = "", code = -1, Count = 0 };
//            var checkProduct = db.tb_Product.FirstOrDefault(x => x.MaSanPham == id);
//            if (checkProduct != null)
//            {
//                ShoppingCart cart = (ShoppingCart)Session["Cart"];
//                if (cart == null)
//                {
//                    cart = new ShoppingCart();
//                }
//                ShoppingCartItem item = new ShoppingCartItem
//                {
//                    iMaSanPham = checkProduct.MaSanPham,
//                    iTenSanPham = checkProduct.TenSanPham,
//                    iLink = checkProduct.Link,
//                    iSoLuong = quantity

//                };
//                if (checkProduct.tb_ProductImages.FirstOrDefault(x => x.IsDefault == true) != null)
//                {
//                    item.iHinhAnh = db.tb_Product.FirstOrDefault(x => x.MaSanPham == id).tb_ProductImages.FirstOrDefault(x => x.IsDefault == true).Image;
//                }
//                if (checkProduct.IsSale == true)
//                {
//                    item.iGiaSanPham = (decimal)checkProduct.PriceSale;
//                }
//                else
//                {
//                    item.iGiaSanPham = (decimal)checkProduct.Price;
//                }

//                item.iTongGia = item.iSoLuong * item.iGiaSanPham;
//                cart.AddToCart(item, quantity);
//                Session["Cart"] = cart;
//                code = new { Success = true, msg = "Đã thêm sản phẩm vào giỏ hàng!", code = 1, Count = cart.GetTotalQuantity() };
//            }
//            return Json(code);
//        }
//        public ActionResult Update(int id, int quantity)
//        {
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                cart.UpdateQuantity(id, quantity);
//                return Json(new { Success = true });
//            }
//            return Json(new { Success = false });
//        }

//        public ActionResult Delete(int id)
//        {
//            var code = new { Success = false, msg = "", code = -1, Count = 0 };
//            ShoppingCart cart = (ShoppingCart)Session["Cart"];
//            if (cart != null)
//            {
//                var checkProduct = cart.Items.FirstOrDefault(x => x.iMaSanPham == id);
//                if (checkProduct != null)
//                {
//                    cart.Remove(id);
//                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
//                }
//            }
//            return Json(code);
//        }
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                if (db != null)
//                    db.Dispose();
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}