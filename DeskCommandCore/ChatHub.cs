using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace DeskCommandCore
{
    public class ChatHub : Hub
    {
        private readonly Layouts _layouts;
        private readonly Dictionary<string, Layout> _layoutDict;


        public ChatHub(Layouts layouts)
        {
            _layouts = layouts;
            _layoutDict = _layouts.ToDictionary(l => l.LayoutId, StringComparer.OrdinalIgnoreCase);

            foreach (var layout in layouts)
            {
                layout.LayoutChanged += LayoutOnLayoutChanged;
            }
        }

        private void LayoutOnLayoutChanged(object sender, LayoutChangedEventArgs layoutChangedEventArgs)
        {
            var layout = sender as Layout;
            if (layout == null)
                return;

            try
            {
                GetLayoutInternal(layout).Wait();
            }
            catch (Exception ex)
            {
                
            }
        }

        public Task InitializeGui()
        {
            var u = FindDefaultLayout();
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layout>(u);

            GetLayoutHeadings();
            return Clients.All.SendAsync("ReceiveLayout", layoutsDto);
        }



        public Task DoAction(string layout, int itemIndex)
        {
            var layoutItem = FindLayoutItem(layout, itemIndex);


            if (layoutItem.IsRunning)
            {
                //We don`t want to run it if it`s already running, maybe we wont to stop it?
                //TODO: stop it if they have clicked to start and is already running
                return Task.CompletedTask;
            }
            layoutItem.IsRunning = true;
            layoutItem?.Action.Do();
            layoutItem.IsRunning = false;
            return Task.CompletedTask;
        }




        private Task GetLayoutHeadings()
        {
            var layoutHeadings = new Dictionary<string, string>();
            foreach (var layout in _layouts)
            {
                layoutHeadings.Add(layout.LayoutId, layout.Title);
            }

            return Clients.All.SendAsync("ReceiveLayoutHeadings", layoutHeadings);
        }



        public Task GetLayout(string layout)
        {
            var u = FindLayout(layout);
            return GetLayoutInternal(u);
        }

        public Task GetLayoutInternal(Layout layout)
        {
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layout>(layout);

            return Clients.All.SendAsync("ReceiveLayout", layoutsDto);
        }


        private Layout FindLayout(string layoutId)
        {
            return _layoutDict[layoutId];
        }

        private Layout FindDefaultLayout()
        {
            return _layoutDict.Any() ? _layoutDict.First().Value : null;
        }

        private LayoutItem FindLayoutItem(string layoutId, int itemIndex)
        {
            var layout = FindLayout(layoutId);
            return layout.Items[itemIndex];
        }

    }
}
