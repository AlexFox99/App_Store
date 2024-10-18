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

        /*Функция ContentFrame_NavigationFailed необхадима для предотвращения ошибок навигации
             * и коректной работы приложения*/
        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            
        }

        /*Функция NavView_ItemInvoked предназначена словить момент выбора какой элемент NavigationView.MenuItems
         был нажат и запуска функции NavigateToView с тэгом элемента*/
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

        /*Функция NavigateToView предназначена вернуть ложь в случае если элемент ужу отображен,
         * иначе контент будет отображен и вернется правда*/
        private bool NavigateToView(string clickedView)
        {
            var view = Assembly.GetExecutingAssembly().GetType($"App_Store.View.{clickedView}");
            if (string.IsNullOrWhiteSpace(clickedView) || view == null)
                return false;
            ContentFrame.Navigate(view, null, new EntranceNavigationTransitionInfo());
            return true;
        }

        /*Функция NavView_Loaded предназначена выставить изначально выбраным страницу магазина*/
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
            settings.Content = "О приложении";
            /*Отображение StoreView*/
            ContentFrame.Navigate(typeof(App_Store.View.StoreView));
        }

        /*Функция NavView_SelectionChanged нужна для просмотра изменения выделенного фрагмента. */
        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {

        }

    }
}
