using System;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using NUnitLite;

namespace ACAD.Common.Testing
{
    public class TestRunner
    {
        [LispFunction("RunTests")]
        public void RunTests(ResultBuffer args)
        {
            foreach (TypedValue arg in args)
            {
                if (arg.TypeCode == (int)LispDataType.Text)
                    RunTestsInAssembly(arg.Value.ToString());
            }

            AutoCadLog.Info("Done");
        }

        private void RunTestsInAssembly(string assemblyName)
        {
            AutoCadLog.Info($"Running tests from assembly {assemblyName}");

            var testAssembly = Assembly.Load(assemblyName);

            // for details of options see  http://www.nunit.com/index.php?p=nunitliteOptions&r=3.0
            var nunitArgs = new string[]
            {
                "--trace=Verbose",
                //"--wait" // Wait for input before closing console window (PAUSE)
            };

            var failedTestCount = new AutoRun(testAssembly).Execute(nunitArgs, new TestRunnerConsoleTextWriter(), Console.In);
        }
    }

    public static class AutoCadLog
    {
        public static void Info(string text)
        {
            Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(text);
        }
    }
}
