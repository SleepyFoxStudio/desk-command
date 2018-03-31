using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using DeskCommandCore.Properties;
using Microsoft.AspNetCore.Hosting;

namespace DeskCommandCore
{
    public class TrayHelper
    {
        private readonly IWebHost _webHost;

        private ToolStripMenuItem _startDeviceMenuItem;
        private ToolStripMenuItem _stopDeviceMenuItem;
        private ToolStripMenuItem _exitMenuItem;
        private NotifyIcon _notifyIcon;
        private System.ComponentModel.IContainer _components;
        // This allows code to be run on a GUI thread
        private System.Windows.Window _hiddenWindow;

        // This allows code to be run on a GUI thread

        public TrayHelper(Microsoft.AspNetCore.Hosting.IWebHost _webHost)
        {
            this._webHost = _webHost;

            System.Drawing.Icon trayStoppedIcon;
            using (var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DeskCommandCore.Resources.trayRunning.ico"))
            {
                trayStoppedIcon = new System.Drawing.Icon(iconStream);
            }

            _components = new System.ComponentModel.Container();
            _notifyIcon = new NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = trayStoppedIcon,
                Text = Resources.TrayHelper_TrayHelper_Desk_Command,
                Visible = true,
            };

            _notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;

            _hiddenWindow = new System.Windows.Window();
            _hiddenWindow.Hide();
            
            UserMessage("Welcome to Desk Command", "Click on the icon below to start");
        }

        private void UserMessage(string title, string text, EventHandler<EventArgs> eventHandler = null)
        {
            _hiddenWindow.Dispatcher.Invoke(delegate
            {
                _notifyIcon.BalloonTipTitle = title;
                _notifyIcon.BalloonTipText = text;
                _notifyIcon.ShowBalloonTip(3000);
                if (eventHandler != null)
                {
                    _notifyIcon.BalloonTipClicked += new EventHandler(eventHandler);
                }
            });
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if (_notifyIcon.ContextMenuStrip.Items.Count == 0)
            {

                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Start Desk Command Interface", "Start the Desk Command Interface so you can access it from external devices.", StartWeb_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Stop Desk Command Interface", "Stop the Desk Command Interface.", StopWeb_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Code Project Web Site", "Navigates to the Code Project Web Site", ShowWebSite_Click));
                _notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
                _exitMenuItem = ToolStripMenuItemWithHandler("&Exit", "Exits System Tray App", exitItem_Click);
                _notifyIcon.ContextMenuStrip.Items.Add(_exitMenuItem);
                _exitMenuItem.Enabled = true;
            }
        }


        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(_notifyIcon, null);
            }
        }


        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, string tooltipText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }

            item.ToolTipText = tooltipText;
            return item;
        }

        private void StartWeb_Click(object sender, EventArgs e)
        {
            _webHost.Start();
            UserMessage("Desk Command running", "Now running, you can access the interface via http://localhost:5000/", notifyIcon_BalloonTipClicked);
        }

        private void StopWeb_Click(object sender, EventArgs e)
        {
            _webHost.StopAsync(TimeSpan.FromMilliseconds(5000));
        }

        private static void ShowWebSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tridionted/desk-command");
        }


        void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://localhost:5000/");
        }



        private void exitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}