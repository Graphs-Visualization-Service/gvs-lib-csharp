using System;
using gvs_lib_csharp.gvs.typ.vertex;

namespace gvs_lib_csharp.gvs.graph
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
