using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACAD.Apparel.Notches.Tests.Drawings
{
    internal static class KnownValues
    {
        public static readonly TestDrawingKnownValues Notches1 = new TestDrawingKnownValues("Notches1.dwg")
        {
            SourceCurveHandle = new Handle(0x73b),

            SourceNotchLines =
            {
                new Handle(0x727),
                new Handle(0x71a),
                new Handle(0x715),
                new Handle(0x87e),
            },

            SourceSegmentLengths =
            {
                10.4862
            },

            TargetCurveHandle = new Handle(0x91d),

            TargetFacetPercentages =
            {
                35,
                25,
                5,
                8,
                27
            }
        };
    }

    public class TestDrawingKnownValues
    {
        public TestDrawingKnownValues(string drawingFilename)
        {
            DrawingPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Drawings", drawingFilename);
        }

        public string DrawingPath { get; set; }

        public string GenerateSnapshotDrawingPath()
        {
            var snapshotTimestamp = System.DateTime.Now.ToString("u").Replace(':', '-');
            var snapshotFilename = Path.GetFileNameWithoutExtension(DrawingPath) + " " + snapshotTimestamp;

            return Path.Combine(
                Path.GetDirectoryName(DrawingPath),
                Path.ChangeExtension(snapshotFilename, "dwg"));
        }

        public Handle SourceCurveHandle { get; set; }

        public List<Handle> SourceNotchLines { get; } = new List<Handle>();

        public List<double> SourceSegmentLengths = new List<double>();

        public List<double> TargetFacetPercentages { get; } = new List<double>();


        public Handle TargetCurveHandle { get; set; }

        public Projector ToCalculator(Database db, Transaction trans)
        {
            var sourceCurve = AcadHelpers.GetObject<Polyline>(SourceCurveHandle, db, trans);
            var targetCurve = AcadHelpers.GetObject<Polyline>(TargetCurveHandle, db, trans);

            var notches = SourceNotchLines.Select(lineHandle =>
               {
                   var line = AcadHelpers.GetObject<Line>(lineHandle, db, trans);
                   return AcadHelpers.GetSingleIntersection(sourceCurve, line);
               });

            return new Projector(sourceCurve, notches.ToList(), targetCurve, TargetFacetPercentages);
        }

        public (Curve sourceCurve, IEnumerable<Point3d> sourceNotches, Curve targetCurve) ToObjects(Database db, Transaction trans)
        {
            var sourceCurve = AcadHelpers.GetObject<Polyline>(SourceCurveHandle, db, trans);
            var targetCurve = AcadHelpers.GetObject<Polyline>(TargetCurveHandle, db, trans);

            var notches = SourceNotchLines.Select(lineHandle =>
            {
                var line = AcadHelpers.GetObject<Line>(lineHandle, db, trans);
                return AcadHelpers.GetSingleIntersection(sourceCurve, line);
            });

            return (sourceCurve, notches.ToList(), targetCurve);
        }


    }
}
