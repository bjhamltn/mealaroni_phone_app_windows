using mealaroni_ordering.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace mealaroni_ordering
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class profilePage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public profilePage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        async private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            try
            {
                Windows.Storage.StorageFile file = null;
                Windows.Storage.StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                file = await local.GetFileAsync("profile.xml");

                Stream fStream = await file.OpenStreamForReadAsync();
                StreamReader sr = new StreamReader(fStream);
                string result2 = sr.ReadToEnd();
                sr.Dispose();
                fStream.Dispose();
                XmlDocument profile = new XmlDocument();
                profile.LoadXml(result2);
                input_email.Text = profile.DocumentElement.SelectSingleNode("email").InnerText;
                input_username.Text = profile.DocumentElement.SelectSingleNode("username").InnerText;
                input_phone.Text = profile.DocumentElement.SelectSingleNode("phone").InnerText;


            }
            catch
            {
              
            }

        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        async private void save_click(object sender, RoutedEventArgs e)
        {
            try
            {
                Windows.Storage.StorageFile file = null;
                Windows.Storage.StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml("<profile/>");
                XmlElement userNode = xmlDoc.CreateElement("username");
                XmlElement emailNode = xmlDoc.CreateElement("email");
                XmlElement phoneNode = xmlDoc.CreateElement("phone");

                XmlCDataSection userName = xmlDoc.CreateCDataSection(input_username.Text);
                XmlCDataSection email = xmlDoc.CreateCDataSection(input_email.Text);
                XmlCDataSection phone = xmlDoc.CreateCDataSection(input_phone.Text);

                userNode.AppendChild(userName);
                emailNode.AppendChild(email);
                phoneNode.AppendChild(phone);

                xmlDoc.DocumentElement.AppendChild(userNode);
                xmlDoc.DocumentElement.AppendChild(emailNode);
                xmlDoc.DocumentElement.AppendChild(phoneNode);
                bool createFile = false;
                try
                {
                    file = await local.GetFileAsync("profile.xml");
                }
                catch
                {
                    createFile = true;
                }
                if(createFile)
                {
                    file = await local.CreateFileAsync("profile.xml");
                }
                await xmlDoc.SaveToFileAsync(file);                               
            }
            catch { }
            this.navigationHelper.GoBack();
        }
    }
}
