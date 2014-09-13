using mealaroni_ordering.Common;
using mealaroni_ordering.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace mealaroni_ordering
{
    
    public sealed partial class itemdetailsPage : Page
    {


        public apiMealaroni mealaroniApi = new apiMealaroni();
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");


        public itemdetailsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            
        }


        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }


        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            apiMealaroni menuItem = (apiMealaroni)e.NavigationParameter;
            BizName.Text = menuItem.selectedBiz.name+": "+menuItem.selectedItem.menu;
            HubSection overview = new HubSection();
            overview.ContentTemplate = (DataTemplate)this.Resources["ItemTemplate"];
            overview.Header = menuItem.selectedItem.name;
            overview.HeaderTemplate = (DataTemplate)this.Resources["HubSectionHeaderTemplate"];
            itemHub.Sections.Add(overview);
            overview.DataContext = menuItem.selectedItem;
            if (menuItem.selectedItem.options != null)
            {
                foreach (IXmlNode option in menuItem.selectedItem.options.ChildNodes)
                {
                    HubSection optionHub = new HubSection();
                    optionHub.HeaderTemplate = (DataTemplate)this.Resources["HubSectionHeaderTemplate"];
                    optionHub.ContentTemplate = (DataTemplate)this.Resources["OptionsTemplate"];

                    optionHub.Header = option.NodeName.Replace("_", " ");
                    itemHub.Sections.Add(optionHub);
                    IXmlNode encode = option.Attributes.GetNamedItem("enCode");
                    IXmlNode limitNode = option.Attributes.GetNamedItem("limit");
                    string limit = limitNode != null ? limitNode.NodeValue.ToString() : "";
                    string en = encode != null ? encode.NodeValue.ToString() : "";
                    optionHub.DataContext = menuItem.menuOptions.getOptions(option.NodeName, en, limit).ToList();
                }
            }

        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            
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

        private void Item_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            StackPanel templateRoot = (StackPanel)args.ItemContainer.ContentTemplateRoot;
            args.RegisterUpdateCallback(ShowItemDetails);
        }

        private void ShowItemDetails(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 1)
            {
                var h = args.Item;
               
            }
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void ItemTemplate_ContexChange(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            StackPanel templateRoot = (StackPanel)args.ItemContainer.ContentTemplateRoot;
            
        }

        private void StackPanel_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            StackPanel sp = (StackPanel)sender;
            TextBlock a = (TextBlock)sp.Children[0];
            TextBlock b = (TextBlock)sp.Children[1];

            a.Text = ((apiMealaroni.MenuItem)args.NewValue).price;
            b.Text = ((apiMealaroni.MenuItem)args.NewValue).decription;
            
        }

        private void ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            StackPanel root = (StackPanel)args.ItemContainer.ContentTemplateRoot;
            args.RegisterUpdateCallback(showOtionData);
        }

        private void showOtionData(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.Phase == 1)
            {
                StackPanel root = (StackPanel)args.ItemContainer.ContentTemplateRoot;
                IXmlNode option = (IXmlNode)args.Item;
                string limit = option.Attributes.GetNamedItem("limit").NodeValue.ToString();
                if (limit == "limit_one")
                {
                    ((RadioButton)root.FindName("rdbx")).Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ((RadioButton)root.FindName("rdbx")).GroupName = option.NodeName;
                    ((RadioButton)root.FindName("rdbx")).Content = option.Attributes.GetNamedItem("name").NodeValue.ToString();
                }
                else
                {
                    ((CheckBox)root.FindName("cbx")).Visibility = Windows.UI.Xaml.Visibility.Visible;
                    ((CheckBox)root.FindName("cbx")).Content = option.Attributes.GetNamedItem("name").NodeValue.ToString();
                }
                

                string price = option.Attributes.GetNamedItem("price").NodeValue.ToString();
                decimal price_DEC  = 0;
                decimal.TryParse(price, out price_DEC);
                CultureInfo ci = new CultureInfo("en-us");

                price = price == "0" ? "No Added Charge" : price_DEC.ToString("C", ci);

                ((TextBlock)root.FindName("price")).Text = price;


            }
        }      
    }
}
