using System;

namespace gvs_lib_csharp.gvs.tree
{
	/// <summary>
	/// Represents a DefaultTreeNode.
	/// </summary>
	public interface GVSDefaultTreeNode:GVSTreeNode {

		/// <summary>
		/// Returns all childnodes
		/// </summary>
		/// <returns>childnodes</returns>
		GVSDefaultTreeNode[] GetGvsChildNodes();
	}
}
