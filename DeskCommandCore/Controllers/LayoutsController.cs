using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DeskCommandCore.Actions;
using DeskCommandCore.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace DeskCommandCore.Controllers
{



    [Route("api/[controller]")]
    public class LayoutsController : Controller
    {

        private readonly IHubContext<ChatHub> _hubContext;
        private readonly LayoutsConfig _layoutsConfig;
        private readonly Layouts _layouts;
        private readonly Dictionary<string, Layout> _layoutDict;

        public LayoutsController(IOptionsSnapshot<LayoutsConfig> layoutsConfigAccessor, IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
            _layoutsConfig = layoutsConfigAccessor.Value;
            _layouts = ReadConfig(_layoutsConfig);
            _layoutDict = _layouts.ToDictionary(l => l.LayoutId);
        }

        public async Task SendToAll(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
        }


        [HttpGet]
        public Models.Layouts GetAllLayouts()
        {
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layouts>(_layouts);

            return layoutsDto;
        }

        [HttpGet("{id}")]
        public Models.Layout GetLayout(string id)
        {

            var u = FindLayout(id);
            var layoutsDto = AutoMapper.Mapper.Map<Models.Layout>(u);

            return layoutsDto;
        }

        [HttpPost("{id}/do/{actionId:int}")]
        public async Task DoAction(string id, int actionId)
        {
            await SendToAll($"Doing {id}, {actionId}");
            var layoutItem = FindLayoutItem(id, actionId);
            layoutItem?.Action.Do();
        }


        private Layout FindLayout(string layoutId)
        {
            return _layoutDict[layoutId];
        }

        private LayoutItem FindLayoutItem(string layoutId, int itemIndex)
        {
            var layout = FindLayout(layoutId);
            return layout.Items[itemIndex];
        }

        private static Layouts ReadConfig(LayoutsConfig config)
        {
            var layouts = new Layouts();
            foreach (var layoutConfig in config)
            {
                var layout = new Layout
                {
                    LayoutId = layoutConfig.LayoutId,
                    Title = layoutConfig.Title
                };

                foreach (var itemConfig in layoutConfig.Items)
                {
                    var layoutItem = new LayoutItem
                    {
                        Icon = itemConfig.Icon,
                        Text = itemConfig.Text
                    };

                    var actionType = Assembly.GetExecutingAssembly().GetType(itemConfig.Action);
                    var action = (InterfaceAction)Activator.CreateInstance(actionType, itemConfig.Arguments);
                    layoutItem.Action = action;

                    layout.Items.Add(layoutItem);
                }

                layouts.Add(layout);
            }
            return layouts;
        }

    }


}