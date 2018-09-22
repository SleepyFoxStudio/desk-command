using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskCommandCore.Models.Config
{
    public class Layouts : List<Layout>
    { }

    public class Layout
    {
        public string LayoutId { get; set; }
        public string Title { get; set; }
        public List<LayoutItem> Items { get; set; } = new List<LayoutItem>();
    }

    public class LayoutItem
    {
        public string Icon { get; set; }
        public string IconRunning { get; set; }
        public string Text { get; set; }
        public string Action { get; set; }
        public string[] Arguments { get; set; }
    }
}
