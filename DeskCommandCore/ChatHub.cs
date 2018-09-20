using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DeskCommandCore.Models;
using Microsoft.AspNetCore.SignalR;

namespace DeskCommandCore
{
    public class ChatHub : Hub
    {
        private readonly Layouts _layouts;
        private Ui _ui { get; set; }


        public ChatHub(Layouts layouts)
        {
            _layouts = layouts;
            UpdateUiModel();
        }

        public override Task OnConnectedAsync()
        {
            SendUiModel();
            return base.OnConnectedAsync();
        }

        private void SendUiModel()
        {
            Clients.All.SendAsync("ReceiveUi", _ui);
        }


        public void DoAction(string layout, int itemIndex)
        {
            System.Diagnostics.Debug.WriteLine($"Starting {layout} {itemIndex}");
            var layoutItem = _layouts.AllLayouts.Find(t => t.LayoutId == layout).Items[itemIndex];

            if (layoutItem.IsRunning)
            {
                //We don`t want to run it if it`s already running, maybe we want to stop it?
                //TODO: stop it if they have clicked to start and is already running
                return;
            }

            layoutItem.IsRunning = true;
            Task.Run(async () =>
            {
                await layoutItem.Action.Do();
                layoutItem.IsRunning = false;
                System.Diagnostics.Debug.WriteLine($"Finished Action Do {layout} {itemIndex}");
            });
            System.Diagnostics.Debug.WriteLine($"Finished {layout} {itemIndex}");
        }

        public void SetActiveLayout(string layout)
        {
            _ui.SelectedLayout = layout;
            UpdateUiModel();
            SendUiModel();
        }



        private void UpdateUiModel()
        {

            var ui = new Ui();

       
            foreach (var layout in _layouts.AllLayouts)
            {
                ui.Headings.Add(new Heading { Id = layout.LayoutId, Title = layout.Title });
                if (_ui?.SelectedLayout == layout.LayoutId || 
                    ( _ui == null && layout.LayoutId == _layouts.AllLayouts.FirstOrDefault()?.LayoutId))
                {
                    ui.SelectedLayout = layout.LayoutId;
                    foreach (var item in layout.Items)
                    {
                        ui.Items.Add(new Models.LayoutItem() { Icon = item.Icon, IconRunning = "aftereffects.png", Text = item.Text });
                    }
                }
            }
            ui.Items.Add(new Models.LayoutItem() { Icon = "aftereffects.png", Text = DateTime.Now.ToLongTimeString() });

            _ui = ui;
        }
    }
}
