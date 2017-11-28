using System;

namespace gvs_lib_csharp.gvs.typ.graph
{
	/// <summary>
	/// Represents a Graphtyp. A backgroundimage can be set.
	/// </summary>
	public class GVSGraphTyp {
		public enum Background{
			standard,background1,background2,backgrond3,background4,
			background5,background6,background7,background8,background9}
		private Background background;
	
		public GVSGraphTyp(Background pBackground){
			this.background=pBackground;
		}

		/// <summary>
		/// Returns the Background. 
		/// </summary>
		/// <returns>Background</returns>
		public Background GetBackground() {
			return background;
		}
	
	}
}
