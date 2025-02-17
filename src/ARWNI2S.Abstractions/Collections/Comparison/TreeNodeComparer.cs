﻿using ARWNI2S.Collections.Tree;
using System.Collections;

namespace ARWNI2S.Collections.Comparison
{
    internal class TreeNodeComparer<T> : IComparer
    {
        private IComparer<T> _valueCompare;

        public TreeNodeComparer(IComparer<T> comparer)
        {
            _valueCompare = comparer;
        }

        public int Compare(object x, object y)
        {
            TreeNode<T> xData = (TreeNode<T>)x;
            TreeNode<T> yData = (TreeNode<T>)y;

            return _valueCompare.Compare(xData.Value, yData.Value);
        }
    }
}
