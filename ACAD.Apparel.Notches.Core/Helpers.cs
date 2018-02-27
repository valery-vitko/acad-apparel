using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACAD.Apparel.Notches
{
    public static class AcadHelpers
    {
        public static TObject GetObject<TObject>(Handle objectHandle, Database db, Transaction tx) where TObject : DBObject
        {
            if (!db.TryGetObjectId(objectHandle, out var objectId))
                throw new InvalidOperationException($"Object with handle {objectHandle} not found");

            return GetObject<TObject>(objectId, tx);
        }

        public static TObject GetObject<TObject>(ObjectId objectId, Transaction tx) where TObject : DBObject
        {
            var obj = tx.GetObject(objectId, OpenMode.ForRead) as TObject;
            if (obj == null)
                throw new InvalidOperationException($"Object with id {objectId} cannot be retrieved for {OpenMode.ForRead} as type {typeof(TObject)}");

            return obj;
        }

        public static double GetDistanceToPoint(Curve curve, Point3d pt)
        {
            var ptOnCurve = curve.GetClosestPointTo(pt, extend: false);
            double ptparam = curve.GetParameterAtPoint(ptOnCurve);
            double a = curve.GetDistanceAtParameter(ptparam);
            double b = curve.GetDistanceAtParameter(curve.StartParam);
            return a - b;
        }

        public static Point3d GetSingleIntersection(Curve /* TODO: Polyline only? */ curve, Line line)
        {
            var intersections = GetIntersections(curve, line);
            if (intersections.Count != 1)
                throw new InvalidOperationException($"Expected single intersection for {line.Handle} over {curve.Handle}, but got {intersections.Count}");

            return intersections[0];
        }

        public static Point3dCollection GetIntersections(Curve /* TODO: Polyline only? */ curve, Line line)
        {
            var intersections = new Point3dCollection();
            line.IntersectWith(curve, Intersect.OnBothOperands, intersections, IntPtr.Zero, IntPtr.Zero);
            return intersections;
        }

        public static double GetCurveLength(Curve sourceCurve)
        {
            var startParam = sourceCurve.StartParam;
            var endParam = sourceCurve.EndParam;
            return sourceCurve.GetDistanceAtParameter(endParam) - sourceCurve.GetDistanceAtParameter(startParam);
        }

        public static void GeneratePoints(Database db, Transaction tx, Point3d[] targetNotches)
        {
            var bt = (BlockTable)tx.GetObject(db.BlockTableId, OpenMode.ForRead);
            var blockTableRecord = (BlockTableRecord)tx.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            foreach (var notch in targetNotches)
            {
                using (var notchPoint = new DBPoint(notch))
                {
                    blockTableRecord.AppendEntity(notchPoint);
                    tx.AddNewlyCreatedDBObject(notchPoint, add: true);
                }
            }
        }

        public static void ConfigurePointsToBeVisible(Database db)
        {
            db.Pdmode = 34;
            db.Pdsize = 1;
        }
    }
}
