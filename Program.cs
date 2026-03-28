using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotiKnob
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex singleInstanceMutex;
            if (!SingleInstanceManager.TryAcquire(out singleInstanceMutex))
            {
                SingleInstanceManager.SignalExistingInstance();
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (singleInstanceMutex)
            {
                Application.Run(new Form1());
            }
        }
    }
}
