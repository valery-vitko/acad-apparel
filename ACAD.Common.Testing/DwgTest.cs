using System;
using System.IO;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace ACAD.Common.Testing
{
    public class DwgTest
    {
        public static void ExecuteActionsInDwg(String drawingPath, params Action<Database, Transaction>[] actionList)
        {
            bool buildDefaultDrawing;

            if (String.IsNullOrEmpty(drawingPath))
            {
                buildDefaultDrawing = true;
            }
            else
            {
                buildDefaultDrawing = false;

                if (!File.Exists(drawingPath))
                {
                    Assert.Fail("The file '{0}' doesn't exist", drawingPath);
                    return;
                }
            }

            Exception exceptionToThrown = null;

            var doc = Application.DocumentManager.MdiActiveDocument;
            using (doc.LockDocument())
            {
                using (Database db = new Database(buildDefaultDrawing, false))
                {
                    if (!String.IsNullOrEmpty(drawingPath))
                        db.ReadDwgFile(drawingPath, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                    using (new Helpers.WorkingDatabaseSwitcher(db))
                    {
                        foreach (var item in actionList)
                        {
                            using (Transaction tr = db.TransactionManager.StartTransaction())
                            {
                                try
                                {
                                    item(db, tr);
                                }
                                catch (Exception ex)
                                {
                                    exceptionToThrown = ex;
                                    tr.Commit();

                                    //stop execution of actions
                                    break;
                                }

                                tr.Commit();

                            }//transaction

                        }//foreach

                    }//change WorkingDatabase

                }//database

            }//lock document

            //throw exception outside of transaction
            //Sometimes Autocad crashes when exception throws
            if (exceptionToThrown != null)
            {
                throw exceptionToThrown;
            }
        }
    }
}
