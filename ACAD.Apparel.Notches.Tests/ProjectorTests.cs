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
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var calculator = KnownValues.Notches1.ToCalculator(db, trans);
                calculator.ShouldNotBeNull();
            });
        }

        [Test]
        public void ShouldProject()
        {
            DwgTest.ExecuteActionsInDwg(KnownValues.Notches1.DrawingPath, (db, trans) =>
            {
                var calculator = KnownValues.Notches1.ToCalculator(db, trans);
                var targetNotches = calculator.Project();

                var bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
                var blockTableRecord = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

                foreach (var notch in targetNotches)
                {
                    using (var notchPoint = new DBPoint(notch))
                    {
                        blockTableRecord.AppendEntity(notchPoint);
                        trans.AddNewlyCreatedDBObject(notchPoint, add: true);
                    }
                }

                // Set the style for all point objects in the drawing
                db.Pdmode = 34;
                db.Pdsize = 1;

                return KnownValues.Notches1.GenerateSnapshotDrawingPath();
            });
        }
    }
}
