using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeskCommandCore.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeskCommandEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public static List<LayoutConfig> _Layouts;
        public static string DeskCommandHome = "D:\\Code\\Github\\desk-command\\DeskCommandCore\\";

        public MainWindow()
        {
            InitializeComponent();
            LoadLayoutsFromDisk();
            RefreshDropdown();
            LayoutDropdown.SelectedIndex = 0;
            LoadPluginList();
        }

        private void LoadPluginList()
        {







            var assemblyPath = System.IO.Path.Combine(DeskCommandHome, "bin\\Debug\\net471\\DeskCommandCore.exe");
            Assembly testAssembly = Assembly.LoadFile(assemblyPath);


            var plugins =
                testAssembly
                  .GetTypes()  // Gets all types
//                  .Where(type => type.IsInstanceOfType(typeof(DeskCommandCore.InterfaceAction))) // Ensures that object can be cast to interface
                  .Where(type =>
                      !type.IsAbstract &&
                      !type.IsGenericType)
                  .Select(d => d)
                  .ToList();





            foreach (var plugin in plugins)
            {
                var listViewItem = new ListViewItem();
                if (plugin.FullName.ToLower().Contains("action"))
                {
                    listViewItem.Content = plugin.Name;
                    Plugins.Items.Add(listViewItem);
                }
            }

        }

        private void RefreshDropdown()
        {

            var configPath = System.IO.Path.Combine(DeskCommandHome, "appsettings.json");

            JObject configJObject = JObject.Parse(File.ReadAllText(configPath));
            // get JSON result objects into a list
            var results = configJObject["Layouts"].ToString();

            var layoutList = JsonConvert.DeserializeObject<List<Layout>>(results);
            var itemCount = 0;
            foreach (var layoutItem in layoutList)
            {

                var listItem = new ListBoxItem();
                listItem.Content = layoutItem.Title;
                LayoutDropdown.Items.Add(listItem);
                itemCount++;
            }
        }



        private void LoadLayoutsFromDisk()
        {
            var configPath = System.IO.Path.Combine(DeskCommandHome, "appsettings.json");

            JObject configJObject = JObject.Parse(File.ReadAllText(configPath));
            // get JSON result objects into a list
            var results = configJObject["Layouts"].ToString();

            _Layouts = JsonConvert.DeserializeObject<List<LayoutConfig>>(results);
        }


        private void RefreshIcons()
        {
            RemoveAllIcons();
            var layoutItemSelcted = LayoutDropdown.SelectedIndex;
            var layoutItems = _Layouts[layoutItemSelcted];
            foreach (var layoutItem in layoutItems.Items)
            {
                AddIcon(layoutItem.Icon);
            }
        }

        private void AddIcon(string imageName)
        {
            var image = new Image();
            var imageSource =
                new BitmapImage(new Uri(
                    System.IO.Path.Combine(DeskCommandHome, "wwwroot\\Icons\\",
                        imageName)));
            image.Source = imageSource;
            IconListControl.Items.Add(image);
        }

        private void LayoutDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RemoveAllIcons();
            RefreshIcons();
        }

        private void RemoveAllIcons()
        {
            IconListControl.Items.Clear();
        }

        private void IconListControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var listBox = (ListBox)sender;
            if (listBox.SelectedIndex < 0)
            {
                return;
            }

            LayoutItemConfig selectedItem = _Layouts[LayoutDropdown.SelectedIndex].Items[listBox.SelectedIndex];

            ArgumentGrid.Children.Clear();


            var assemblyPath = System.IO.Path.Combine(DeskCommandHome, "bin\\Debug\\net471\\DeskCommandCore.exe");
            Assembly testAssembly = Assembly.LoadFile(assemblyPath);
            var action = testAssembly.GetType(selectedItem.Action);
            var constructor = action.GetConstructors();

            AddHeading("Action", action.Name, 0);

            AddImage("Icon", selectedItem.Icon, 1);
            AddImage("Icon Running", selectedItem.IconRunning, 2);

            int rowCount = 3;
            var firstConstructor = constructor[0];
            int paramCount = 0;
            foreach (var t in firstConstructor.GetParameters())
            {
                var argValue = selectedItem.Arguments[paramCount];

                AddTextBox(t.Name, argValue, rowCount);
                rowCount++;
                paramCount++;
            }
        }

        private void AddHeading(string heading, string value, int index)
        {
            var label = new Label
            {
                Content = heading
            };
            Grid.SetRow(label, index);
            Grid.SetColumn(label, 0);

            var content2 = new Label
            {
                Content = value
            };
            Grid.SetRow(content2, index);
            Grid.SetColumn(content2, 1);

            ArgumentGrid.Children.Add(label);
            ArgumentGrid.Children.Add(content2);

        }
        private void AddTextBox(string heading, string value, int index)
        {
            var label = new Label
            {
                Content = heading
            };
            Grid.SetRow(label, index);
            Grid.SetColumn(label, 0);

            var txtBox = new TextBox
            {
                Text = value
            };
            Grid.SetRow(txtBox, index);
            Grid.SetColumn(txtBox, 1);

            ArgumentGrid.Children.Add(label);
            ArgumentGrid.Children.Add(txtBox);

        }

        private void AddImage(string heading, string value, int index)
        {
            var label = new Label
            {
                Content = heading
            };
            Grid.SetRow(label, index);
            Grid.SetColumn(label, 0);




            var image = new Image();

            if (value == null)
            {
                value = "upload.png";
            }

            var imageSource =
                new BitmapImage(new Uri(
                    System.IO.Path.Combine(DeskCommandHome, "wwwroot\\Icons\\",
                        value)));
            image.Source = imageSource;
            image.Height = 100;
            image.Width = 100;


            Grid.SetRow(image, index);
            Grid.SetColumn(image, 1);

            ArgumentGrid.Children.Add(label);
           ArgumentGrid.Children.Add(image);

        }
    }
}
