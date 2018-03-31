using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskCommandCore.Models
{
    public class LayoutsConfig : List<LayoutConfig>
    {
    }

    public class LayoutConfig
    {
        public string LayoutId { get; set; }
        public string Title { get; set; }

        public List<LayoutItemConfig> Items { get; set; } = new List<LayoutItemConfig>();

    }

    public class LayoutItemConfig
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Action { get; set; }
        public string[] Arguments { get; set; }
    }
}
