using System.Threading;
using ACAD.Apparel.Notches.Tests.Drawings;
using ACAD.Common.Testing;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;
using Shouldly;

namespace ACAD.Apparel.Notches.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class TestInputTests
    {
        [Test]
        public void ShouldFindNotchPointsByLinesOnCurve()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var sourceCurve = AcadHelpers.GetObject<Polyline>(KnownValues.Notches1.SourceCurveHandle, db, trans);

                foreach (var lineHandle in KnownValues.Notches1.SourceNotchLines)
                {
                    var line = AcadHelpers.GetObject<Line>(lineHandle, db, trans);
                    AcadHelpers.GetSingleIntersection(sourceCurve, line);
                }
            });
        }

        [Test]
        public void TestInputsShouldExist()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                KnownValues.Notches1.SourceCurveHandle.ShouldExistIn(db, trans);
                KnownValues.Notches1.TargetCurveHandle.ShouldExistIn(db, trans);
                foreach (var lineHandle in KnownValues.Notches1.SourceNotchLines)
                    lineHandle.ShouldExistIn(db, trans);
            });
        }

        [Test]
        public void InvalidInputsShouldNotExist()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var invalidHandle = new Handle(0xfffff); // let's just assume it does not exist
                db.TryGetObjectId(invalidHandle, out var objectId).ShouldBeFalse();
                objectId.IsNull.ShouldBeTrue();
                objectId.IsValid.ShouldBeFalse();
            });
        }
    }
}
