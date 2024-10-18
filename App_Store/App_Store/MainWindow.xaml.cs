using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using static System.Formats.Asn1.AsnWriter;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace App_Store
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private NavigationViewItem _lastItem;

        public MainWindow()
        {
            this.InitializeComponent();
        }

        /*������� ContentFrame_NavigationFailed ���������� ��� �������������� ������ ���������
             * � ��������� ������ ����������*/
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            
        }

        /*������� NavView_ItemInvoked ������������� ������� ������ ������ ����� ������� NavigationView.MenuItems
         ��� ����� � ������� ������� NavigateToView � ����� ��������*/
        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var item = args.InvokedItemContainer as NavigationViewItem;
            if (item == null || item == _lastItem)
                return;
            var clickedView = item.Tag?.ToString();
            if (clickedView == "Settings") clickedView = "SettingsView";
            if (!NavigateToView(clickedView)) return;
            _lastItem = item;
        }

        /*������� NavigateToView ������������� ������� ���� � ������ ���� ������� ��� ���������,
         * ����� ������� ����� ��������� � �������� ������*/
        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"App_Store.View.{clickedView}");
            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                return false;
            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        /*������� NavView_Loaded ������������� ��������� ���������� �������� �������� ��������*/
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItemBase && item.Tag.ToString() == "StoreView")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }
            var settings = (NavigationViewItem)NavView.SettingsItem;
            settings.Content = "� ����������";
            /*����������� StoreView*/
            ContentFrame.Navigate(typeof(App_Store.View.StoreView));
        }

        /*������� NavView_SelectionChanged ����� ��� ��������� ��������� ����������� ���������. */
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

        }

    }
}
