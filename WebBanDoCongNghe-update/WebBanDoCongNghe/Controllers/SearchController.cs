using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDoCongNghe.Models;

namespace WebBanDoCongNghe.Controllers
{
    public class SearchController : Controller
    {
        DBQuanLyBanDoCongNgheEntities db = new DBQuanLyBanDoCongNgheEntities();

        // Interface định nghĩa hành vi của các chiến lược tìm kiếm
        public interface ISearchStrategy
        {
            List<tb_Product> Search(string keyword, DBQuanLyBanDoCongNgheEntities db);
        }

        // Chiến lược tìm kiếm theo từ khóa
        public class KeywordSearchStrategy : ISearchStrategy
        {
            public List<tb_Product> Search(string keyword, DBQuanLyBanDoCongNgheEntities db)
            {
                return db.tb_Product.Where(n => n.TenSanPham.Contains(keyword)).ToList();
            }
        }

        private ISearchStrategy _searchStrategy;

        public SearchController(ISearchStrategy searchStrategy)
        {
            _searchStrategy = searchStrategy;
        }

        [HttpPost]
        public ActionResult KetQuaTimKiem(FormCollection form)
        {
            string sTuKhoa = form["InputTimKiem"].ToString();
            var listKQTL = _searchStrategy.Search(sTuKhoa, db);
            return View(listKQTL.OrderBy(n => n.TenSanPham).ToList());
        }

        [HttpGet]
        public ActionResult KetQuaTimKiem(string sTuKhoa)
        {
            var listKQTL = _searchStrategy.Search(sTuKhoa, db);
            return View(listKQTL.OrderBy(n => n.TenSanPham).ToList());
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
