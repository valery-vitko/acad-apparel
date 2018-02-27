using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACAD.Apparel.Notches
{
    public class Projector
    {
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

        public static Projector FromNotchLines(Curve sourceCurve, IEnumerable<Line> sourceNotchLines, Curve targetCurve, IEnumerable<double> targetFacetAdjustmentPercentages = null)
        {
            var sourceNotches = sourceNotchLines.Select(line => AcadHelpers.GetSingleIntersection(sourceCurve, line)).ToList();
            return new Projector(sourceCurve, sourceNotches, targetCurve, targetFacetAdjustmentPercentages);
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

                var targetNotch = targetCurve.GetPointAtDist(targetFacetDist);
                targetNotches.Add(targetNotch);
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
