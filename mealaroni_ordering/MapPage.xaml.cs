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
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Xaml.Media.Animation;
using Windows.Storage.FileProperties;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace mealaroni_ordering
{

    public sealed partial class mapPage : Page
    {

        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");
        private apiMealaroni mealaRoniApi = new apiMealaroni();
        private List<KeyValuePair<string, MapIcon>> titleMarkers = new List<KeyValuePair<string, MapIcon>>();

        public mapPage()
        {

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;
            this.NavigationCacheMode = NavigationCacheMode.Required;
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
            this.Loaded += mapPage_Loaded;            
            this.InitializeComponent();
            
            mapcontrol.ZoomLevel = 12;
            locateMe();


        }

        void mapcontrol_CenterChanged(MapControl sender, object args)
        {
            ClockState asa = gridHandleStroyBoardY.GetCurrentState();
            if (ClockState.Active != gridHandleStroyBoardY.GetCurrentState())
            {
                gridHandelY.Duration = new Duration(TimeSpan.FromMilliseconds(200));
                gridHandelY.To = -(infoPanel.ActualHeight + 20);
                gridHandleStroyBoardY.Begin();
                mapcontrol.CenterChanged -= mapcontrol_CenterChanged;
            }
        }

   

        async void locateMe()
        {
            Geolocator dd = new Geolocator();
            dd.DesiredAccuracy = PositionAccuracy.Default;
            try
            {
                Geoposition geoPos = await dd.GetGeopositionAsync();
                mapcontrol.ZoomLevel = 12;
                mapcontrol.Center = geoPos.Coordinate.Point;
            }
            catch { }
        }

        private void load_profile(object sender, RoutedEventArgs e)
        {
            if (!Frame.Navigate(typeof(profilePage)))
            {
                throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
            }
        }

        void mapPage_Loaded(object sender, RoutedEventArgs e)
        {
            mapcontrol.MapServiceToken = "P8ZDp3VZ-2ZwzbK-3-MJ0w";     
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            mealaRoniApi = (apiMealaroni)e.Parameter;
            this.navigationHelper.OnNavigatedTo(e);

            loadMarkers();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void appBar_myLocation_Click(object sender, RoutedEventArgs e)
        {
            locateMe();
        }

        async private void loadMarkers()
        {


            try
            {
                mapcontrol.MapElements.Clear();

                string geoListPath = String.Format("geo_{0}_{1}_{2}_.xml", mealaRoniApi.locationPicker.city, mealaRoniApi.locationPicker.state, mealaRoniApi.locationPicker.country);

                bool createFile = false;
                Windows.Storage.StorageFile markerFile = null;
                string result2 = "";
                Windows.Storage.StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
                try
                {
                    markerFile = await local.GetFileAsync(geoListPath);
                    BasicProperties s = await markerFile.GetBasicPropertiesAsync();
                    long f = DateTime.Now.Ticks - s.ItemDate.Ticks;
                    double age = TimeSpan.FromTicks(f).TotalMinutes;
                    if (age > 15)
                    {
                        createFile = true;
                        await markerFile.DeleteAsync();
                    }
                    else
                    {
                        Stream fStream = await markerFile.OpenStreamForReadAsync();
                        StreamReader sr = new StreamReader(fStream);
                        result2 = sr.ReadToEnd();
                        sr.Dispose();
                        fStream.Dispose();
                    }


                }
                catch
                {
                    createFile = true;
                }

                if (createFile)
                {
                    Task<string> result = mealaRoniApi.getGeoList(mealaRoniApi.locationPicker.city, mealaRoniApi.locationPicker.state, mealaRoniApi.locationPicker.country);
                    result2 = await result;
                    markerFile = await local.CreateFileAsync(geoListPath);
                    Stream fStream = await markerFile.OpenStreamForWriteAsync();
                    StreamWriter sw = new StreamWriter(fStream);
                    sw.Write(result2);
                    sw.Dispose();
                    fStream.Dispose();
                }


                Windows.Data.Json.JsonObject dd = Windows.Data.Json.JsonObject.Parse(result2);
                Windows.Data.Json.JsonArray geoObjects = dd["results"].GetArray();

                foreach (JsonValue geoValue in geoObjects)
                {
                    JsonObject geoObject = geoValue.GetObject();
                    BasicGeoposition geoPos = new BasicGeoposition();

                    string geo = geoObject["geo"].GetString();

                    geoPos.Latitude = double.Parse(geo.Split(new char[] { ',' })[0]);

                    geoPos.Longitude = double.Parse(geo.Split(new char[] { ',' })[1]);
                    Geopoint geoPoint = new Geopoint(geoPos);
                    string markerUrl = geoObject["marker"].GetString();
                    Uri imageUri = new Uri("http://mealaroni.com/images/" + markerUrl);
                    string status = geoObject["status"].GetString();
                    status = status == null ? "" : status;
                    status = status == "null" ? "" : status;
                    string title = geoObject["name"].GetString().ToUpper() + "\r\n" + geoObject["phone"].GetString() + "\r\n" + status;

                    if (App.Context.searchTerm == null || App.Context.searchTerm == "" || App.Context.searchTerm.ToLower() == "all")
                    {
                        
                    }
                    else if (title.ToLower().Contains(App.Context.searchTerm) == false)
                    {
                        continue;
                    }


                    createFile = false;
                    markerFile = null;
                    local = Windows.Storage.ApplicationData.Current.LocalFolder;
                    try
                    {
                        markerFile = await local.GetFileAsync(markerUrl);
                    }
                    catch
                    {
                        createFile = true;
                    }
                    if (createFile)
                    {
                        RandomAccessStreamReference image = RandomAccessStreamReference.CreateFromUri(imageUri);
                        IRandomAccessStreamWithContentType ss = await image.OpenReadAsync();
                        BinaryReader br = new BinaryReader(ss.AsStreamForRead());
                        byte[] buffer = new byte[ss.AsStream().Length];
                        buffer = br.ReadBytes(buffer.Length);
                        markerFile = await local.CreateFileAsync(markerUrl);
                        Stream fStream = await markerFile.OpenStreamForWriteAsync();
                        BinaryWriter bw = new BinaryWriter(fStream);
                        bw.Write(buffer);
                        bw.Dispose();
                    }
                    IRandomAccessStreamWithContentType fStream2 = await markerFile.OpenReadAsync();
                    RandomAccessStreamReference img = RandomAccessStreamReference.CreateFromStream(fStream2);
                    MapIcon marker = new MapIcon
                    {
                        Location = geoPoint,
                        Image = img,
                        NormalizedAnchorPoint = new Point(0.5, 0.5),
                    };

                    titleMarkers.Add(new KeyValuePair<string, MapIcon>(title, marker));

                    Image myButton = new Image
                    {
                        Width = 35,
                        Height = 35,
                        Source = new BitmapImage(new Uri(markerFile.Path)),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    mapcontrol.MapElements.Add(marker);
                }
            }
            catch (Exception dssd)
            {
                return;
            }
        }


        private void mapcontrol_MapTapped(MapControl sender, MapInputEventArgs args)
        {


            wavemoveX.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            wavemoveY.Duration = new Duration(TimeSpan.FromMilliseconds(200));
            wavemoveY.To = args.Position.Y - 25;
            wavemoveX.To = args.Position.X - 25;
    

            List<KeyValuePair<double, MapIcon>> results = new List<KeyValuePair<double, MapIcon>>();
            myStoryboardX.Begin();
            myStoryboardY.Begin();
            int cnt = 0;
            double k = mapcontrol.MaxZoomLevel;
            double t = mapcontrol.ZoomLevel;
            foreach (var b in mapcontrol.MapElements)
            {
                MapIcon d = (MapIcon)b;
                double dd = calDistance_GPS(args.Location, d.Location);
                KeyValuePair<double, MapIcon> result = new KeyValuePair<double, MapIcon>(dd, d);
                results.Add(result);
            }
            if (results.Count > 0)
            {
                results.Sort((x, y) => x.Key.CompareTo(y.Key));
                Point p = new Point();
                mapcontrol.GetOffsetFromLocation(results[0].Value.Location, out p);
                
                double hMax = mapcontrol.ActualHeight;
                double wMax = mapcontrol.ActualWidth;
                if (p.X < 0 || p.Y < 0 || p.X>wMax || p.Y>hMax)
                {
                    mapcontrol.Center = results[0].Value.Location;
                    mapcontrol.GetOffsetFromLocation(results[0].Value.Location, out p);
                }
                
                
                wavemoveY.To = p.Y - 25;
                wavemoveX.To = p.X - 25;

                myStoryboardX.Begin();
                myStoryboardY.Begin();

                infoPanel_Side.Children.Clear();
                infoPanel.Children.Clear();





                List<KeyValuePair<string, MapIcon>> results2 = titleMarkers.Where(fd => fd.Value.Location == results[0].Value.Location).ToList();


                string[] textPrams = results2[0].Key.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string title in textPrams)
                {
                    TextBlock info = new TextBlock
                    {
                        Text = title
                    };
                    info.TextWrapping = TextWrapping.Wrap;
                    info.FontSize = 20;
                    info.Margin = new Thickness
                    {
                        Bottom = 0,
                        Top = 0,
                        Left = 0,
                        Right = 0
                    };
                    infoPanel.Children.Add(info);
                }

                AppBarButton gotoBiz = new AppBarButton
                {
                    Icon = this.goBizBnt.Icon,
                    Label = "menu"
                };
                string phone = textPrams[1].Replace("-","").Replace(" ","").Replace(")","").Replace("(","");
                try
                {
                    gotoBiz.CommandParameter = textPrams[1].Replace("-", "").Replace(" ", "").Replace(")", "").Replace("(", "");
                    gotoBiz.Click += gotoBiz_Click;
                    infoPanel_Side.Children.Add(gotoBiz);
                    dir_bnt.CommandParameter = mealaRoniApi.bizItems.First(fd => fd.phone == phone);
                    gridHandelY.Duration = new Duration(TimeSpan.FromMilliseconds(200));                    
                    gridHandelY.To = 0;
                    gridHandleStroyBoardY.Begin();
                    mapcontrol.CenterChanged += mapcontrol_CenterChanged;

                }
                catch { }
        

            }

        }

        void gotoBiz_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                mealaRoniApi.selectedBiz = mealaRoniApi.bizItems.First(fd => fd.phone == ((AppBarButton)sender).CommandParameter.ToString());
                
                if (!Frame.Navigate(typeof(SectionPage), mealaRoniApi))
                {
                    throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
                }
            }
            catch (Exception eer)
            {
                
                
            }
        }

        public double calDistance_GPS(Geopoint a, Geopoint b)
        {
            double R = 6371 * 1 / 1.6;
            double delataLat = a.Position.Latitude - b.Position.Latitude;
            double delataLng = a.Position.Longitude - b.Position.Longitude;
            double alpha = Math.Pow(Math.Sin(delataLat / 2), 2) +
            Math.Cos(a.Position.Latitude) * Math.Cos(b.Position.Latitude) * Math.Pow(Math.Sin(delataLng / 2), 2);
            double c = 2 * Math.Atan2(Math.Sqrt(alpha), Math.Sqrt(1 - alpha));
            double d = R * c;
            return d;


        }

        private void infoPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void showFilters(object sender, RoutedEventArgs e)
        {
            MenuFlyout mf = (MenuFlyout)this.Resources["myMenuFlyout"];
            mf.Placement = FlyoutPlacementMode.Bottom;
            mf.ShowAt(commandBar);
        }

        private void ShowTheseMarkers(object sender, RoutedEventArgs e)
        {
            string status = "";
            switch (((MenuFlyoutItem)sender).Text)
            {

                case "All":
                    status = "All";
                    break;
                case "Show Opened":
                    status = "open";
                    break;
                case "Show Closed":
                    status = "close";
                    break;
                default:
                    break;
            }
            mapcontrol.MapElements.Clear();
            foreach (KeyValuePair<string, MapIcon> pair in titleMarkers.Where(fd => fd.Key.ToLower().Contains(status) || status == "All"))
            {
                mapcontrol.MapElements.Add(pair.Value);
            }
            

        }

        async private void getDirections(object sender, RoutedEventArgs e)
        {
            apiMealaroni.BizItem selectedBiz = (apiMealaroni.BizItem)((AppBarButton)sender).CommandParameter;
            if (selectedBiz.geo != null)
            {
                string[] geo = selectedBiz.geo.Split(new char[] { ',' });
                if (geo.Length == 2)
                {
                    string uriLaunch = String.Format("ms-drive-to:?destination.latitude={0}&destination.longitude={1}&destination.name={2}", geo[0], geo[1], selectedBiz.name);
                    Uri uri = new Uri(uriLaunch);
                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);
                }
                else
                {

                    string uriLaunch = String.Format("bingmaps:?q={0}", selectedBiz.name);
                    Uri uri = new Uri(uriLaunch);
                    var success = await Windows.System.Launcher.LaunchUriAsync(uri);

                }


            }
        }

        private void animationComplete(object sender, object e)
        {

        }


     

 
   

    }

        
}
