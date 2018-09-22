using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Navigation;
using AutoMapper;
using DeskCommandCore.Models;
using Microsoft.AspNetCore.SignalR;

namespace DeskCommandCore
{
    public class ChatHub : Hub
    {
        private readonly ClientUiManager _clientUiManager;
        private readonly List<Layout> _layouts;

        public ChatHub(Layouts layouts, ClientUiManager clientUiManager)
        {
            _clientUiManager = clientUiManager;
            _layouts = layouts.AllLayouts;
        }

        public override Task OnConnectedAsync()
        {
            _clientUiManager.AddClient(Context.ConnectionId);
            _clientUiManager.ChangeClientLayout(Context.ConnectionId, _layouts.First().LayoutId);
            return base.OnConnectedAsync();
        }

        public void DoAction(string layout, int itemIndex)
        {
            System.Diagnostics.Debug.WriteLine($"Starting {layout} {itemIndex}");
            var layoutItem = _layouts.Find(t => t.LayoutId == layout).Items[itemIndex];

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
            _clientUiManager.ChangeClientLayout(Context.ConnectionId, layout);
        }
    }
}
