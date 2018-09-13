using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeskCommandCore.Models
{
    public class Ui
    {
        public string Title { get; set; }
        public string SelectedLayout { get; set; }
        public List<Heading> Headings { get; set; } = new List<Heading>();
        public List<LayoutItem> Items { get; set; } = new List<LayoutItem>();

    }

    public class Heading
    {
        public string Title { get; set; }
        public string Id { get; set; }

    }
}