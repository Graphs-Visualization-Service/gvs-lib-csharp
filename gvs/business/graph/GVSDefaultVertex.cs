using gvs_lib_csharp.gvs.business.styles;

namespace gvs_lib_csharp.gvs.business.graph
{
	/// <summary>
	/// This interface is needed for the realization of a DefaultVertex
	/// </summary>
	public interface GVSDefaultVertex{
		
		/// <summary>
		///  Returns the label of the vertex
		/// </summary>
		/// <returns>the label. If it is null empty string will be set</returns>
		string GetGvsVertexLabel();
		
		/// <summary>
		/// Returns the typ of the vertex
		/// </summary>
		/// <returns>the typ. If it is null the default typ will be set</returns>
		GVSStyle GetStyle();
	}
}
