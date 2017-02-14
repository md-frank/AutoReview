using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mondol.AutoReview.Asserts
{
    /// <summary>
    /// 断言
    /// </summary>
    public interface IAssert
    {
        void Assert(AppContext context);
    }
}
