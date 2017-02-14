using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mondol.AutoReview.Asserts;

namespace Mondol.AutoReview
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加AutoReview功能
        /// 此方法要放到所有注册服务代码后调用
        /// </summary>
        public static void AddAutoReview(this IServiceCollection services, params IAssert[] asserts)
        {
            var appCtx = AppContextFactory.Instance;
            appCtx.Services = services;
            appCtx.Asserts = asserts;
        }
    }
}
