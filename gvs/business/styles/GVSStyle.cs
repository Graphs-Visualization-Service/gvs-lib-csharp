namespace gvs_lib_csharp.gvs.business.styles
{
    public class GVSStyle
    {
        private readonly GVSColor lineColor;
        private readonly GVSLineStyle lineStyle;
        private readonly GVSLineThickness lineThickness;
        private readonly GVSColor fillColor;
        private readonly GVSIcon? icon;

        public GVSStyle() : this(null, null, null, null, null) { }

        public GVSStyle(GVSColor lineColor, GVSLineStyle lineStyle, GVSLineThickness lineThickness) : 
            this(lineColor, lineStyle, lineThickness, null, null) { }

        public GVSStyle(GVSColor lineColor, GVSLineStyle lineStyle, GVSLineThickness lineThickness, GVSColor fillColor) : 
            this(lineColor, lineStyle, lineThickness, fillColor, null) { }


        public GVSStyle(GVSColor? lineColor, GVSLineStyle? lineStyle, GVSLineThickness? lineThickness, GVSColor? fillColor, GVSIcon? icon)
        {
            this.lineColor = lineColor ?? GVSColor.STANDARD;
            this.lineStyle = lineStyle ?? GVSLineStyle.THROUGH;
            this.lineThickness = lineThickness ?? GVSLineThickness.STANDARD;
            this.fillColor = fillColor ?? GVSColor.STANDARD;
            this.icon = icon;
        }

        public GVSColor GetLineColor() => lineColor;

        public GVSLineStyle GetLineStyle() => this.lineStyle;

        public GVSLineThickness GetLineThickness() => this.lineThickness;

        public GVSIcon? GetIcon() => this.icon;

        public GVSColor GetFillColor() => this.fillColor;
    }
}