using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using DeskCommandCore.Properties;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;

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
            _webHost.Start();

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




            ShowRunningMessage();
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

                _notifyIcon.ContextMenuStrip.Items.Add(ToolStripMenuItemWithHandler("Show Command Interface", "Show the Desk Command UI on your local PC.", ShowUi));
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

        private void ShowRunningMessage()
        {
            Uri address = GetWebHostAddress();

            string runningMessage = "Now running";
            if (address != null)
            {
                runningMessage += $", you can access the interface via {address}";
            }

            UserMessage("Desk Command running", runningMessage, ShowUi);
        }

        private Uri GetWebHostAddress()
        {
            var address = _webHost.ServerFeatures?.Get<IServerAddressesFeature>()?.Addresses?.FirstOrDefault();

            Uri sanitizedAddress = null;
            if (address != null)
            {
                var uriBuilder = new UriBuilder(address);
                if (uriBuilder.Host == "0.0.0.0")
                    uriBuilder.Host = "localhost";

                sanitizedAddress = uriBuilder.Uri;
            }

            return sanitizedAddress;
        }



        private static void ShowWebSite_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/tridionted/desk-command");
        }

        void ShowUi(object sender, EventArgs e)
        {
            var address = GetWebHostAddress();
            System.Diagnostics.Process.Start(address.ToString());
        }


        private void exitItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

    }
}