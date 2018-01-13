using System.Linq;
using System.IO;
using System.Reflection;
using System.Threading;
using ACAD.Apparel.Notches.Tests.Drawings;
using ACAD.Common.Testing;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NUnit.Framework;
using Shouldly;

namespace ACAD.Apparel.Notches.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class GeometryTests
    {
        [Test]
        public void LengthsAreEqual()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var (sourceCurve, sourceNotches, targetCurve) = KnownValues.Notches1.ToObjects(db, trans);
                var sourceLength = (sourceCurve as Polyline).Length;
                var distSourceLength = AcadHelpers.GetCurveLength(sourceCurve);
                sourceLength.ShouldBe(distSourceLength);
            });
        }

        [Test]
        public void Props()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var (sourceCurve, sourceNotches, targetCurve) = KnownValues.Notches1.ToObjects(db, trans);

                var notchCollection = new Point3dCollection(sourceNotches.ToArray());
                var notchSegments = sourceCurve.GetSplitCurves(notchCollection);

                var sourceLength = AcadHelpers.GetCurveLength(sourceCurve);

                var facet1 = notchSegments[0] as Curve;

                var facet1Length = AcadHelpers.GetCurveLength(facet1);
                var facet1RatioByLengths = facet1Length / sourceLength;

                //var facet1RatioByParams = sourceCurve.GetParameterAtPoint(notchCollection[0]) / (sourceCurve.EndParam - sourceCurve.StartParam);
                //facet1RatioByLengths.ShouldBe(facet1RatioByParams);
            });
        }
    }
}
