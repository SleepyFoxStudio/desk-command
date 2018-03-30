using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Desk_Command_Core.Actions;
using Desk_Command_Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;


namespace Desk_Command_Core.Controllers
{
    [Route("api/[controller]")]
    public class LayoutsController : Controller
    {
        private readonly LayoutsConfig _layoutsConfig;
        private readonly Layouts _layouts;
        private readonly Dictionary<string, Layout> _layoutDict;

        public LayoutsController(IOptionsSnapshot<LayoutsConfig> layoutsConfigAccessor)
        {
            _layoutsConfig = layoutsConfigAccessor.Value;
            _layouts = ReadConfig(_layoutsConfig);
            _layoutDict = _layouts.ToDictionary(l => l.LayoutId);
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
        public string DoAction(string id, int actionId)
        {
            var layoutItem = FindLayoutItem(id, actionId);
            layoutItem?.Action.Do();
            return "Ok!";
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