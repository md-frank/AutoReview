using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Mondol.AutoReview.Asserts;

namespace Mondol.AutoReview
{
    /// <summary>
    /// APP上下文信息
    /// </summary>
    public class AppContext
    {
        private Assembly[] _referencedAssemblies;

        public IServiceCollection Services { get; internal set; }

        public IEnumerable<IAssert> Asserts { get; internal set; }

        /// <summary>
        /// 获取当前项目引用的所有程序集
        /// </summary>
        public Assembly[] ReferencedAssemblies
        {
            get
            {
                if (_referencedAssemblies == null)
                {
                    var asmDict = new Dictionary<string, Assembly>();
                    ScanReferencedAssemblies(Assembly.GetEntryAssembly(), asmDict);
                    _referencedAssemblies = asmDict.Values.ToArray();
                }
                return _referencedAssemblies;
            }
        }

        private void ScanReferencedAssemblies(Assembly asm, Dictionary<string, Assembly> asmDict)
        {
            if (!asmDict.ContainsKey(asm.FullName))
            {
                asmDict[asm.FullName] = asm;

                var asmNames = asm.GetReferencedAssemblies();
                foreach (var asmName in asmNames)
                {
                    var rAsm = Assembly.Load(asmName);
                    ScanReferencedAssemblies(rAsm, asmDict);
                }
            }
        }
    }

    public static class AppContextFactory
    {
        private static AppContext _instance;

        public static AppContext Instance => _instance ?? (_instance = new AppContext());
    }
}
