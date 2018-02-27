using Autodesk.AutoCAD.ApplicationServices.Core;

namespace ACAD.Apparel.Notches
{
    public class Logger
    {
        private readonly string prefix;

        public Logger() : this(null)
        {
        }

        public Logger(string prefix)
        {
            this.prefix = prefix != null ? prefix + ": " : "";
        }

        public void Debug(string message)
        {
            WriteLine(message);
        }

        public void Warn(string message)
        {
            WriteLine(message);
        }

        public void Error(string message)
        {
            WriteLine(message);
        }

        private void Write(string message)
        {
            var doc = Application.DocumentManager.MdiActiveDocument;
            if (doc != null)
                doc.Editor.WriteMessage(prefix + message);
        }

        private void WriteLine(string message)
        {
            Write(message + "\r\n");
        }
    }
}
