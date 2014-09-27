using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using CefTest;

namespace CefTest
{
    [TestClass]
    public abstract class CefConfig
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        static extern IntPtr LoadLibrary(string lpFileName);

        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(context.TestDir));
            string cefRoot = Path.Combine(solution_dir, @"Resources\cef\");

            LoadLibrary(cefRoot + "libcef.dll");

            var mainArgs = new CefMainArgs(new string[] { });
            var cefApp = new TestCefApp();

            var exitCode = CefRuntime.ExecuteProcess(mainArgs, cefApp, IntPtr.Zero);
            if (exitCode != -1)
            {
                Assert.Fail();
                return;
            }

            var cefSettings = new CefSettings
            {
                SingleProcess = true,
                WindowlessRenderingEnabled = true,
                MultiThreadedMessageLoop = true,
                LogSeverity = CefLogSeverity.Disable,
                LogFile = "cef.log",
                ResourcesDirPath = cefRoot + "\\Resources",
                LocalesDirPath = cefRoot + "\\Resources\\locales"
            };

            try
            {
                CefRuntime.Initialize(mainArgs, cefSettings, cefApp, IntPtr.Zero);
            }
            catch (CefRuntimeException ex)
            {
                Assert.Fail();
                return;
            }
        }

        [AssemblyCleanup]
        public static void Cleanup()
        {
            Task.Factory.StartNew(() =>
            {
                CefRuntime.Shutdown();
            });

            // Allow time for Cef to shutdown before this process terminates.
            System.Threading.Thread.Sleep(2000);
        }
    }
}
