using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DeskCommandCore.Messages;
using DeskCommandCore.Models;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace DeskCommandCore
{
    public class ClientUi
    {
        public ClientUi(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
        public string ActiveLayout { get; set; }
    }

    public class ClientUiManager : INotificationHandler<LayoutChanged>
    {
        private readonly Layouts _layouts;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly Dictionary<string, ClientUi> _clients = new Dictionary<string, ClientUi>();

        public ClientUiManager(Layouts layouts, IHubContext<ChatHub> hubContext)
        {
            _layouts = layouts;
            _hubContext = hubContext;
        }

        //private void LayoutOnLayoutChanged(object sender, LayoutChangedEventArgs layoutChangedEventArgs)
        //{
        //    var layout = sender as Layout;
        //    if (layout == null) return;

        //    var clientsToUpdate = _clients.Values.Where(c => c.ActiveLayout == layout.LayoutId);
        //    foreach (var client in clientsToUpdate)
        //    {
        //        PushClientLayout(client.ConnectionId);
        //    }
        //}

        public void AddClient(string connectionId)
        {
            _clients.Add(connectionId, new ClientUi(connectionId));
        }

        public void RemoveClient(string connectionId)
        {
            _clients.Remove(connectionId);
        }

        public Task ChangeClientLayout(string connectionId, string layoutId, CancellationToken cancellationToken)
        {
            _clients[connectionId].ActiveLayout = layoutId;
            return PushClientLayout(connectionId, cancellationToken);
        }

        private Task PushClientLayout(string connectionId, CancellationToken cancellationToken)
        {
            var selectedLayout = _layouts.Single(l => l.LayoutId == _clients[connectionId].ActiveLayout);
            var ui = new Ui
            {
                Headings = _layouts.Select(h => new Heading
                {
                    Id = h.LayoutId,
                    Title = h.Title
                }).ToList(),
                SelectedLayout = selectedLayout.LayoutId,
                Title = selectedLayout.Title,
                Items = selectedLayout.Items.Select(i => new Models.LayoutItem
                {
                    Icon = i.IsRunning ? i.IconRunning : i.Icon,
                    Text = i.Text
                }).ToList()
            };

            return _hubContext.Clients.Client(connectionId).SendAsync("ReceiveUi", ui, cancellationToken);
        }

        public Task Handle(LayoutChanged notification, CancellationToken cancellationToken)
        {
            return Task.WhenAll(_clients.Keys.Select(c => PushClientLayout(c, cancellationToken)));
        }
    }
}
