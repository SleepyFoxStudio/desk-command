using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeskCommandCore.Actions
{
    public class SendKeystrokes : InterfaceAction
    {


        public SendKeystrokes(string keys, string processNames)
        {
            Keys = keys;
            ProcessNames = processNames.Split(',').ToList();
        }

        public string Keys { get; set; }
        public List<String> ProcessNames { get; set; } = new List<string>();


        public static IntPtr WinGetHandle(string wName)
        {
            foreach (Process pList in Process.GetProcesses())
            {

                //  Console.WriteLine(pList.ProcessName + pList.Id);
                if (String.Equals(pList.ProcessName, wName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Handle found for {wName} :   {pList.MainWindowHandle}");
                    return pList.MainWindowHandle;
                }
            }
            Console.WriteLine($"Could not find Handle for {wName} :  {wName}");
            return IntPtr.Zero;
        }


        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);





        public Task Do()
        {
            if (String.IsNullOrWhiteSpace(Keys)) return Task.CompletedTask;
            foreach (var processName in ProcessNames)
            {
                try
                {
                    // find window handle of Notepad
                    IntPtr handle = WinGetHandle(processName);
                    if (!handle.Equals(IntPtr.Zero))
                    {
                        // activate Notepad window
                        if (SetForegroundWindow(handle))
                        {
                            Thread.Sleep(150);
                            SendKeys.SendWait(Keys);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return Task.CompletedTask;
        }
    }
}
