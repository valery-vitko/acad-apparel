using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace ACAD.Apparel.Notches
{
    public static class AcadHelpers
    {
        public static TObject GetObject<TObject>(Handle objectHandle, Database db, Transaction trans) where TObject : DBObject
        {
            if (!db.TryGetObjectId(objectHandle, out var objectId))
                throw new InvalidOperationException($"Object with handle {objectHandle} not found");

            var obj = trans.GetObject(objectId, OpenMode.ForRead) as TObject;
            if (obj == null)
                throw new InvalidOperationException($"Object with handle {objectHandle} cannot be retrieved for {OpenMode.ForRead} as type {typeof(TObject)}");

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

        public static Point3d GetSingleIntersection(Polyline curve, Line line)
        {
            var intersections = new Point3dCollection();
            line.IntersectWith(curve, Intersect.OnBothOperands, intersections, IntPtr.Zero, IntPtr.Zero);
            if (intersections.Count != 1)
                throw new InvalidOperationException($"Expected single intersection for {line.Handle} over {curve.Handle}, but got {intersections.Count}");

            return intersections[0];
        }

        public static double GetCurveLength(Curve sourceCurve)
        {
            var startParam = sourceCurve.StartParam;
            var endParam = sourceCurve.EndParam;
            return sourceCurve.GetDistanceAtParameter(endParam) - sourceCurve.GetDistanceAtParameter(startParam);
        }
    }
}
