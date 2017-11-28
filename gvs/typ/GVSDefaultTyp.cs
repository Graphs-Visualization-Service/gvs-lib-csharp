using System;

namespace gvs_lib_csharp.gvs.typ
{
	/// <summary>
	/// Represents the general typdefinition for all vertex and nodes. That includes
	/// LineColor,LineStyle and LineThickness
	/// </summary>
	public class GVSDefaultTyp {
		public enum LineColor {
			standard,black,gray,ligthGray,red,ligthRed,blue,darkBlue,
			ligthBlue,green,ligthGreen,darkGreen,turqoise,yellow,brown,
			orange,pink,violet
		}
		public enum LineStyle{
			standard,dashed,dotted,through
		}
		public enum LineThickness{
			standard,slight,bold,fat
		}
	}
}
