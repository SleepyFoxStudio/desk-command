using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private readonly Dictionary<string, Layout> _layoutDict;
        private readonly Layouts _layouts;


        public LayoutsController(IOptionsSnapshot<LayoutsConfig> layoutsConfigAccessor, IHubContext<ChatHub> hubContext, Layouts layouts)
        {
            _layouts = layouts;
            _hubContext = hubContext;
            _layoutsConfig = layoutsConfigAccessor.Value;
            _layoutDict = _layouts.ToDictionary(l => l.LayoutId);
        }

        public async Task ChangeButtonImage(string id, int actionId, string imgUrl)
        {
            await _hubContext.Clients.All.SendAsync("ChangeImage",id, actionId,imgUrl);
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
            var layoutItem = FindLayoutItem(id, actionId);
            layoutItem.IsRunning = true;
            //if (layoutItem?.IconRunning != null)
            //{
            //    await ChangeButtonImage(id, actionId, layoutItem.IconRunning);
            //}
            layoutItem?.Action.Do();
            //if (layoutItem?.Icon != null)
            //{
            //    await ChangeButtonImage(id, actionId, layoutItem.Icon);
            //}
            layoutItem.IsRunning = false;
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



    }


}