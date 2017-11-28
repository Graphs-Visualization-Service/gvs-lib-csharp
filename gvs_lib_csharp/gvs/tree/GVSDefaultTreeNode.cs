using System;

namespace GVS_Client_Socket_v1._3.gvs.tree
{
	/// <summary>
	/// Represents a DefaultTreeNode.
	/// </summary>
	public interface GVSDefaultTreeNode:GVSTreeNode {

		/// <summary>
		/// Returns all childnodes
		/// </summary>
		/// <returns>childnodes</returns>
		GVSDefaultTreeNode[] getGVSChildNodes();
	}
}
