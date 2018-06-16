using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;


namespace DeskCommandCore
{

    public class Layouts : List<Layout>
    {
    }

    public class Layout
    {
        [JsonProperty("layoutId")]
        public string LayoutId { get; set; }

        //[JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("items")]
        public List<LayoutItem> Items { get; set; } = new List<LayoutItem>();

    }

    public class LayoutItem : INotifyPropertyChanged
    {
        private bool _isRunning;
        public string Icon { get; set; }
        public string IconRunning { get; set; }
        public string Text { get; set; }

        //[DataMember(Name = "action", Order = 1)]
        public InterfaceAction Action { get; set; }
        public bool IsRunning
        {
            get => _isRunning;
            set
            {
                if (value == _isRunning) return;
                _isRunning = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    }





}
