using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsiTrello.Model
{

    public class Card
    {
        public string id { get; set; }
        public object checkItemStates { get; set; }
        public bool closed { get; set; }
        public string dateLastActivity { get; set; }
        public string desc { get; set; }
        public object descData { get; set; }
        public string idBoard { get; set; }
        public string idList { get; set; }
        public List<object> idMembersVoted { get; set; }
        public int idShort { get; set; }
        public object idAttachmentCover { get; set; }
        public bool manualCoverAttachment { get; set; }
        public List<string> idLabels { get; set; }
        public string name { get; set; }
        public double pos { get; set; }
        public string shortLink { get; set; }
        public Badges badges { get; set; }
        public bool dueComplete { get; set; }
        public string due { get; set; }
        public List<object> idChecklists { get; set; }
        public List<object> idMembers { get; set; }
        public List<Label> labels { get; set; }
        public string shortUrl { get; set; }
        public bool subscribed { get; set; }
        public string url { get; set; }
    }

}
