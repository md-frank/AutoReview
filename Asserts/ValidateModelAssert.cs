using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mondol.AutoReview.Utils;

namespace Mondol.AutoReview.Asserts
{
    /// <summary>
    /// 断言控制器里的Action，如果参数类型为Model则必需具有ValidateModel属性标记
    /// </summary>
    public class ValidateModelAssert : IAssert
    {
        public Type ValidateModelAttributeType { get; set; }

        public void Assert(AppContext appContext)
        {
            if (ValidateModelAttributeType == null)
                throw new ArgumentNullException(nameof(ValidateModelAttributeType));

            var controllerTypes = ControllerUtil.GetControllers(appContext);

            foreach (var controllerType in controllerTypes)
            {
                var actions = ControllerUtil.GetControllerActions(appContext, controllerType);
                foreach (var action in actions)
                {
                    var hasModel = action.GetParameters().Any(p =>
                    {
                        var tInfo = p.ParameterType.GetTypeInfo();
                        return p.ParameterType != typeof(string) && tInfo.IsClass;
                    });
                    if (hasModel && action.GetCustomAttribute(ValidateModelAttributeType) == null)
                        throw new InvalidProgramException($"Action {controllerType.FullName}.{action.Name} 包含Model类型参数，但未指定ValidateModel属性");
                }
            }
        }
    }
}
