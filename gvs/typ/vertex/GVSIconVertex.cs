using System;

namespace gvs_lib_csharp.gvs.typ.vertex
{
	/// <summary>
	/// Represents a vertex which will be drawn with a image.
	/// Linethickness,linecolor,linestyle and icon can be set.
	/// </summary>
	public class GVSIconVertexTyp : GVSVertexTyp{

		public enum Icon{standard,image1,image2,image3,image4,image5,image6,image7,image8,image9 }
	
		private GVSDefaultTyp.LineColor lineColor;
		private GVSDefaultTyp.LineStyle lineStyle;
		private GVSDefaultTyp.LineThickness lineThickness;
		private Icon icon;
	
		public GVSIconVertexTyp(GVSDefaultTyp.LineColor pLineColor, GVSDefaultTyp.LineStyle pLineStyle,
			GVSDefaultTyp.LineThickness pLineThickness, Icon pIcon){
			this.lineColor=pLineColor;
			this.lineStyle=pLineStyle;
			this.lineThickness=pLineThickness;
			this.icon=pIcon;
		}

		/// <summary>
		/// Return the linceolor
		/// </summary>
		/// <returns>linecolor</returns>
		public GVSDefaultTyp.LineColor getLineColor() {
			return lineColor;
		}

		/// <summary>
		/// Return the lincestyle
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
		///	Return the icon
		/// </summary>
		/// <returns>icon</returns>
		public Icon getIcon(){
			return icon;
		}
	
	}
}
