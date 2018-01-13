using Autodesk.AutoCAD.DatabaseServices;
using Shouldly;

namespace ACAD.Apparel.Notches.Tests
{
    public static class ShouldExtensions
    {
        public static void ShouldExistIn(this Handle objectHandle, Database db, Transaction trans)
        {
            db.TryGetObjectId(objectHandle, out var objectId).ShouldBeTrue();
            trans.GetObject(objectId, OpenMode.ForRead).ShouldNotBeNull();
        }
    }
}
