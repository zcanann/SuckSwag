namespace SuckSwag.Source.Utils.Extensions
{
    using System;
    using System.Windows.Media;

    static class ColorExtensions
    {
        public static Color ToWpfColor(this System.Drawing.Color color)
        {
            Byte AVal = color.A;
            Byte RVal = color.R;
            Byte GVal = color.G;
            Byte BVal = color.B;

            return Color.FromArgb(AVal, RVal, GVal, BVal);
        }
    }
    //// End class
}
//// End namespace