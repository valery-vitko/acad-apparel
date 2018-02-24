using System;
using System.Threading;
using ACAD.Apparel.Notches.Tests.Drawings;
using ACAD.Common.Testing;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using NUnit.Framework;
using Shouldly;

namespace ACAD.Apparel.Notches.Tests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class ProjectorTests
    {
        [Test]
        public void ShouldCreateCalculator()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, tx) =>
            {
                var calculator = KnownValues.Notches1.ToProjector(db, tx);
                calculator.ShouldNotBeNull();
            });
        }

        [Test]
        public void ShouldProject()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, tx) =>
            {
                var projector = KnownValues.Notches1.ToProjector(db, tx);
                var targetNotches = projector.Project();

                AcadHelpers.GeneratePoints(db, tx, targetNotches);
                AcadHelpers.ConfigurePointsToBeVisible(db);

                return KnownValues.Notches1.GenerateSnapshotDrawingPath();
            });
        }
    }
}
