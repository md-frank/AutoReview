using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mondol.AutoReview
{
    /// <summary>
    /// 指定在CodeReview时要忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class CodeReviewIgnore : Attribute
    {
    }
}
