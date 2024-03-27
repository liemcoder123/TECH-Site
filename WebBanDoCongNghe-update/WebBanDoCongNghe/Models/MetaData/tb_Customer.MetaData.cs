using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebBanDoCongNghe.Models
{
    [MetadataTypeAttribute(typeof(tb_CustomerMetaData))]
    public partial class tb_Customer
    {
        internal sealed class tb_CustomerMetaData
        {
            public int MaKH { get; set; }
            [StringLength(200, ErrorMessage = "Không nhập quá 200 ký tự")]
            public string HoTen { get; set; }
            [StringLength(100, ErrorMessage = "Không nhập quá 100 ký tự")]
            public string TaiKhoan { get; set; }
            [StringLength(100, ErrorMessage = "Không nhập quá 100 ký tự")]
            public string MatKhau { get; set; }
            [StringLength(100, ErrorMessage = "Không nhập quá 100 ký tự")]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            public string ImageUser { get; set; }
            [StringLength(20, ErrorMessage = "Không nhập quá 20 ký tự")]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }
            [StringLength(500, ErrorMessage = "Không nhập quá 500 ký tự")]
            public string Address { get; set; }
            public string GioiTinh { get; set; }
            [DataType(DataType.Date)]
            public Nullable<System.DateTime> NgaySinh { get; set; }
            public Nullable<bool> IsActive { get; set; }
            public string CreatedBy { get; set; }
            public Nullable<System.DateTime> CreateDate { get; set; }
            public Nullable<System.DateTime> UpdatedDate { get; set; }
            public string UpdatedBy { get; set; }
            public Nullable<bool> IsAdmin { get; set; }
        }
    }
}