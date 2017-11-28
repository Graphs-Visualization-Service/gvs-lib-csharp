using System;

namespace GVS_Client_Socket_v1._3.gvs.tree
{
	/// <summary>
	/// Summary description for GVSBinaryTreeNode.
	/// </summary>
	public interface GVSBinaryTreeNode:GVSTreeNode {
		
		/// <summary>
		/// Returns the Leftchild from the Treenode
		/// </summary>
		/// <returns>leftChild</returns>
		GVSBinaryTreeNode getGVSLeftChild();

		/// <summary>
		/// Returns the Rigthchild from the Treenode
		/// </summary>
		/// <returns>rigthchild</returns>
		GVSBinaryTreeNode getGVSRigthChild();
	}
}
