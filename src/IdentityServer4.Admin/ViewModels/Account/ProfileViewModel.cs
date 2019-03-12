using System.ComponentModel.DataAnnotations;
using IdentityServer4.Admin.Infrastructure;

namespace IdentityServer4.Admin.ViewModels.Account
{
    public class ProfileViewModel
    {
        /// <summary>
        /// 邮件
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [Display(Name = "PhoneNumber", Description = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// 用户拥有的角色
        /// </summary>
        public string Roles { get; set; }

        /// <summary>
        /// 姓
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// 名
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 标语
        /// </summary>
        public string Slogan { get; set; }

        /// <summary>
        /// 所在地
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// 个人网址
        /// </summary>
        public string WebSite { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string OfficePhone { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public Sex Sex { get; set; }
    }
}