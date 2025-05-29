using Mapsui;
using Mapsui.Extensions;
using Mapsui.Styles;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;
using System.Runtime.CompilerServices;

namespace AvaloniaMapsuiLib
{
    public class MouseMoveCoordinatesWidget : MouseCoordinatesWidget
    {
       
        public override void OnPointerMoved(WidgetEventArgs e)
        {
            var worldPosition = e.Map.Navigator.Viewport.ScreenToWorld(e.ScreenPosition);
            // update the Mouse position
         //   Text = $"{worldPosition.X:F0}, {worldPosition.Y:F0}";
            var converter = new CGCS2000Converter(117);

            // 输入XY坐标（单位：米）
            double[] latLon = converter.XYToLatLon(35599811.05246, 3367716.28704);

            Console.WriteLine($"纬度: {latLon[0]:F8}°, 经度: {latLon[1]:F8}°");

              Text = $"lat: {latLon[0]:F8}°, lon: {latLon[1]:F8}°";
            
           // Text = "维度";
            
        }

    }

    public class CGCS2000Converter
    {
        // CGCS2000椭球参数
        private const double a = 6378137.0;      // 长半轴
        private const double f = 1 / 298.257222101; // 扁率倒数
        private const double e2 = 2 * f - f * f; // 第一偏心率平方
        private const double k0 = 1.0;           // 高斯投影比例因子

        // 中央子午线经度（需根据带号设定，如3度带L0=3*带号）
        private readonly double L0;

        public CGCS2000Converter(double centralMeridian)
        {
            L0 = centralMeridian;
        }
        public double[] XYToLatLon(double X, double Y)
        {
            Y -= 500000; // 去除东偏移量（适用于3度带）
            X = X / k0;  // 比例因子校正

            // 迭代计算底点纬度
            double Bf = X / (a * (1 - e2 / 4 - 3 * e2 * e2 / 64)); // 初始近似值
            for (int i = 0; i < 5; i++) // 迭代5次精度足够
            {
                double sinB = Math.Sin(Bf);
                double N = a / Math.Sqrt(1 - e2 * sinB * sinB);

                double deltaB = (X - CalculateMeridianArc(Bf)) / (N * Math.Cos(Bf));
                Bf += deltaB;
                if (Math.Abs(deltaB) < 1e-10) break;
            }

            // 计算经度差
            double Nf = a / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(Bf), 2));
            double tf = Math.Tan(Bf);
            double eta2 = e2 * Math.Pow(Math.Cos(Bf), 2);
            double l = (Y / (Nf * Math.Cos(Bf))) * (1 - (Y * Y) / (6 * Nf * Nf) * (1 + 2 * tf * tf + eta2));

            // 转换为经纬度
            double latitude = Bf * 180 / Math.PI;
            double longitude = L0 + l * 180 / Math.PI;
            return new double[] { latitude, longitude };
        }

        private double CalculateMeridianArc(double B)
        {
            // 子午线弧长公式简化版
            return a * ((1 - e2 / 4 - 3 * e2 * e2 / 64) * B
                      - (3 * e2 / 8 + 3 * e2 * e2 / 32) * Math.Sin(2 * B)
                      + (15 * e2 * e2 / 256) * Math.Sin(4 * B));
        }

    }

}
