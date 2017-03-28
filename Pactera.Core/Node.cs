using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pactera.Core
{
    /// <summary>
    /// 树节点
    /// </summary>
    public static class Node
    {
        /// <summary>
        /// 根据根节点和节点数组构造节点树
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="nodes">节点数组</param>
        /// <param name="root">根节点</param>
        /// <returns></returns>
        public static NodeData<TData> PopulateTree<TData, TKey>(TData[] nodes, TData root)
            where TData : INode<TKey>
            where TKey : struct
        {
            if (nodes == null
                || nodes.Length == 0
                || root == null)
                return null;

            var tree = new NodeData<TData>();
            tree.Data = root;

            Action<NodeData<TData>> BuildSubNode = null;
            BuildSubNode = parentNode =>
            {
                foreach (var c in nodes.Where(p => p.ParentNumber.HasValue).Where(p => object.Equals(p.ParentNumber.Value, parentNode.Data.ParentNumber)))
                {
                    var node = new NodeData<TData> { Data = c };
                    parentNode.Children.Add(node);
                    BuildSubNode(node);
                }
            };

            BuildSubNode(tree);
            return tree;
        }

    }
}
