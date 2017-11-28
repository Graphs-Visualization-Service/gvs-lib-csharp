using System;
using gvs_lib_csharp.gvs.styles;

namespace gvs_lib_csharp.gvs.graph
{
	/// <summary>
	/// Super class for Edges. Do not use
	/// </summary>
	public interface GVSGraphEdge {

		/// <summary>
		/// Returns the edgelabel. 
		/// </summary>
		/// <returns>the edgelabel. If it is null the label will be set to empty</returns>
		string GetGvsEdgeLabel();

		/// <summary>
		/// Returns the edgetyp. 
		/// </summary>
		/// <returns>the edgetyp. If it is null the default typ will be set</returns>
		GVSStyle GetStyle();
	}
}
