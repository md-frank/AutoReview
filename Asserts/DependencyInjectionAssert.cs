using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Mondol.AutoReview.Asserts
{
    /// <summary>
    /// 依赖注入断言
    /// </summary>
    public class DependencyInjectionAssert : IAssert
    {
        /// <summary>
        /// 要忽略的类型FullName
        /// </summary>
        public string[] IgnoreTypes { get; set; }

        public void Assert(AppContext appContext)
        {
            var internalIgnoreTypes = new[]
            {
                "Microsoft.AspNetCore.Mvc.Internal.MvcRouteHandler",
                "Microsoft.AspNetCore.Mvc.Internal.MvcAttributeRouteHandler",
                "Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperDescriptorResolver"
            };
            var finalIgnoreTypes = internalIgnoreTypes;
            if (IgnoreTypes != null)
                finalIgnoreTypes = internalIgnoreTypes.Concat(IgnoreTypes).ToArray();

            var services = appContext.Services;
            foreach (var svce in services)
            {
                var svceType = svce.ImplementationType;
                if (svceType == null)
                    continue;

                if (finalIgnoreTypes.Contains(svceType.FullName))
                    continue;

                var ctors = svceType.GetConstructors();
                foreach (var ctor in ctors)
                {
                    var paramLst = ctor.GetParameters();
                    foreach (var param in paramLst)
                    {
                        var paramType = param.ParameterType;
                        var paramTypeInfo = paramType.GetTypeInfo();
                        if (paramTypeInfo.IsGenericType)
                        {
                            if (paramType.ToString().StartsWith("System.Collections.Generic.IEnumerable`1"))
                            {
                                paramType = paramTypeInfo.GetGenericArguments().First();
                                paramTypeInfo = paramType.GetTypeInfo();
                            }
                        }
                        if (paramType == typeof(IServiceProvider))
                            continue;

                        ServiceDescriptor pSvce;
                        if (paramTypeInfo.IsGenericType)
                        {
                            //泛型采用模糊识别，可能有遗漏
                            var prefix = Regex.Match(paramType.ToString(), @"^[^`]+`\d+\[").Value;
                            pSvce = services.FirstOrDefault(p => p.ServiceType.ToString().StartsWith(prefix));
                        }
                        else
                        {
                            pSvce = services.FirstOrDefault(p => p.ServiceType == paramType);
                        }
                        if (pSvce == null)
                            throw new InvalidProgramException($"服务 {svceType.FullName} 的构造方法引用了未注册的服务 {paramType.FullName}");

                        //确保Singleton的服务不能依赖Scoped的服务
                        if (svce.Lifetime == ServiceLifetime.Singleton && pSvce.Lifetime == ServiceLifetime.Scoped)
                            throw new InvalidProgramException($"Singleton的服务 {svceType.FullName} 的构造方法引用了Scoped的服务 {paramType.FullName}");
                    }
                }
            }
        }
    }
}
