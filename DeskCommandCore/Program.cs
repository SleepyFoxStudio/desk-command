using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeskCommandCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DeskCommandCore
{
    public class STAApplicationContext : ApplicationContext
    {
        public STAApplicationContext(string[] args)
        {
            _webHost = BuildWebHost(args);
            _trayHelper = new TrayHelper(_webHost);
        }

        private TrayHelper _trayHelper;
        private IWebHost _webHost;

        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {
            _trayHelper = null;
            _webHost.StopAsync(TimeSpan.FromMilliseconds(100));
        }

        private static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }


    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            // Use the assembly GUID as the name of the mutex which we use to detect if an application instance is already running
            bool createdNew = false;
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out createdNew))
            {
                if (!createdNew)
                {
                    // Only allow one instance
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    STAApplicationContext context = new STAApplicationContext(args);
                    Application.Run(context);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
        }
    }
}
