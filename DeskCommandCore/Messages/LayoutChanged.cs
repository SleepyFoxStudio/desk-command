using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace DeskCommandCore.Messages
{
    public class LayoutChanged : INotification
    {
        public string LayoutId { get; set; }
    }
}
