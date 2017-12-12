using System;

namespace gvs_lib_csharp.gvs.business.tree
{
	/// <summary>
	/// Summary description for GVSBinaryTreeNode.
	/// </summary>
	public interface GVSBinaryTreeNode:GVSTreeNode {
		
		/// <summary>
		/// Returns the Leftchild from the Treenode
		/// </summary>
		/// <returns>leftChild</returns>
		GVSBinaryTreeNode GetGvsLeftChild();

		/// <summary>
		/// Returns the Rigthchild from the Treenode
		/// </summary>
		/// <returns>rigthchild</returns>
		GVSBinaryTreeNode GetGvsRigthChild();
	}
}
