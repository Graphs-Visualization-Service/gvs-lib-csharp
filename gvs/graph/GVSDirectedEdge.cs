using System;

namespace gvs_lib_csharp.gvs.graph
{
	/// <summary>
	/// Represents a DirectedEdge. Start and end must be set. The Arrow
	/// will be drawn at the endvertex.
	/// </summary>
	public interface GVSDirectedEdge : GVSGraphEdge{
	
		/// <summary>
		/// Returns the startevertex
		/// </summary>
		/// <returns>startevertex</returns>
		GVSDefaultVertex GetGvsStartVertex();
		
		/// <summary>
		/// Returns the endvertex
		/// </summary>
		/// <returns>endvertex</returns>
		GVSDefaultVertex GetGvsEndVertex();
	
	}
}
