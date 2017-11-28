using System;

namespace GVS_Client_Socket_v1._3.gvs.typ
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
