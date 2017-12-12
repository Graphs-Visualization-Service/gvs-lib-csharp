using System;

namespace gvs_lib_csharp.gvs.business.graph
{
	/// <summary>
	/// Represents a RelativeVertex. 
	/// </summary>
	public interface GVSRelativeVertex : GVSDefaultVertex{
	
		/// <summary>
		/// Returns the xPosition. Values between 0-100 are allowed
		/// </summary>
		/// <returns>Xpos</returns>
		double GetX();

		/// <summary>
		/// Returns the yPosition. Values between 0-100 are allowed
		/// </summary>
		/// <returns>Ypos</returns>
		double GetY();
	}
}
