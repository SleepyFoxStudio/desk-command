using System.Collections.Generic;


namespace Desk_Command_Core
{

    public class Layouts : List<Layout>
    {
    }

    public class Layout
    {
        public string LayoutId { get; set; }
        public string Title { get; set; }

        public List<LayoutItem> Items { get; set; } = new List<LayoutItem>();

    }

    public class LayoutItem
    {
        public string Icon { get; set; }
        public string Text { get; set; }
        //[DataMember(Name = "action", Order = 1)]
        public InterfaceAction Action { get; set; }
    }
}
