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
        }



        public Task InitializeGui()
        {
            var u = FindDefaultLayout();
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layout>(u);

            GetLayoutHeadings();
            return Clients.All.SendAsync("ReceiveLayout", layoutsDto);
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
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layout>(u);

            return Clients.All.SendAsync("ReceiveLayout", layoutsDto);
        }




        public Task SendMessage(string user, string message)
        {
            string timestamp = DateTime.Now.ToShortTimeString();
            return Clients.All.SendAsync("ReceiveMessage", timestamp, user, message);
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
