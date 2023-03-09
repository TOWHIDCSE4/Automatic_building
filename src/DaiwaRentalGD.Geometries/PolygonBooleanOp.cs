using System;
using System.Collections.Generic;
using System.Linq;
using ClipperLib;

namespace DaiwaRentalGD.Geometries
{
    /// <summary>
    /// Performs polygon boolean operations.
    /// </summary>
    /// 
    /// <remarks>
    /// Logic for polygon boolean operations is provided by Clipper library
    /// by Angus Johnson, subject to Boost Software License Ver 1.
    /// </remarks>
    public class PolygonBooleanOp
    {
        #region Constructors

        public PolygonBooleanOp()
        { }

        #endregion

        #region Methods

        public IReadOnlyList<Polygon> Union(
            IEnumerable<Polygon> polygons
        ) => Execute(
            polygons, Enumerable.Empty<Polygon>(),
            ClipType.ctUnion,
            PolyFillType.pftNonZero, PolyFillType.pftNonZero,
            Precision
        );

        public IReadOnlyList<Polygon> Intersect(
            IEnumerable<Polygon> subjects,
            IEnumerable<Polygon> clips
        ) => Execute(
            subjects, clips,
            ClipType.ctIntersection,
            PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd,
            Precision
        );

        public IReadOnlyList<Polygon> Difference(
            IEnumerable<Polygon> subjects,
            IEnumerable<Polygon> clips
        ) => Execute(
            subjects, clips,
            ClipType.ctDifference,
            PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd,
            Precision
        );

        public IReadOnlyList<Polygon> Xor(
            IEnumerable<Polygon> subjects,
            IEnumerable<Polygon> clips
        ) => Execute(
            subjects, clips,
            ClipType.ctXor,
            PolyFillType.pftEvenOdd, PolyFillType.pftEvenOdd,
            Precision
        );

        internal static IntPoint ToClipperPoint(
            Point point, double precision = DefaultPrecision
        ) => new IntPoint(point.X / precision, point.Y / precision);

        internal static Point ToPoint(
            IntPoint clipperPoint, double precision = DefaultPrecision
        ) => new Point(
            clipperPoint.X * precision, clipperPoint.Y * precision, 0.0
        );

        internal static List<IntPoint> ToClipperPath(
            Polygon polygon, double precision = DefaultPrecision
        )
        {
            var clipperPath = polygon.Points.Select(
                point => ToClipperPoint(point, precision)
            ).ToList();

            return clipperPath;
        }

        internal static Polygon ToPolygon(
            List<IntPoint> clipperPath,
            double precision = DefaultPrecision
        )
        {
            var points = clipperPath.Select(
                clipperPoint => ToPoint(clipperPoint, precision)
            ).ToList();

            var polygon = new Polygon(points);

            return polygon;
        }

        internal static IReadOnlyList<Polygon> Execute(
            IEnumerable<Polygon> subjects,
            IEnumerable<Polygon> clips,
            ClipType clipType,
            PolyFillType subjectFillType, PolyFillType clipFillType,
            double precision = DefaultPrecision
        )
        {
            if (subjects is null)
            {
                throw new ArgumentNullException(nameof(subjects));
            }

            if (clips is null)
            {
                throw new ArgumentNullException(nameof(clips));
            }

            var clipperSubjects =
                subjects.Select(subject => ToClipperPath(subject, precision))
                .ToList();

            var clipperClips =
                clips.Select(clip => ToClipperPath(clip, precision))
                .ToList();

            var clipper = new Clipper();

            clipper.AddPaths(clipperSubjects, PolyType.ptSubject, true);
            clipper.AddPaths(clipperClips, PolyType.ptClip, true);

            var clipperResults = new List<List<IntPoint>>();

            clipper.Execute(
                clipType, clipperResults,
                subjectFillType, clipFillType
            );

            var results = clipperResults.Select(
                clipperResult => ToPolygon(clipperResult, precision)
            ).ToList();

            return results;
        }

        #endregion

        #region Properties

        public static PolygonBooleanOp Default { get; } =
            new PolygonBooleanOp();

        public double Precision
        {
            get => _precision;
            set => _precision = value > 0.0 ? value :
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"{nameof(Precision)} must be positive"
                );
        }

        #endregion

        #region Fields

        private double _precision = DefaultPrecision;

        #endregion

        #region Constants

        public const double DefaultPrecision = 1e-6;

        #endregion
    }
}
