using System;
using GVS_Client_Socket_v1._3.gvs.typ.vertex;

namespace GVS_Client_Socket_v1._3.gvs.typ.node
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
