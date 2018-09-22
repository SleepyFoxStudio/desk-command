using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace DeskCommandCore
{
    public class ConfigurationMonitor
    {
        private readonly IOptionsMonitor<Models.Config.Layouts> _monitor;
        private readonly LayoutProvider _layoutProvider;

        public ConfigurationMonitor(IOptionsMonitor<Models.Config.Layouts> monitor, LayoutProvider layoutProvider)
        {
            _monitor = monitor;
            _layoutProvider = layoutProvider;
            _monitor.OnChange(config => InvokeChanged(config).Wait());
        }

        private Task InvokeChanged(Models.Config.Layouts config)
        {
            var layouts = BuildLayouts(config);
            return _layoutProvider.UpdateLayouts(layouts);
        }

        private static List<Layout> BuildLayouts(Models.Config.Layouts config)
        {
            return new List<Layout>(config.Select(layoutConfig => new Layout
            {
                LayoutId = layoutConfig.LayoutId,
                Title = layoutConfig.Title,
                Items = layoutConfig.Items.Select(i => new LayoutItem
                {
                    Icon = i.Icon,
                    IconRunning = i.IconRunning,
                    Text = i.Text,
                    Action = BuildAction(i)
                }).ToList()
            }));
        }

        private static InterfaceAction BuildAction(Models.Config.LayoutItem itemConfig)
        {
            var actionType = Assembly.GetExecutingAssembly().GetType(itemConfig.Action);
            var action = (InterfaceAction)Activator.CreateInstance(actionType, itemConfig.Arguments);
            return action;
        }
    }
}