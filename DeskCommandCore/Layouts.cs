using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DeskCommandCore.Messages;
using MediatR;
using Newtonsoft.Json;

namespace DeskCommandCore
{
    public class LayoutManager
    {
        private readonly IMediator _mediator;

        public LayoutManager(IMediator mediator)
        {
            _mediator = mediator;
        }

        private List<Layout> _layouts = new List<Layout>();

        public List<Layout> GetLayouts()
        {
            return _layouts;
        }

        public async Task UpdateLayouts(List<Layout> newLayouts)
        {
            _layouts = newLayouts;
        }
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
        public string IconRunning { get; set; }
        public string Text { get; set; }

        public bool IsRunning { get; set; }
        public InterfaceAction Action { get; set; }
    }
}
