using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;
using System.Xml;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Collections;
using Windows.Data.Xml.Dom;
using System.Text.RegularExpressions;
using Windows.Data.Json;

namespace mealaroni_ordering.Common
{
    public class apiMealaroni
    {
        public string api_response = "";

        public class BizItem
        {
            public String name { get; set; }
            public String namekey { get; set; }
            public String address { get; set; }
            public String city { get; set; }
            public String state { get; set; }
            public String zip { get; set; }
            public String country { get; set; }
            public String timezone { get; set; }
            public String phone { get; set; }
            public String geo { get; set; }
            public String cityState { get; set; }
            public String bizName { get; set; }
            public IXmlNode tagCloud;
            public IXmlNode storehours;
             
        }

        public class MenuOptions
        {
            public XmlDocument OptionGroups = null;
            
            public List<IXmlNode> getOptions(string optionGroup, string encode, string limit)
            {
                List<IXmlNode> options = new System.Collections.Generic.List<IXmlNode>();
                Action<IXmlNode, List<IXmlNode>> fillList = (IXmlNode inNode, List<IXmlNode> outList) =>
                {
                    foreach (IXmlNode nd in inNode.ChildNodes.Where(hd => hd.Attributes.GetNamedItem("name").NodeValue.ToString()!="").ToArray() )
                    {
                        bool isactive = false;
                        bool.TryParse(nd.Attributes.GetNamedItem("isActive").NodeValue.ToString(), out isactive);
                        if (isactive)
                        {
                            XmlAttribute limitNode = nd.OwnerDocument.CreateAttribute("limit");
                            limitNode.Value = limit;
                            nd.Attributes.SetNamedItem(limitNode);
                            outList.Add(nd);
                        }
                    }
                };

                IXmlNode group = OptionGroups.DocumentElement.SelectSingleNode(optionGroup);
                
                if(group!=null)
                {
                    if(encode=="")
                    {
                        fillList(group, options);
                    }
                    else
                    {
                        MatchCollection enabledOPtions = Regex.Matches(encode, "1");
                        foreach(Match m in enabledOPtions)
                        {
                            XmlAttribute limitNode = group.ChildNodes[m.Index].OwnerDocument.CreateAttribute("limit");
                            limitNode.Value = limit;
                            group.ChildNodes[m.Index].Attributes.SetNamedItem(limitNode);
                            options.Add(group.ChildNodes[m.Index]);
                        }
                    }
                }
                return options;
            }
            

        }

        public class MenuItem
        {
            public String menu { get; set; }
            public String name { get; set; }
            public String decription { get; set; }
            public String price { get; set; }
            public BizItem Biz;
            public IXmlNode options;

        }

        public class Menu
        {
            public String menu { get; set; }
            public String biz { get; set; }
            public IXmlNode days;
            public IXmlNode storehours;
            public XmlNodeList items;
            public List<KeyValuePair<string, List<MenuItem>>> keyMenus = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, System.Collections.Generic.List<MenuItem>>>();
            public List<MenuItem> getMenu(string menuname)
            {
                List<MenuItem> menuitemsList = new System.Collections.Generic.List<MenuItem>();
                List<KeyValuePair<string, List<MenuItem>>> items = keyMenus.Where(fd => fd.Key == menuname).ToList();
                return null;
            }
            

        }

        public class LocPicker
        {
            public List<string> Countries = new List<string>();
            public List<string> States = new List<string>();
            public List<string> Cities = new List<string>();
            public IXmlNode provs = null;
            public string city { get; set; }
            public string state { get; set; }
            public string country { get; set; }
        }

        public LocPicker locationPicker = new LocPicker();
        public List<BizItem> bizItems = new List<BizItem>();
        public List<MenuItem> menuItems = new List<MenuItem>();
        public List<Menu> menus = new List<Menu>();
        public MenuOptions menuOptions = new MenuOptions();
        public MenuItem selectedItem = new MenuItem();
        public BizItem selectedBiz = new BizItem();

        async public Task<string> getRequest(string url)
        {

            HttpClient client = new HttpClient();
            Uri uri = new Uri(url);
            var respones = await client.GetAsync(uri);
            string responseText = await respones.Content.ReadAsStringAsync();
            
            if (responseText != null)
            {
                api_response = responseText.ToString();
            }
            return responseText;
        }

        async public  Task< List<BizItem>>  getBizList(string city, string state, string country)
        {
            string url = String.Format("http://mealaroni.com/api_roni.aspx?biz_city={0}&biz_state={1}&biz_country={2}&cmd=restaurants_list", city, state, country);
            string result = await getRequest(url);
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(result);
                XmlNodeList biz_indexNodes = xmlDoc.DocumentElement.SelectNodes("//indexed");
                bizItems.Clear();
                foreach (IXmlNode  node in biz_indexNodes)
                {
                    BizItem newItem = new BizItem();
                    newItem.name = node.Attributes.GetNamedItem("bizName").NodeValue.ToString();
                    newItem.namekey = node.Attributes.GetNamedItem("nameKey").NodeValue.ToString();
                    newItem.address = node.Attributes.GetNamedItem("bizAddrName").NodeValue.ToString();
                    newItem.city = city;
                    newItem.state = state;
                    newItem.country = country;
                    
                    newItem.phone = node.Attributes.GetNamedItem("Phone").NodeValue.ToString();
                    newItem.timezone = node.Attributes.GetNamedItem("timezone").NodeValue.ToString();
                    newItem.geo = node.Attributes.GetNamedItem("geo").NodeValue.ToString();
                    newItem.bizName = node.Attributes.GetNamedItem("bizName").NodeValue.ToString();
                    newItem.tagCloud = node.SelectSingleNode("tagCloud");
                    bizItems.Add(newItem);
                }

            }
            catch
            {

            }       
            return bizItems;            
        }

