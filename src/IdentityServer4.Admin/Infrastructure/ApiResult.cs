using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4.Admin.Infrastructure
{
    public class ApiResult : JsonResult
    {
        public static ApiResult Ok = new ApiResult();

        public ApiResult(object data = null) : base(new
        {
            Code = (int) ApiResultType.Success,
            Msg = "success",
            Data = data
        })
        {
        }

        public ApiResult(ApiResultType type, string msg = null, object data = null) : base(new
        {
            Code = type,
            Msg = msg,
            Data = data
        })
        {
        }
    }

    /// <summary>
    /// 表示 ajax 操作结果类型的枚举
    /// </summary>
    public enum ApiResultType
    {
        /// <summary>
        /// 消息结果类型
        /// </summary>
        Info = 203,

        /// <summary>
        /// 成功结果类型
        /// </summary>
        Success = 200,

        /// <summary>
        /// 异常结果类型
        /// </summary>
        Error = 500,

        /// <summary>
        /// 用户未登录
        /// </summary>
        UnAuth = 401,

        /// <summary>
        /// 已登录，但权限不足
        /// </summary>
        Forbidden = 403,

        /// <summary>
        /// 资源未找到
        /// </summary>
        NoFound = 404,

        /// <summary>
        /// 资源被锁定
        /// </summary>
        Locked = 423
    }
}