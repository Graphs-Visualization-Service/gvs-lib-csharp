using System;

namespace GVS_Client_Socket_v1._3.gvs.typ.edge
{
	/// <summary>
	/// Represents a EdgeTyp. Linecolor, linestyle and linethickness can be set
	/// </summary>
	public class GVSEdgeTyp : GVSDefaultTyp{
	
		private LineColor lineColor;
		private LineStyle lineStyle;
		private LineThickness lineThickness;
	
		public GVSEdgeTyp(LineColor pLineColor, 
			LineStyle pLineStyle,
			LineThickness pLineThickness){
			this.lineColor=pLineColor;
			this.lineStyle=pLineStyle;
			this.lineThickness=pLineThickness;
		
		}
	
		/// <summary>
		/// Returns the linecolor
		/// </summary>
		/// <returns>linecolor</returns>
		public GVSDefaultTyp.LineColor getLineColor() {
			return lineColor;
		}

		/// <summary>
		/// Returns the linestyle
		/// </summary>
		/// <returns>linestyle</returns>
		public GVSDefaultTyp.LineStyle getLineStyle() {
			return lineStyle;
		}

		/// <summary>
		/// Returns the linethickness
		/// </summary>
		/// <returns>linethickness</returns>
		public GVSDefaultTyp.LineThickness getLineThickness(){
			return lineThickness;
		}
		
	}
}