        async public Task<MenuOptions> getMenuOptions(string city, string state, string country, string phone)
        {
            MenuOptions options = new MenuOptions();
            string url = String.Format("http://mealaroni.com/api_roni.aspx?biz_city={0}&biz_state={1}&biz_country={2}&biz_phone={3}&cmd=menus_options", city, state, country, phone.Replace(" ", ""));
            string result = await getRequest(url);
            XmlDocument xmldoc = new XmlDocument();
            try
            {
                xmldoc.LoadXml(result);
            }
            catch { }
            options.OptionGroups = xmldoc;
           
            return options;
        }

        async public Task<List<MenuItem>> getMenu(string city, string state, string country, string phone)
        {
            try
            {
                menuOptions = await getMenuOptions(city, state, country, phone);
            }
            catch { }

            string url = String.Format("http://mealaroni.com/api_roni.aspx?biz_city={0}&biz_state={1}&biz_country={2}&biz_phone={3}&cmd=menus_all", city, state, country, phone.Replace(" ", ""));
            string result = await getRequest(url);
            XmlDocument xmlDoc = new XmlDocument();

            try
            {
                xmlDoc.LoadXml(result);
                menus.Clear();
                IXmlNode hours = xmlDoc.DocumentElement.SelectSingleNode("hours");
                xmlDoc.DocumentElement.SelectNodes("//Menu").All(fd => {
                    try {
                        Menu s = new Menu();
                        s.storehours = hours;
                        s.menu = fd.Attributes.GetNamedItem("name").NodeValue.ToString().ToString().Replace("_", " ");
                        s.days = fd.SelectSingleNode("ActiveDays");
                        s.items = fd.SelectNodes("MenuItem");
                        List<MenuItem> keyedMenuItems = new System.Collections.Generic.List<MenuItem>();
                        List<MenuItem> menuItemsX = new System.Collections.Generic.List<MenuItem>();
                        foreach (IXmlNode node in s.items)
                        {
                            IXmlNode options = node.SelectSingleNode("ItemOptions");
                            MenuItem newItem = new MenuItem();
                            newItem.menu = node.ParentNode.Attributes.GetNamedItem("name").NodeValue.ToString().Replace("_", " ");
                            newItem.name = node.SelectSingleNode("ItemName").InnerText;
                            newItem.decription = node.SelectSingleNode("ItemDescript").InnerText;
                            newItem.price = node.SelectSingleNode("ItemPrice").InnerText;
                            newItem.options = options;
                            menuItems.Add(newItem);  
                            menuItemsX.Add(newItem);
                        }
                        KeyValuePair<string, List<MenuItem>> keyedMenuItem = new System.Collections.Generic.KeyValuePair<string, List<MenuItem>>(s.menu, menuItemsX);
                        s.keyMenus.Add(keyedMenuItem);                    
                        menus.Add(s);
                    }
                    catch{}
                    return true;
                });


                XmlNodeList menuNodes = xmlDoc.DocumentElement.SelectNodes("//MenuItem");
                
                menuItems.Clear();
                
                foreach (IXmlNode node in menuNodes)
                {
                    IXmlNode options = node.SelectSingleNode("ItemOptions");
                   
                    MenuItem newItem = new MenuItem();                                       
                    newItem.menu = node.ParentNode.Attributes.GetNamedItem("name").NodeValue.ToString().Replace("_", " ");
                    newItem.name = node.SelectSingleNode("ItemName").InnerText;
                    newItem.decription = node.SelectSingleNode("ItemDescript").InnerText;
                    newItem.price = node.SelectSingleNode("ItemPrice").InnerText;
                    newItem.options = options;
                    menuItems.Add(newItem);                    
                }

            }
            catch
            {

            }
            return menuItems;
        }

         async public Task<string> getGeoList(string city, string state, string country)
        {
            city = city==null?"MEMPHIS":city;
            city = city==""?"MEMPHIS":city;
            string locale = country +"_"+state+"_"+city;
            locale = locale.ToUpper();
            string url = String.Format("http://mealaroni.com/apibase.aspx?getGeolist={0}", locale);
            Task<string> result = getRequest(url);
            string result2 = await result;
            api_response = result2 = result2.Split(new string[] { "|" }, StringSplitOptions.None)[0];
            api_response = "{\"results\":[" + result2 + "]}";
            return api_response;
            
        }

         async public Task<LocPicker> getCities(string city, string state, string country)
        {
            XmlDocument locationpicker = new XmlDocument();
            string url = String.Format("http://mealaroni.com/api_roni.aspx?biz_state={0}&biz_country={1}&biz_city={2}&cmd=ssrch", state, country, city);
            string result = await getRequest(url);
            locationpicker.LoadXml(result);

            Action<IXmlNode, List<string>> fillList = (IXmlNode inNode, List<string> outList) => {
                foreach(IXmlNode nd in inNode.ChildNodes)
                {
                    outList.Add(nd.InnerText);
                }
            };
            locationPicker.city = city.ToUpper();
            locationPicker.state = state.ToUpper();
            locationPicker.country = country.ToUpper();
             IXmlNode srcNode = locationpicker.DocumentElement.SelectSingleNode("select[@id='sel_country']");
             locationPicker.provs = locationpicker.DocumentElement.SelectSingleNode("provs");
             fillList(srcNode, locationPicker.Countries);
             
            srcNode = locationpicker.DocumentElement.SelectSingleNode("select[@id='sel_provices']");

             fillList(srcNode, locationPicker.States);

             srcNode = locationpicker.DocumentElement.SelectSingleNode("select[@id='sel_cities']");

             fillList(srcNode, locationPicker.Cities);

             return locationPicker;
            
        }

    }
}
