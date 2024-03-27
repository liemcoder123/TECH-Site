using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using WebBanDoCongNghe.Models;

namespace WebBanDoCongNghe.Controllers
{
    public class ShoppingCartController : Controller
    {
        DBQuanLyBanDoCongNgheEntities db = new DBQuanLyBanDoCongNgheEntities();
        // GET: GioHang
        // GET: ShoppingCart
        public ActionResult IndexShoppingCart()
        {

            return View();
        }
        public ActionResult Partial_Item_Cart()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        public ActionResult MaGiamGia(FormCollection form)
        {   //hiển thị số tiền đã giảm
            var MaGiamGia = form["txtMaGiamGia"].ToString();
            ViewBag.GiamGia = db.tb_DiscountCode.FirstOrDefault(n => n.DiscountCode == MaGiamGia).Value;
            //hiển thị tổng giá tiền sau khi giảm
            decimal soTienGiamGia = (decimal)db.tb_DiscountCode.FirstOrDefault(n => n.DiscountCode == MaGiamGia).Value;
            decimal tienSauKhiGiam = TongTien() - soTienGiamGia;
            ViewBag.tienSauKhiGiam = tienSauKhiGiam;

            tb_DiscountCode discountCode = db.tb_DiscountCode.FirstOrDefault(x => x.DiscountCode.Equals(MaGiamGia));
            Session["discount"] = discountCode;


            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return PartialView(cart.Items);
            }
            return View();
        }
        public decimal TongTien()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            decimal tongTien = 0;
            foreach (var item in cart.Items)
            {
                tongTien = tongTien + item.iTongGia;
            }
            return tongTien;
        }
        public ActionResult GioHangPartial()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return PartialView(cart.Items);
            }
            return PartialView();
        }
        public ActionResult CheckOut()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                ViewBag.CheckCart = cart;
            }
            ViewBag.PhuongThucThanhToan = new SelectList(db.tb_PhuongThucThanhToan.ToList(), "MaPhuongThucThanhToan", "TenPhuongThucThanhToan");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOut(tb_Traveler model)
        {
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    tb_Traveler traveler = new tb_Traveler();

                    foreach (var item in cart.Items)
                    {
                        tb_ChiTietOrder_Traveler chiTietOrder = new tb_ChiTietOrder_Traveler();
                        chiTietOrder.MaDonHang = traveler.MaDonHang;
                        chiTietOrder.MaSanPham = item.iMaSanPham;
                        chiTietOrder.Quantity = item.iSoLuong;
                        chiTietOrder.Price = (decimal)item.iGiaSanPham;
                        db.tb_ChiTietOrder_Traveler.Add(chiTietOrder);
                    }
                    model.TotalPayment = cart.Items.Sum(x => x.iGiaSanPham * x.iSoLuong);
                    model.IsThanhToan = false;
                    model.IsHoanThanh = false;
                    model.IsHuyDon = false;
                    model.CreateDate = DateTime.Now;
                    db.tb_Traveler.Add(model);
                    db.SaveChanges();
                    cart.ClearCart();
                    return RedirectToAction("CheckOutSuccess");
                }

            }
            return View(model);
        }
        public ActionResult CheckOutUser()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                ViewBag.CheckCart = cart;
            }
            ViewBag.PhuongThucThanhToan = new SelectList(db.tb_PhuongThucThanhToan.ToList(), "MaPhuongThucThanhToan", "TenPhuongThucThanhToan");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutUser(tb_Order model)
        {
            if (ModelState.IsValid)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart != null)
                {
                    if (Session["MaKH"] != null)
                    {
                        tb_Customer customer = (tb_Customer)Session["taikhoan"];

                        foreach (var item in cart.Items)
                        {
                            tb_ChiTietOrder chiTietOrder = new tb_ChiTietOrder();
                            chiTietOrder.MaDonHang = model.MaDonHang;
                            chiTietOrder.MaSanPham = item.iMaSanPham;
                            chiTietOrder.Quantity = item.iSoLuong;
                            chiTietOrder.Price = (decimal)item.iGiaSanPham;
                            db.tb_ChiTietOrder.Add(chiTietOrder);
                        }
                        model.MaKH = customer.MaKH;
                        model.TotalPayment = cart.Items.Sum(x => x.iGiaSanPham * x.iSoLuong);
                        model.IsThanhToan = false;
                        model.IsHoanThanh = false;
                        model.IsHuyDon = false;
                        model.CreateDate = DateTime.Now;
                        db.tb_Order.Add(model);
                        db.SaveChanges();
                        cart.ClearCart();
                        return RedirectToAction("CheckOutSuccess");
                    }

                }

            }
            return View(model);
        }
        public ActionResult CheckOutSuccess()
        {
            return View();
        }

        public ActionResult ShowCount()
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                return Json(new { Count = cart.GetTotalQuantity() }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Count = 0 }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult AddToCart(int id, int quantity)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            var checkProduct = db.tb_Product.FirstOrDefault(x => x.MaSanPham == id);
            if (checkProduct != null)
            {
                ShoppingCart cart = (ShoppingCart)Session["Cart"];
                if (cart == null)
                {
                    cart = new ShoppingCart();
                }
                ShoppingCartItem item = new ShoppingCartItem
                {
                    iMaSanPham = checkProduct.MaSanPham,
                    iTenSanPham = checkProduct.TenSanPham,
                    iLink = checkProduct.Link,
                    iSoLuong = quantity

                };
                if (checkProduct.tb_ProductImages.FirstOrDefault(x => x.IsDefault == true) != null)
                {
                    item.iHinhAnh = db.tb_Product.FirstOrDefault(x => x.MaSanPham == id).tb_ProductImages.FirstOrDefault(x => x.IsDefault == true).Image;
                }
                if (checkProduct.IsSale == true)
                {
                    item.iGiaSanPham = (decimal)checkProduct.PriceSale;
                }
                else
                {
                    item.iGiaSanPham = (decimal)checkProduct.Price;
                }

                item.iTongGia = item.iSoLuong * item.iGiaSanPham;
                cart.AddToCart(item, quantity);
                Session["Cart"] = cart;
                code = new { Success = true, msg = "Đã thêm sản phẩm vào giỏ hàng!", code = 1, Count = cart.GetTotalQuantity() };
            }
            return Json(code);
        }
        public ActionResult Update(int id, int quantity)
        {
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                cart.UpdateQuantity(id, quantity);
                return Json(new { Success = true });
            }
            return Json(new { Success = false });
        }
        public ActionResult FailureView()
        {
            return View();
        }

        public ActionResult Delete(int id)
        {
            var code = new { Success = false, msg = "", code = -1, Count = 0 };
            ShoppingCart cart = (ShoppingCart)Session["Cart"];
            if (cart != null)
            {
                var checkProduct = cart.Items.FirstOrDefault(x => x.iMaSanPham == id);
                if (checkProduct != null)
                {
                    cart.Remove(id);
                    code = new { Success = true, msg = "", code = 1, Count = cart.Items.Count };
                }
            }
            return Json(code);
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
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            try
            {
                // Lấy apiContext
                APIContext apiContext = PaypalConfiguration.GetAPIContext();

                // Kiểm tra xem PayerID có null hoặc trống không
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    // Thực hiện phần này nếu PayerID không tồn tại
                    // Tạo thanh toán
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/shoppingcart/PaymentWithPayPal?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);

                    // Lấy URL phê duyệt từ thanh toán đã tạo
                    var approvalUrl = createdPayment.links.FirstOrDefault(x => x.rel.ToLower().Trim().Equals("approval_url"))?.href;

                    // Lưu paymentID vào session
                    Session.Add(guid, createdPayment.id);

                    // Chuyển hướng đến URL phê duyệt của PayPal
                    return Redirect(approvalUrl);
                }
                else
                {
                    // Phần này được thực hiện sau khi nhận được tất cả các tham số cho thanh toán
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);

                    // Kiểm tra xem thanh toán đã được phê duyệt chưa
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        // Nếu thanh toán không được phê duyệt, hiển thị view thất bại
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và hiển thị view thất bại
                return View("FailureView");
            }

            // Trên thanh toán thành công, hiển thị view thành công
            if (Session["Cart"] != null)
            {
                // Ép kiểu Session["Cart"] về kiểu ShoppingCart
                ShoppingCart cart = (ShoppingCart)Session["Cart"];

                // Gọi phương thức ClearCart() trên đối tượng cart
                cart.ClearCart();
            }
            return View("CheckOutSuccess");
           
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };

            this.payment = new Payment()
            {
                id = paymentId
            };

            return this.payment.Execute(apiContext, paymentExecution);
        }

            private Payment CreatePayment(APIContext apiContext, string redirectUrl)
            {

                var listSanPham = Session["Cart"] as ShoppingCart;

                //create itemlist and add item objects to it  
                var itemList = new ItemList()
                {
                    items = new List<Item>()
                };

                if (listSanPham != null && listSanPham.Items.Any())
                {
                    foreach (var item in listSanPham.Items)
                    {
                        itemList.items.Add(new Item()
                        {
                            name = item.iTenSanPham,
                            currency = "USD",
                            price = Math.Round(item.iGiaSanPham/23500,2).ToString(),
                            quantity = item.iSoLuong.ToString(),
                            sku = item.iMaSanPham.ToString(),
                        });
                    }
                }

                var payer = new Payer()
                {
                    payment_method = "paypal"
                };

                // Configure Redirect Urls here with RedirectUrls object  
                var redirUrls = new RedirectUrls()
                {
                    cancel_url = redirectUrl + "&Cancel=true",
                    return_url = redirectUrl
                };

                // Adding Tax, shipping and Subtotal details  
                var details = new Details()
                {
                    tax = "1",
                    shipping = "1",
                    subtotal = listSanPham.GetTotalUSD().ToString()

                };

                //Final amount with details  
                var amount = new Amount()
                {
                    currency = "USD",
                    total = (listSanPham.GetTotalUSD() + 1 + 1).ToString(), // Tổng cộng: subtotal + tax + shipping
                    details = details
                };

                var transactionList = new List<Transaction>();

                // Adding description about the transaction  
                var paypalOrderId = DateTime.Now.Ticks;
                transactionList.Add(new Transaction()
                {
                    description = $"Invoice #{paypalOrderId}",
                    invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                    amount = amount,
                    item_list = itemList
                });

                this.payment = new Payment()
                {
                    intent = "sale",
                    payer = payer,
                    transactions = transactionList,
                    redirect_urls = redirUrls
                };

                // Create a payment using a APIContext  
                return this.payment.Create(apiContext);
            }

        }
    }
