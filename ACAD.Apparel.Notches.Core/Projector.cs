using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACAD.Apparel.Notches
{
    public class Projector
    {
        private static Logger logger = new Logger("Notches (Projection)");

        private readonly Curve sourceCurve;
        private readonly IEnumerable<Point3d> sourceNotches;
        private readonly Curve targetCurve;

        public List<double> TargetFacetAdjustmentPercentages { get; }

        public bool IsInverted { get; set; } = true;

        public Projector(Curve sourceCurve, IEnumerable<Point3d> sourceNotches, Curve targetCurve, IEnumerable<double> targetFacetAdjustmentPercentages = null)
        {
            this.sourceCurve = sourceCurve;
            this.sourceNotches = sourceNotches;
            this.targetCurve = targetCurve;

            this.TargetFacetAdjustmentPercentages = new List<double>(targetFacetAdjustmentPercentages ?? new double[0]);
        }

        public static Projector FromNotchLines(Curve curve1, Curve curve2, IEnumerable<Line> notchLines, IEnumerable<double> targetFacetAdjustmentPercentages = null)
        {
            var (sourceCurve, targetCurve) = TryDetectSourceAndTargetCurves(curve1, curve2, notchLines);
            var sourceNotches = GetOrderedNotches(sourceCurve, notchLines);

            return new Projector(sourceCurve, sourceNotches, targetCurve, targetFacetAdjustmentPercentages);
        }

        private static (Curve sourceCurve, Curve targetCurve) TryDetectSourceAndTargetCurves(Curve curve1, Curve curve2, IEnumerable<Line> notchLines)
        {
            var curve1NotchedCount = GetCurveNotchedCount(curve1, notchLines);
            var curve2NotchedCount = GetCurveNotchedCount(curve2, notchLines);

            if (curve1NotchedCount > 0 && curve2NotchedCount == 0)
                return (curve1, curve2);
            else if (curve2NotchedCount > 0 && curve1NotchedCount == 0)
                return (curve2, curve1);

            logger.Warn($"Can't autodetect source and target curves by notches intersections. Using the first curve ${curve1.Handle} as a source one.");
            return (curve1, curve2);
        }

        private static int GetCurveNotchedCount(Curve curve1, IEnumerable<Line> notchLines) =>
            notchLines.Select(line => AcadHelpers.GetIntersections(curve1, line).Count).Sum();

        private static List<Point3d> GetOrderedNotches(Curve sourceCurve, IEnumerable<Line> notchLines)
        {
            return notchLines
                .Select(line => AcadHelpers.GetSingleIntersection(sourceCurve, line))
                .OrderBy(notch => sourceCurve.GetDistAtPoint(notch))
                .ToList();
        }

        public Point3d[] Project()
        {
            var sourceLength = AcadHelpers.GetCurveLength(sourceCurve);
            var targetLength = AcadHelpers.GetCurveLength(targetCurve);

            var adjustment = targetLength - sourceLength;

            var targetNotches = new List<Point3d>();

            double accumulatedAdjustment = 0;

            foreach (var (sourceNotch, adjustmentFractionPercentage) in sourceNotches.Zip(TargetFacetAdjustmentPercentages, (notch, adj) => (notch, adj)))
            {
                var sourceFacetDist = sourceCurve.GetDistAtPoint(sourceNotch);

                accumulatedAdjustment += adjustment * adjustmentFractionPercentage / 100.0;
                var targetFacetDist = sourceFacetDist + accumulatedAdjustment;

                if (IsInverted)
                    targetFacetDist = targetLength - targetFacetDist;

                // Check if the target notch point is still on the curve, .GetPointAtDist will fail otherwise
                if (targetFacetDist <= targetLength)
                {
                    var targetNotch = targetCurve.GetPointAtDist(targetFacetDist);
                    targetNotches.Add(targetNotch);
                }
                else
                {
                    logger.Warn($"Target notch point at {targetFacetDist:N2} exceeds target curve length of {targetLength:N2} and can't be set");
                }
            }

            return targetNotches.ToArray();
        }

        public Stats GetFacetStats()
        {
            var sourceLength = AcadHelpers.GetCurveLength(sourceCurve);

            return new Stats
            {
                SourceLength = sourceLength,
                TargetLength = AcadHelpers.GetCurveLength(targetCurve),
                SourceFacetLengths = GetSourceFacetLengts(sourceLength).ToList()
            };
        }

        private IEnumerable<double> GetSourceFacetLengts(double totalLength)
        {
            var previousNotchDist = 0.0;

            foreach (var sourceNotch in sourceNotches)
            {
                var sourceFacetDist = sourceCurve.GetDistAtPoint(sourceNotch);
                yield return sourceFacetDist - previousNotchDist;
                previousNotchDist = sourceFacetDist;
            }

            yield return totalLength - previousNotchDist;
        }

        public class Stats
        {
            public double SourceLength { get; set; }
            public double TargetLength { get; set; }

            public IList<double> SourceFacetLengths { get; set; }
        }
    }
}
