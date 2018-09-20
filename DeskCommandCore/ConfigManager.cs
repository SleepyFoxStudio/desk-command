using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DeskCommandCore.Models;

namespace DeskCommandCore
{
    internal class ConfigManager
    {
        public Layouts ReadConfig(LayoutsConfig config)
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
                        IconRunning = itemConfig.IconRunning,
                        Text = itemConfig.Text
                    };

                    var actionType = Assembly.GetExecutingAssembly().GetType(itemConfig.Action);
                    var action = (InterfaceAction)Activator.CreateInstance(actionType, itemConfig.Arguments);
                    layoutItem.Action = action;
                    layout.Items.Add(layoutItem);
                }
                layouts.AllLayouts.Add(layout);
            }
            return layouts;
        }
    }
}
