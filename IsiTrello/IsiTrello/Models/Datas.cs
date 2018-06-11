using Newtonsoft.Json;
using System.Collections.Generic;

namespace IsiTrello.Model
{
    public class Attachment
    {
        public string id { get; set; }
        public string url { get; set; }
        public string name { get; set; }
    }
    public class CheckItem
    {
        public string id { get; set; }
        public string name { get; set; }
        public string state { get; set; }
    }
    public class Checklist
    {
        public string id { get; set; }
        public string name { get; set; }
        public IList<CheckItem> checkitems { get; set; }
    }

    public class Badges
    {
        public int votes { get; set; }
        public bool viewingMemberVoted { get; set; }
        public bool subscribed { get; set; }
        public string fogbugz { get; set; }
        public int checkItems { get; set; }
        public int checkItemsChecked { get; set; }
        public int comments { get; set; }
        public int attachments { get; set; }
        public bool description { get; set; }
        public string due { get; set; }
        public bool dueComplete { get; set; }
    }

    public class Label
    {
        public string id { get; set; }
        public string idBoard { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public int uses { get; set; }
    }
    public class Blist
    {
        public string id { get; set; }
        public string name { get; set; }
       
        public Bpreferences prefs { get; set; }
    }

        public class Bpreferences
        {
        [JsonProperty("backgroundColor")]
            public string bacgroundColor { get; set; }
        }
}
