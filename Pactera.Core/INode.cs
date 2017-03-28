using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
    /// <summary>
    /// 树节点接口定义
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface INode<TKey> where TKey : struct
    {
        /// <summary>
        /// 节点号码
        /// </summary>
        TKey StepNumber { get; }
        /// <summary>
        /// 父节点Id
        /// </summary>
        TKey? ParentNumber { get; }
    }
}
