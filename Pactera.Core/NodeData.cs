using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
    /// <summary>
    /// 树节点
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class NodeData<TData>
    {
        /// <summary>
        /// 节点数据
        /// </summary>
        public TData Data { get; internal set; }
        /// <summary>
        /// 子节点列表
        /// </summary>
        public List<NodeData<TData>> Children { get; internal set; }

        public NodeData()
        {
            Children = new List<NodeData<TData>>();
        }

    }

}