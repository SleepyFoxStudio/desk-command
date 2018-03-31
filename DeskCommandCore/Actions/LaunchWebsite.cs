using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskCommandCore.Actions
{
    public class LaunchWebsite : InterfaceAction
    {
        public LaunchWebsite(string url)
        {
            Url = url;
        }

        public string Url { get; set; }

        public void Do()
        {
            if (!string.IsNullOrWhiteSpace(Url))
            {
                System.Diagnostics.Process.Start(Url);
            }
        }
    }
}
