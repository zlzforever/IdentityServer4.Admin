using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure;

namespace IdentityServer4.Admin.ViewModels.User
{
    public class CreateUserViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        [StringLength(32)]
        [MinLength(4)]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        /// <summary>
        /// 邮件
        /// </summary>
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(24, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "新密码")]
        public string Password { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [StringLength(50)]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        [StringLength(50)]
        public string FirstName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        [StringLength(50)]
        public string LastName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(100)]
        public string Title { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [StringLength(24)]
        public string NickName { get; set; }

        /// <summary>
        /// 标语
        /// </summary>
        [StringLength(100)]
        public string Slogan { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        [StringLength(256)]
        public string Location { get; set; }

        /// <summary>
        /// 个人网址
        /// </summary>
        [StringLength(500)]
        [Url]
        public string WebSite { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        [StringLength(50)]
        public string OfficePhone { get; set; }
    }
}