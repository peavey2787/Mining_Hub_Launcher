using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mining_Hub_Launcher
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Define a unique name for the Mutex
            string mutexName = "Mining_Hub_Launcher";

            // Create the Mutex
            bool createdNew;
            Mutex mutex = new Mutex(true, mutexName, out createdNew);

            // If the Mutex already exists, another instance of the application is running
            if (!createdNew)
            {
                MessageBox.Show("Another instance of the application is already running.");
                return;
            }

            // Run the application
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_Form());

            // Release the Mutex when the application exits
            mutex.ReleaseMutex();
        }
    }
}
