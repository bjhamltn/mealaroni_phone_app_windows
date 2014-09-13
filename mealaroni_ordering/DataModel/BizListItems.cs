using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mealaroni_ordering.DataModel
{
    class BizListItems
    {
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
        }
    }

    class BizGroupList
    {
        List<BizGroupList> BizGroup_List;
    }
}
