using System;
using gvs_lib_csharp.gvs.business.styles;

namespace gvs_lib_csharp.gvs.business.tree
{
	/// <summary>
	/// Superclass for nodes. Do not use
	/// </summary>
	public interface GVSTreeNode {

		/// <summary>
		/// Returns the label of the node
		/// </summary>
		/// <returns>nodeLabel. If it is null empty string will be set</returns>
		string GetGvsNodeLabel();
		
		/// <summary>
		/// Returns the typ of the node
		/// </summary>
		/// <returns>nodeTyp. If it is null the default typ will be set</returns>
		GVSStyle GetStyle();
	}
}
