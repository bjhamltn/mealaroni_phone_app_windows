using mealaroni_ordering.Common;
using mealaroni_ordering.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace mealaroni_ordering
{
    public sealed partial class SectionPage : Page
    {
        public apiMealaroni mealaroniApi = new apiMealaroni();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public SectionPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        private void load_profile(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(profilePage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        
        async private  void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            mealaroniApi = (apiMealaroni)e.NavigationParameter;                        
            BizName.Text = mealaroniApi.selectedBiz.name;
            await mealaroniApi.getMenu(mealaroniApi.selectedBiz.city, mealaroniApi.selectedBiz.state, mealaroniApi.selectedBiz.country, mealaroniApi.selectedBiz.phone);
            mealaroniApi.selectedBiz.storehours = mealaroniApi.menus[0].storehours;
            foreach(apiMealaroni.Menu menu in  mealaroniApi.menus)
            {
                string menuName = menu.menu;

                HubSection menusection = new HubSection();
                menusection.Header = menuName;
                menusection.Margin = MenuItemListView.Margin;
                menusection.ContentTemplate =  (DataTemplate)this.Resources["MenuTemplate"];
                menusection.HeaderTemplate = (DataTemplate)this.Resources["HubSectionHeaderTemplate"];
                MenuHub.Sections.Add(menusection);
                menusection.DataContext = mealaroniApi.menuItems.Where(fd => fd.menu == menuName).ToList();
                
            }
            MenuItemListView.Header = "Full Menu";
            MenuItemListView.DataContext = mealaroniApi.menuItems;            
        }

        
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            mealaroniApi.selectedItem = (apiMealaroni.MenuItem)e.ClickedItem;
            if (!Frame.Navigate(typeof(itemdetailsPage), mealaroniApi ))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
            return;        
        }

        #region NavigationHelper registration

        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            Grid templateRoot = (Grid)args.ItemContainer.ContentTemplateRoot;
            args.RegisterUpdateCallback(ShowText);
        }

        private void ShowText(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 1)
            {
                apiMealaroni.MenuItem dd = (apiMealaroni.MenuItem)args.Item;
                Grid templateRoot = (Grid)args.ItemContainer.ContentTemplateRoot;
                TextBlock ss1 = (TextBlock)templateRoot.FindName("Title1");
                TextBlock ss2 = (TextBlock)templateRoot.FindName("Title2");
                TextBlock ss3 = (TextBlock)templateRoot.FindName("Title3");
                ss1.Text = dd.name;
                ss2.Text = dd.price;
                ss3.Text = dd.decription;
                ss1.Opacity = ss2.Opacity = ss3.Opacity = 1;
            }
        }

        async private void get_directions_to(object sender, RoutedEventArgs e)
        {
            if (mealaroniApi.selectedBiz.geo != null)
            {
                string[] geo = mealaroniApi.selectedBiz.geo.Split(new char[] { ',' });
                if (geo.Length == 2)
                {
                    string uriLaunch = String.Format("ms-drive-to:?destination.latitude={0}&destination.longitude={1}&destination.name={2}", geo[0], geo[1], mealaroniApi.selectedBiz.name);
                    Uri uri = new Uri(uriLaunch);
                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                }
                else
                {
                    string uriLaunch = String.Format("bingmaps:?q={0}", mealaroniApi.selectedBiz.name);
                    Uri uri = new Uri(uriLaunch);
                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                }


            }
        }

        private void get_bizInfo(object sender, RoutedEventArgs e)
        {
            try { Frame.Navigate(typeof(bizInfoPage), mealaroniApi); }
            catch { }
        }
    }
}
