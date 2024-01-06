using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

using NCryptor.GUI.Forms;

namespace NCryptor.GUI
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly()
                                                        .GetCustomAttributes(typeof(GuidAttribute), false)
                                                        .GetValue(0)).Value.ToString();
            string mutexId = $"Global\\{appGuid}";
            bool createdNew;
            var rule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                            MutexRights.FullControl,
                                            AccessControlType.Allow);
            var mutexSettings = new MutexSecurity();
            mutexSettings.AddAccessRule(rule);

            using(var mutex = new Mutex(false, mutexId, out createdNew, mutexSettings))
            {
                bool hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(1000, false);
                        if (!hasHandle)
                        {
                            MessageBox.Show("An instance of the application is already running.", "NCryptor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        hasHandle = true;
                    }

                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainWindow());
                }
                finally
                {
                    if (hasHandle)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }
    }
}
