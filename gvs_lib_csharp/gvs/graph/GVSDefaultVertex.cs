using System;
using GVS_Client_Socket_v1._3.gvs.typ.vertex;

namespace GVS_Client_Socket_v1._3.gvs.graph
{
	/// <summary>
	/// This interface is needed for the realization of a DefaultVertex
	/// </summary>
	public interface GVSDefaultVertex{
		
		/// <summary>
		///  Returns the label of the vertex
		/// </summary>
		/// <returns>the label. If it is null empty string will be set</returns>
		String getGVSVertexLabel();
		
		/// <summary>
		/// Returns the typ of the vertex
		/// </summary>
		/// <returns>the typ. If it is null the default typ will be set</returns>
		GVSVertexTyp getGVSVertexTyp();
	}
}
