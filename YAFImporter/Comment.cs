using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YAFImporter {
    class Comment {
        public int CommentID{ get; set; }
        public int DiscussionID { get; set; }
        public int InsertUserID { get; set; }
        public string Body{ get; set; }
        public DateTime DateCreated { get; set; }

        public Comment() {
        }

        public Comment(XElement ele)
        :this(){
            this.CommentID = Convert.ToInt32(ele.Element("CommentID").Value);
            this.DiscussionID = Convert.ToInt32(ele.Element("DiscussionID").Value);
            this.InsertUserID = Convert.ToInt32(ele.Element("InsertUserID").Value);
            this.Body = System.Web.HttpUtility.HtmlDecode(ele.Element("Body").Value);
            this.DateCreated = Convert.ToDateTime(ele.Element("DateInserted").Value);
        }
    }
}
