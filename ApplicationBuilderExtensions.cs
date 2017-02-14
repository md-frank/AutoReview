using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace Mondol.AutoReview
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// 启用AutoReview功能
        /// </summary>
        public static void UseAutoReview(this IApplicationBuilder app)
        {
            var appCtx = AppContextFactory.Instance;
            foreach (var assert in appCtx.Asserts)
            {
                assert.Assert(appCtx);
            }
        }
    }
}
