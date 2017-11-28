using System;
using gvs_lib_csharp.gvs.typ.vertex;

namespace gvs_lib_csharp.gvs.typ.node
{
	/// <summary>
	/// Represents a node which will be drawn as a Ellipse.
	/// Linethickness,linecolor,linestyle and fillcolor can be set.
	/// </summary>
	public class GVSNodeTyp : GVSEllipseVertexTyp{

		public GVSNodeTyp(GVSDefaultTyp.LineColor pLineColor, GVSDefaultTyp.LineStyle pLineStyle,
			GVSDefaultTyp.LineThickness pLineThickness, 
			FillColor pFillColor):base(pLineColor, pLineStyle, 
			pLineThickness, pFillColor){	
		}
	}
}
