using System;
using GVS_Client_Socket_v1._3.gvs.typ.node;

namespace GVS_Client_Socket_v1._3.gvs.tree
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
