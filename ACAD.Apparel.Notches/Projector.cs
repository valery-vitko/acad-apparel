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
        private readonly IEnumerable<double> targetFacetAdjustmentPercentages;

        public bool Inverted { get; set; } = true;

        public Projector(Curve sourceCurve, IEnumerable<Point3d> sourceNotches, Curve targetCurve, IEnumerable<double> targetFacetAdjustmentPercentages)
        {
            this.sourceCurve = sourceCurve;
            this.sourceNotches = sourceNotches;
            this.targetCurve = targetCurve;
            this.targetFacetAdjustmentPercentages = targetFacetAdjustmentPercentages;
        }

        public Point3d[] Project()
        {
            var sourceLength = AcadHelpers.GetCurveLength(sourceCurve);
            var targetLength = AcadHelpers.GetCurveLength(targetCurve);

            var adjustment = targetLength - sourceLength;

            var targetNotches = new List<Point3d>();

            double accumulatedAdjustment = 0;

            foreach (var (sourceNotch, adjustmentFractionPercentage) in sourceNotches.Zip(targetFacetAdjustmentPercentages, (notch, adj) => (notch, adj)))
            {
                var sourceFacetDist = sourceCurve.GetDistAtPoint(sourceNotch);

                accumulatedAdjustment += adjustment * adjustmentFractionPercentage / 100.0;
                var targetFacetDist = sourceFacetDist + accumulatedAdjustment;

                if (Inverted)
                    targetFacetDist = targetLength - targetFacetDist;

                var targetNotch = targetCurve.GetPointAtDist(targetFacetDist);
                targetNotches.Add(targetNotch);
            }

            return targetNotches.ToArray();
        }
    }
}
