using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YAFImporter {
    class Discussion {
        public int DiscussionID { get; set; }
        public int CategoryID { get; set; }
        public int InsertUserID { get; set; }
        public int? LastCommentID { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public int CountComments { get; set; }
        public int CountViews { get; set; }
        public bool IsClosed { get; set; }
        public bool IsAnnounce { get; set; }
        public DateTime DateInserted { get; set; }
        public DateTime DateLastComment { get; set; }
        public int? LastCommentUserID { get; set; }
        public int? RegardingID { get; set; }

        public List<Comment> Comments { get; set; }

        public Discussion()
        {
            this.Comments = new List<Comment>();
        }
        public Discussion(XElement ele)
        :this(){
            this.DiscussionID = Convert.ToInt32(ele.Element("DiscussionID").Value);
            this.CategoryID = Convert.ToInt32(ele.Element("CategoryID").Value);
            this.InsertUserID = Convert.ToInt32(ele.Element("InsertUserID").Value);
            this.Name = System.Web.HttpUtility.HtmlDecode(ele.Element("Name").Value);
            this.Body = System.Web.HttpUtility.HtmlDecode(ele.Element("Body").Value);
            this.CountComments = Convert.ToInt32(ele.Element("CountComments").Value);
            this.CountViews = Convert.ToInt32(ele.Element("CountViews").Value);
            this.IsClosed = Convert.ToInt32(ele.Element("Closed").Value) == 1;
            this.IsAnnounce = Convert.ToInt32(ele.Element("Announce").Value) == 1;
            this.DateInserted = Convert.ToDateTime(ele.Element("DateInserted").Value);
            this.DateLastComment = Convert.ToDateTime(ele.Element("DateLastComment").Value);

            var eleLastComment = ele.Element("LastCommentID");
            if (eleLastComment != null)
                this.LastCommentID = Convert.ToInt32(eleLastComment.Value);

            var eleLastCommentUserID = ele.Element("LastCommentUserID");
            if (eleLastCommentUserID != null)
                this.LastCommentUserID = Convert.ToInt32(eleLastCommentUserID.Value);

            var eleRegardingID = ele.Element("RegardingID");
            if (eleRegardingID != null)
                this.RegardingID = Convert.ToInt32(eleRegardingID.Value);
        }
    }
}
