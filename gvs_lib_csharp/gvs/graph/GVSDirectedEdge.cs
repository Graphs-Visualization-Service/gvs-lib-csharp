using System;

namespace GVS_Client_Socket_v1._3.gvs.graph
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
		GVSDefaultVertex getGVSStartVertex();
		
		/// <summary>
		/// Returns the endvertex
		/// </summary>
		/// <returns>endvertex</returns>
		GVSDefaultVertex getGVSEndVertex();
	
	}
}
