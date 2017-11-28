using System;
using gvs_lib_csharp.gvs.graph;

namespace gvs_lib_csharp.gvs.graph
{
	/// <summary>
	/// Represents a Undirectededge. The Edge could be drawn with an Arrow
	/// </summary>
	public interface GVSUndirectedEdge : GVSGraphEdge {
										   
		/// <summary>
		/// Returns the connected nodes
		/// </summary>
		/// <returns> the 2 Nodes which are connected</returns>
		GVSDefaultVertex[] GetGvsVertizes();

		/// <summary>
		/// Returns the Position of the Arrow. 1 and 2 are allowed.
		/// Values greater or lower than 1 or 2 takes no effect
		/// </summary>
		/// <returns>the Arrow position</returns>
		int HasArrow();
	}
}
