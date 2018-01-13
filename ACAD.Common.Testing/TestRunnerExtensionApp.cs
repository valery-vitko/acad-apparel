using System;
using System.IO;
using System.Runtime.InteropServices;
using ACAD.Common.Testing;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof (TestRunnerExtensionApp))]

namespace ACAD.Common.Testing
{
    public class TestRunnerExtensionApp : IExtensionApplication
    {
        public void Initialize()
        {
#if !CoreConsole
            if (!AttachConsole(-1))  // Attach to a parent process console
                AllocConsole(); // Alloc a new console if none available

#else
// http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652
// This code is so you can see the test results in the console window, as per the above forum thread.
            if (Application.Version.Major * 10 + Application.Version.Minor > 190) // >v2013
            {
                // http://stackoverflow.com/a/15960495/492
                // stdout's handle seems to always be equal to 7
//                var defaultStdout = new IntPtr(7);
//                IntPtr currentStdout = GetStdHandle(StdOutputHandle);
//                if (currentStdout != defaultStdout)
//                    SetStdHandle(StdOutputHandle, defaultStdout);

//// http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652#M43708
//                FixStdOut();

                ConsoleMode.SetMode();
            }
#endif
        }

        public void Terminate()
        {
#if !CoreConsole
            FreeConsole();
#endif
        }

        // http://stackoverflow.com/questions/3917202/how-do-i-include-a-console-in-winforms/3917353#3917353
        /// <summary> Allocates a new console for current process. </summary>
        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        /// <summary> Frees the console. </summary>
        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        // http://stackoverflow.com/a/8048269/492
        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(int pid);

#if CoreConsole
// http://forums.autodesk.com/t5/net/accoreconsole-exe-in-2015-doesn-t-do-system-console-writeline/m-p/5551652#M43708
// trying to fix the telex-like line output from Core Console in versions > 2013. This didn't work for me. :(
        [DllImport("api-ms-win-crt-stdio-l1-1-0.dll")]
        private static extern int _setmode(int fileno, int mode);

        public void FixStdOut() { _setmode(3, 0x4000); }

// http://stackoverflow.com/a/15960495/492
        private const uint StdOutputHandle = 0xFFFFFFF5;

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(uint nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);
#endif
    }



    class ConsoleMode
    {
        private const uint StdOutputHandle = 0xFFFFFFF5;
        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 4;

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(uint nStdHandle);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static void SetMode()
        {
            var handle = GetStdHandle(StdOutputHandle);
            uint mode;
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
            SetConsoleMode(handle, mode);

            //const string UNDERLINE = "\x1B[4m";
            //const string RESET = "\x1B[0m";
            //Console.WriteLine("Some " + UNDERLINE + "underlined" + RESET + " text");
            //Console.ReadLine();
        }
    }
}