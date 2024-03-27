using Google.Apis.Auth;
using System.Web.Mvc;

namespace YourNamespace.Controllers
{
    public class GoogleLoginController : Controller
    {
        public ActionResult LoginGoogle()
        {
            // Redirect đến trang xác thực của Google
            return Redirect("https://accounts.google.com/o/oauth2/auth?client_id=383707341561-b390t7igf9msu4dl8svu9d693iulfvp0.apps.googleusercontent.com&redirect_uri=http://yourwebsite.com/GoogleLogin/Callback&response_type=code&scope=email%20profile&approval_prompt=force&access_type=offline");
        }

        public ActionResult Callback(string code)
        {
            try
            {
                // Gửi mã xác thực nhận được từ Google để trao đổi lấy mã truy cập
                var result = GoogleJsonWebSignature.ValidateAsync(code).Result;

                // Sau khi nhận được kết quả, xử lý việc đăng nhập thành công ở đây
                // Ví dụ: Lấy thông tin người dùng từ result, lưu vào cơ sở dữ liệu, tạo phiên đăng nhập, v.v.

                // Đăng nhập thành công, chuyển hướng người dùng đến trang chính hoặc trang đã đăng nhập
                return RedirectToAction("Index", "Home");
            }
            catch (InvalidJwtException)
            {
                // Xử lý khi mã xác thực không hợp lệ hoặc có lỗi xảy ra
                // Ví dụ: Chuyển hướng người dùng đến trang đăng nhập lại hoặc hiển thị thông báo lỗi
                ViewBag.ErrorMessage = "Đăng nhập không thành công. Vui lòng thử lại!";
                return View("Error");
            }
        }
    }
}
