using System;

namespace GVS_Client_Socket_v1._3.gvs.typ.vertex
{
	/// <summary>
	/// Represents a vertex which will be drawn as a Ellipse.
	/// Linethickness,linecolor,linestyle and fillcolor can be set.
	/// </summary>
	public class GVSEllipseVertexTyp : GVSVertexTyp{
	
		public enum FillColor{standard,gray,ligthGray,red,ligthRed,blue,darkBlue,
			ligthBlue,green,ligthGreen,darkGreen,turqoise,yellow,
			brown,orange,pink,violet
		}
	
		private GVSDefaultTyp.LineColor lineColor;
		private GVSDefaultTyp.LineStyle lineStyle;
		private GVSDefaultTyp.LineThickness lineThickness;
		private FillColor fillColor;
	
		public GVSEllipseVertexTyp(GVSDefaultTyp.LineColor pLineColor, GVSDefaultTyp.LineStyle pLineStyle,
			GVSDefaultTyp.LineThickness pLineThickness, FillColor pFillColor){
			this.lineColor=pLineColor;
			this.lineStyle=pLineStyle;
			this.lineThickness=pLineThickness;
			this.fillColor=pFillColor;
		}

		/// <summary>
		/// Return the linecolor
		/// </summary>
		/// <returns>linecolor</returns>
		public GVSDefaultTyp.LineColor getLineColor() {
			return lineColor;
		}

		/// <summary>
		/// Return the linestyle
		/// </summary>
		/// <returns>linestyle</returns>
		public GVSDefaultTyp.LineStyle getLineStyle() {
			return lineStyle;
		}

		/// <summary>
		/// Return the linethickness
		/// </summary>
		/// <returns>linethickness</returns>
		public GVSDefaultTyp.LineThickness getLineThickness() {
			return lineThickness;
		}

		/// <summary>
		/// Return the fillcolor
		/// </summary>
		/// <returns>fillcolor</returns>
		public FillColor getFillColor() {
			return fillColor;
		}

	}
}
