using DaiwaRentalGD.Geometries;
using System;
using System.Drawing;

namespace DaiwaRentalGD.Gui.Utilities.Configs
{
    public static class Configs
    {
        public static class Compass
        {
            public static double ArrowWidth = 1.0;

            public static double ArrowLength = 3.0;

            public static double LabelWidth = 1.0;

            public static double LabelHeight = 1.0;

            public static Color YAxisArrowBgColor = Color.Gainsboro;

            public static Color YAxisLabelBgColor = Color.Gainsboro;
            public static double LabelFontSize { get; set; } = 16;

            public static Color TrueNorthArrowBgColor = Color.DimGray;

            public static Color TrueNorthLabelBgColor = Color.DimGray;

            public static Polygon TrueNorthArrow = new Polygon(
                    new[]
                    {
                        new Geometries.Point(0.0, 0.0, 0.0),
                        new Geometries.Point(ArrowWidth / 2.0, ArrowWidth / 4.0, 0.0),
                        new Geometries.Point(ArrowWidth, 0.0, 0.0),
                        new Geometries.Point(ArrowWidth / 2.0, ArrowLength, 0.0)
                    });

            public static Polygon YAxis = new Polygon(
                    new[]
                    {
                        new Geometries.Point(0.0, 0.0, 0.0),
                        new Geometries.Point(ArrowWidth / 2.0, ArrowWidth / 4.0, 0.0),
                        new Geometries.Point(ArrowWidth, 0.0, 0.0),
                        new Geometries.Point(ArrowWidth / 2.0, ArrowLength, 0.0)
                    });

            public static double LabeledArrowDiameter => new MathNet.Numerics.LinearAlgebra.Double.DenseVector(
                  new[]
                  {
                    Math.Max(Configs.Compass.ArrowWidth, LabelWidth),
                    Configs.Compass.ArrowLength + LabelHeight * 2
                  }
              ).L2Norm();
        }

        public static class CarParkingArea
        {
            public static Color CarParkingSpaceTextBgColor = Color.White;

        }

        public static class UnitBuilding
        {
            public static Color LabelTextBgColor = Color.DimGray;
            public static double LabelOffsetY = -2.0;
            public static double LabelOffsetX = 1.5;
        }

        public static class BicycleParkingArea
        {
            public static Color BicycleParkingAreaTextBgColor = Color.SlateGray;
        }


        public static class ScaleBar
        {
            public static float FontSize = 14;
            public static Color ScaleBarColor = Color.Black;
            public static float LabelMarginLeft = 0.8f;
            public static float LabelMarginTop = 0.9f;
        }
    }
}
