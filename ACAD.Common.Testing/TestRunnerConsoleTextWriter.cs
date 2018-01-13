using System.Text;
using NUnit.Common;

namespace ACAD.Common.Testing
{
    internal class TestRunnerConsoleTextWriter : ExtendedTextWriter
    {
        private void WriteImpl(string text) =>
            Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(text);

        private void WriteCore(string text) => WriteImpl(text);

        private void WriteLineCore(string text) => WriteImpl(text + "\n");


        public override Encoding Encoding => Encoding.ASCII;

        public override void Write(ColorStyle style, string value) => WriteCore(value);

        public override void WriteLabel(string label, object option) => WriteCore(label + option);

        public override void WriteLabel(string label, object option, ColorStyle valueStyle) => WriteLabel(label, option);

        public override void WriteLabelLine(string label, object option) => WriteLineCore(label + option);

        public override void WriteLabelLine(string label, object option, ColorStyle valueStyle) => WriteLabelLine(label, option);

        public override void WriteLine(ColorStyle style, string value) => WriteLineCore(value);
    }
}
