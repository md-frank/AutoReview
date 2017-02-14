using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Mondol.AutoReview.Utils
{
    public static class ControllerUtil
    {
        public static List<Type> GetControllers(AppContext appCtx)
        {
            var refAsms = appCtx.ReferencedAssemblies;
            var controllerType = GeTypeByFullName(refAsms, "Microsoft.AspNetCore.Mvc.Controller");
            var controllerAttrType = GeTypeByFullName(refAsms, "Microsoft.AspNetCore.Mvc.ControllerAttribute");
            var noControllerAttrType = GeTypeByFullName(refAsms, "Microsoft.AspNetCore.Mvc.NonControllerAttribute");

            var lstRetn = new List<Type>();
            foreach (var asm in refAsms)
            {
                var types = asm.GetTypes();
                foreach (var type in types)
                {
                    var tInfo = type.GetTypeInfo();
                    if (!tInfo.IsClass)
                        continue;
                    if (tInfo.GetCustomAttribute(noControllerAttrType) != null)
                        continue;
                    if (!(type.Name.EndsWith("Controller") && type.Name.Contains(".Controllers.")) &&
                        !controllerType.IsAssignableFrom(type) &&
                        tInfo.GetCustomAttribute(controllerAttrType) == null)
                        continue;

                    lstRetn.Add(type);
                }
            }
            return lstRetn;
        }

        public static List<MethodInfo> GetControllerActions(AppContext appCtx, Type type)
        {
            var refAsms = appCtx.ReferencedAssemblies;
            var noActionAttrType = GeTypeByFullName(refAsms, "Microsoft.AspNetCore.Mvc.NonActionAttribute");

            var lstRetn = new List<MethodInfo>();
            var tInfo = type.GetTypeInfo();
            var actions = tInfo.GetMethods();
            foreach (var action in actions)
            {
                if (action.Name.StartsWith("get_") || action.Name.StartsWith("set_"))
                    continue;
                if (action.GetCustomAttribute(noActionAttrType) != null ||
                    action.DeclaringType == typeof(object))
                    continue;
                lstRetn.Add(action);
            }
            return lstRetn;
        }

        private static Type GeTypeByFullName(Assembly[] refAsms, string fullName)
        {
            foreach (var asm in refAsms)
            {
                var type = asm.GetType(fullName);
                if (type != null)
                    return type;
            }
            throw new InvalidProgramException($"无法找到类型 {fullName}");
        }
    }
}
