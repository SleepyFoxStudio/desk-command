using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

    public class LayoutChangedEventArgs : EventArgs
    {
        
    }

    public class Layout
    {
        private readonly ObservableCollection<LayoutItem> _items = new ObservableCollection<LayoutItem>();
        private readonly List<LayoutItem> _backingItems = new List<LayoutItem>();

        public Layout()
        {
            _items.CollectionChanged += _items_CollectionChanged;
        }

        private void _items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (LayoutItem item in e.NewItems)
                    {
                        item.PropertyChanged += ItemOnPropertyChanged;
                        _backingItems.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (LayoutItem item in e.OldItems)
                    {
                        item.PropertyChanged -= ItemOnPropertyChanged;
                        _backingItems.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    foreach (LayoutItem item in e.NewItems)
                    {
                        item.PropertyChanged += ItemOnPropertyChanged;
                        _backingItems.Add(item);
                    }
                    foreach (LayoutItem item in e.OldItems)
                    {
                        item.PropertyChanged -= ItemOnPropertyChanged;
                        _backingItems.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (LayoutItem item in _backingItems)
                    {
                        item.PropertyChanged -= ItemOnPropertyChanged;
                    }
                    _backingItems.Clear();
                    foreach (LayoutItem item in _items)
                    {
                        item.PropertyChanged += ItemOnPropertyChanged;
                        _backingItems.Add(item);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ItemOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnLayoutChanged();
        }

        [JsonProperty("layoutId")]
        public string LayoutId { get; set; }

        //[JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("items")]
        public IList<LayoutItem> Items => _items;


        public event EventHandler<LayoutChangedEventArgs> LayoutChanged;
        private void OnLayoutChanged()
        {
            LayoutChanged?.Invoke(this, new LayoutChangedEventArgs());
        }
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
