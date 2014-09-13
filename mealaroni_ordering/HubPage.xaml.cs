using mealaroni_ordering.Common;
using mealaroni_ordering.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Data.Json;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml.Shapes;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace mealaroni_ordering
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        public SampleDataSource ssd = new SampleDataSource(); 
        private readonly NavigationHelper navigationHelper;
        public apiMealaroni mealaroniApi = new apiMealaroni();
        public List<apiMealaroni.BizItem> bizlisting = new List<apiMealaroni.BizItem>();
        
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");



        public HubPage()
        {
            this.InitializeComponent();      
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.Loaded += HubPage_Loaded;
        }

        void HubPage_Loaded(object sender, RoutedEventArgs e)
        {
            bool loaded = true;
        }
     


        async  private Task loadBizList()
        {
            if (bizlisting.Count == 0)
            {
                bizlisting = await mealaroniApi.getBizList("memphis", "tn", "usa");
                bizList.Header = "MEMPHIS,TN";
                bizList.DataContext = bizlisting;
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


        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (App.Context.mealaroniAppApi != null)
            {
                string searchKey = App.Context.searchTerm;

                bizList.DataContext = null;
                //bizlisting.Clear();
             
                mealaroniApi = App.Context.mealaroniAppApi;
                bizList.Header = mealaroniApi.locationPicker.city.ToUpper() + "," + mealaroniApi.locationPicker.state.ToUpper();

                bizlisting = mealaroniApi.bizItems.Where(fd => fd.name.ToLower().Contains(searchKey) || searchKey=="all" ).ToList();
                bizList.DataContext = bizlisting;
                App.Context.mealaroniAppApi = null;
                return;

            }
            await loadBizList();
        }


        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        private void GroupItem_ItemClick(object sender, ItemClickEventArgs e)
        {
            
            StackPanel elem = ((StackPanel)e.ClickedItem);
            ListView dsd = (ListView)((StackPanel)e.ClickedItem).Parent;
            GroupSection_ItemClick((apiMealaroni.BizItem)dsd.ItemsSource);
            
                     
        }
        private void GroupSection_ItemClick(apiMealaroni.BizItem e)
        {
            var country = e.country;
            var state = e.state;
            var city = e.city;
            var namekey = e.namekey;
            var phone = e.phone;
            mealaroniApi.selectedBiz = e;
            if (!Frame.Navigate(typeof(SectionPage), mealaroniApi))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }
        private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
        {
            var country = ((apiMealaroni.BizItem)e.ClickedItem).country;
            var state = ((apiMealaroni.BizItem)e.ClickedItem).state;
            var city = ((apiMealaroni.BizItem)e.ClickedItem).city;
            var namekey = ((apiMealaroni.BizItem)e.ClickedItem).namekey;
            var phone = ((apiMealaroni.BizItem)e.ClickedItem).phone;
            
                     
        }
        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            if (!Frame.Navigate(typeof(ItemPage), itemId))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
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
        private void loadMap_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (!Frame.Navigate(typeof(mapPage), mealaroniApi))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));

            }
        }
        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
             
            ListView templateRoot = (ListView)args.ItemContainer.ContentTemplateRoot;
             TextBlock ss1 = (TextBlock)templateRoot.FindName("templateTitle1");
             TextBlock ss2 = (TextBlock)templateRoot.FindName("templateTitle2");
             TextBlock ss3 = (TextBlock)templateRoot.FindName("templateTitle3");
             ss1.Opacity = ss2.Opacity = ss3.Opacity = 0;
            args.RegisterUpdateCallback(ShowText);
        }
        private void ShowText(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 1)
            {
                apiMealaroni.BizItem dd = (apiMealaroni.BizItem)args.Item;
                ListView templateRoot = (ListView)args.ItemContainer.ContentTemplateRoot;                
                TextBlock ss1 = (TextBlock)templateRoot.FindName("templateTitle1");
                ss1.Text = dd.name.ToUpper();
                ss1.Opacity =  1;                
                args.RegisterUpdateCallback(ShowText2);
            }
        }
        private void ShowText2(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 2)
            {
                apiMealaroni.BizItem dd = (apiMealaroni.BizItem)args.Item;
                ListView templateRoot = (ListView)args.ItemContainer.ContentTemplateRoot;
                TextBlock ss2 = (TextBlock)templateRoot.FindName("templateTitle2");
                ss2.Text = dd.address;
                ss2.Opacity = 1;
                args.RegisterUpdateCallback(ShowText3);
            }
        }
        private void ShowText3(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 3)
            {
                apiMealaroni.BizItem dd = (apiMealaroni.BizItem)args.Item;
                ListView templateRoot = (ListView)args.ItemContainer.ContentTemplateRoot;       
                TextBlock ss3 = (TextBlock)templateRoot.FindName("templateTitle3");
                ss3.Opacity = 1;
                ss3.Text = dd.phone;
                StackPanel stack = (StackPanel)templateRoot.FindName("bizStack");
            }
        }

        private void init_picker(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            ComboBox comb_countries = (ComboBox)((ListView)sender).FindName("comb_countries");
            args.RegisterUpdateCallback(init_picker1);                        
        }
        async private void init_picker1(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 1)
            {

                if (mealaroniApi.locationPicker.Countries.Count == 0)
                {
                    mealaroniApi.locationPicker = await mealaroniApi.getCities("memphis", "tn", "usa");
                    picker_panel_loc.DataContext = mealaroniApi.locationPicker;
                }
                
                ComboBox comb_countries = (ComboBox)((ListView)sender).FindName("comb_countries");
                ComboBox comb_cities = (ComboBox)((ListView)sender).FindName("comb_cities");
                ComboBox comb_states = (ComboBox)((ListView)sender).FindName("comb_states");

                try
                {
                    comb_countries.Items.Clear();
                    comb_cities.Items.Clear();
                    comb_states.Items.Clear();
                }
                catch { }

                comb_countries.ItemsSource = mealaroniApi.locationPicker.Countries;
                comb_states.ItemsSource = mealaroniApi.locationPicker.States;
                comb_cities.ItemsSource = mealaroniApi.locationPicker.Cities;
                comb_countries.SelectedValue = mealaroniApi.locationPicker.country;
                comb_states.SelectedValue = mealaroniApi.locationPicker.state;
                comb_cities.SelectedValue = mealaroniApi.locationPicker.city;
            }
        }
        
        private void comb_states_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                mealaroniApi.locationPicker.Cities = new List<string>();
                picker_panel_loc.DataContext = null;

                StackPanel stack = (StackPanel)((ComboBox)sender).Parent;
                ListView lv = (ListView)stack.Parent;
                ComboBox comb_countries = (ComboBox)lv.FindName("comb_countries");
                ComboBox comb_cities = (ComboBox)lv.FindName("comb_cities");
                ComboBox comb_states = (ComboBox)lv.FindName("comb_states");
                if (comb_states.SelectedValue.ToString() == mealaroniApi.locationPicker.state) { return; }
                picker_panel_loc.DataContext = null;

                string city = comb_cities.SelectedValue.ToString();
                string state = comb_states.SelectedValue.ToString();
                string country = comb_countries.SelectedValue.ToString();


                Action<IXmlNode, List<string>> fillList = (IXmlNode inNode, List<string> outList) =>
                {
                    foreach (IXmlNode nd in inNode.SelectNodes("option"))
                    {
                        if (nd.InnerText.ToLower().Contains("select city") == false)
                        {
                            outList.Add(nd.InnerText);
                        }
                    }
                };


                IXmlNode srcNode = mealaroniApi.locationPicker.provs.SelectSingleNode("select[@id='" + state.ToUpper() + "']/select[@id='sel_cities']");

                fillList(srcNode, mealaroniApi.locationPicker.Cities);
                mealaroniApi.locationPicker.state = state;
                mealaroniApi.locationPicker.city = mealaroniApi.locationPicker.Cities[1].ToString();

                picker_panel_loc.DataContext = mealaroniApi.locationPicker;
                comb_cities.ItemsSource = mealaroniApi.locationPicker.Cities;
                comb_cities.SelectedValue = mealaroniApi.locationPicker.Cities[1].ToString();
            }
            catch { }
        }

       async private void comb_city_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          

            
            StackPanel stack = (StackPanel)((ComboBox)sender).Parent;
            ComboBox comb_countries = (ComboBox)stack.FindName("comb_countries");
            ComboBox comb_cities = (ComboBox)stack.FindName("comb_cities");
            ComboBox comb_states = (ComboBox)stack.FindName("comb_states");

            if (comb_cities.SelectedValue == null) {  }

            else if (comb_cities.SelectedValue.ToString() == mealaroniApi.locationPicker.city) { return; }
            bizList.DataContext = null;
            bizlisting.Clear();


            string city = comb_cities.SelectedValue == null ? mealaroniApi.locationPicker.city : comb_cities.SelectedValue.ToString();
            string state = comb_states.SelectedValue.ToString();
            string country = comb_countries.SelectedValue.ToString();
            mealaroniApi.locationPicker.city = city;
            List<apiMealaroni.BizItem> list = await mealaroniApi.getBizList(city, state, country);
            bizList.DataContext = list;
            bizList.Header = city.ToUpper() + ", " + state.ToUpper();
            Hub.ScrollToSection(bizList);
        }

       private void load_profile(object sender, RoutedEventArgs e)
       {
           if (!Frame.Navigate(typeof(profilePage)))
           {
               throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
           }
       }

       private void find_Click(object sender, RoutedEventArgs e)
       {
           if (!Frame.Navigate(typeof(searchPage), mealaroniApi))
           {
               throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
           }
       }

       
    }
}
