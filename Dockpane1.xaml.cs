using ArcGIS.Desktop.Mapping;
using ArcGIS.Desktop.Mapping.Events;
using ArcGIS.Desktop.Framework;
using ArcGIS.Desktop.Framework.Contracts;
using ArcGIS.Desktop.Framework.Dialogs;
using ArcGIS.Desktop.Framework.Threading.Tasks;
using ArcGIS.Desktop.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Diagnostics;

namespace RemoveGapsAndOverlaps
{
    /// <summary>
    /// Interaction logic for Dockpane1View.xaml
    /// </summary>
    public partial class Dockpane1View : UserControl
    {
        private const string _dockPaneID = "RemoveGapsAndOverlaps_Dockpane1";
        static private int cb_layer_count = 1;
        static private List<bool> _status_selected_layer = new List<bool>() { false};
        private bool _isInitialized;
        public Dockpane1View()
        {
            InitializeComponent();
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private void btn_close(object sender, RoutedEventArgs e)
        {
            var pane = FrameworkApplication.DockPaneManager.Find(_dockPaneID);
            if (pane != null) pane.Hide();
            if (cb_layer_count > 1)
            {
                _status_selected_layer = new List<bool>() { false };
                grid_layers.RowDefinitions.RemoveRange(1, cb_layer_count - 1);
                for (int i = grid_layers.Children.Count - 1; i >= 0; i--)
                {
                    UIElement child = grid_layers.Children[i];
                    if (Grid.GetRow(child) >= 1) grid_layers.Children.Remove(child);
                }
               
                cb_layer_count = 1;
            }
        
        }

        private void cb_layer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedLayer = e.AddedItems[0] as FeatureLayer;
            System.Windows.Controls.ComboBox cb = (System.Windows.Controls.ComboBox)sender;
            int index_cb = int.Parse(cb.Name.Split("_")[2]);
            if (_status_selected_layer[index_cb]== false && selectedLayer is not null)
            {
                _status_selected_layer[index_cb] = true;
                _status_selected_layer.Add(false);
                grid_layers.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

                System.Windows.Controls.ComboBox new_cb_layer = new System.Windows.Controls.ComboBox();
                new_cb_layer.Name = "cb_layer_" + cb_layer_count;
                new_cb_layer.Height = 25;
                new_cb_layer.Margin = new Thickness(10, 0, 30, 0);
                new_cb_layer.VerticalAlignment = VerticalAlignment.Top;
                new_cb_layer.SelectionChanged += cb_layer_SelectionChanged;
                new_cb_layer.MouseEnter += cb_layer_MouseEnter;
                Grid.SetRow(new_cb_layer, cb_layer_count);
                Grid.SetColumn(new_cb_layer, 0);
                grid_layers.Children.Add(new_cb_layer);

                System.Windows.Controls.ComboBox new_cb_priority = new System.Windows.Controls.ComboBox();
                new_cb_priority.Name = "cb_priority_" + cb_layer_count;
                new_cb_priority.Width = 40;
                new_cb_priority.Height = 25;
                new_cb_priority.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                new_cb_priority.Margin = new Thickness(25, 0, 0, 0);
                new_cb_priority.Items.Add("1");
                new_cb_priority.Items.Add("2");
                new_cb_priority.Items.Add("3");
                new_cb_priority.Items.Add("4");
                new_cb_priority.Items.Add("5");
                new_cb_priority.Items.Add("6");
                new_cb_priority.Items.Add("7");
                new_cb_priority.Items.Add("8");
                new_cb_priority.Items.Add("9");
                new_cb_priority.Items.Add("10");


                Grid.SetRow(new_cb_priority, cb_layer_count);
                Grid.SetColumn(new_cb_priority, 1);
                grid_layers.Children.Add(new_cb_priority);
                cb_layer_count++;
                grid_layers.UpdateLayout();
            }
           
          
        }

        private void tb_gap_tolerance_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"^\d+(\.\d+)?$");
            if (!regex.IsMatch(e.Text)){
                e.Handled = true;
            }
        }

      
     
        private List<FeatureLayer> get_all_feature_layer()
        {
            return MapView.Active.Map.Layers.Where(layer => layer is FeatureLayer).Cast<FeatureLayer>().ToList();
        }

        private void cb_layer_MouseEnter(object sender, MouseEventArgs e)
        {
            if (MapView.Active != null)
            {
                var featureLayers = get_all_feature_layer();
                System.Windows.Controls.ComboBox cb = (System.Windows.Controls.ComboBox)sender;
                cb.ItemsSource = featureLayers;
                cb.DisplayMemberPath = "Name";
            }
        }

        private void ckb_overlaps_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_gaps.IsChecked == false && ckb_overlaps.IsChecked == false)
            {
                btn_remove.IsEnabled = false;
            }
            else
            {
                btn_remove.IsEnabled = true;
            }

        }

        private void ckb_gaps_Click(object sender, RoutedEventArgs e)
        {
            if (ckb_gaps.IsChecked == false && ckb_overlaps.IsChecked == false)
            {
                btn_remove.IsEnabled = false;
            }
            else
            {
                btn_remove.IsEnabled = true;
            }
        }
    }

}
