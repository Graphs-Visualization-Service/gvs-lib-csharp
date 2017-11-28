using System;
using gvs_lib_csharp.gvs.typ.node;

namespace gvs_lib_csharp.gvs.tree
{
	/// <summary>
	/// Superclass for nodes. Do not use
	/// </summary>
	public interface GVSTreeNode {

		/// <summary>
		/// Returns the label of the node
		/// </summary>
		/// <returns>nodeLabel. If it is null empty string will be set</returns>
		String getGVSNodeLabel();
		
		/// <summary>
		/// Returns the typ of the node
		/// </summary>
		/// <returns>nodeTyp. If it is null the default typ will be set</returns>
		GVSNodeTyp getGVSNodeTyp();
	}
}