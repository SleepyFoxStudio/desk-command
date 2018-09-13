using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeskCommandCore
{
    public class Layouts
    {

        public Layout ActiveLayout { get; set; }
        public List<Layout> AllLayouts { get; set; } = new List<Layout>();
    }
}
