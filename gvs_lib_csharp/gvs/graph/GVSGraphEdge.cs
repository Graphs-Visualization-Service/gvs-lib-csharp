using System;
using GVS_Client_Socket_v1._3.gvs.typ.edge;

namespace GVS_Client_Socket_v1._3.gvs.graph
{
	/// <summary>
	/// Super class for Edges. Do not use
	/// </summary>
	public interface GVSGraphEdge {

		/// <summary>
		/// Returns the edgelabel. 
		/// </summary>
		/// <returns>the edgelabel. If it is null the label will be set to empty</returns>
		String getGVSEdgeLabel();

		/// <summary>
		/// Returns the edgetyp. 
		/// </summary>
		/// <returns>the edgetyp. If it is null the default typ will be set</returns>
		GVSEdgeTyp getGVSEdgeTyp();
	}
}
