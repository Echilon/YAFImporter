using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace YAFImporter {
    /// <summary>
    /// 
    /// </summary>
    class Forum {
        public int ForumID{ get; set; }
        public int CategoryID{ get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        public string Description{ get; set; }
        public int SortOrder { get; set; }
        public int NumTopics { get; set; }
        public int NumPosts { get; set; }
        public int Flags { get; set; }

        public List<Discussion> Discussions { get; set; } 

        public Forum()
        {
            this.Discussions=new List<Discussion>();
        }

        public Forum(XElement ele)
        :this(){
            this.ForumID = Convert.ToInt32(ele.Element("CategoryID").Value);
            this.CategoryID = 1;// Convert.ToInt32(ele.Element("CategoryID").Value);
            //var eleParent = ele.Element("ParentCategoryID");
            //if (eleParent != null && !string.IsNullOrWhiteSpace(eleParent.Value) && eleParent.Value != "-1")
            //    this.ParentID = Convert.ToInt16(ele.Value);
            this.Name = System.Web.HttpUtility.HtmlDecode(ele.Element("Name").Value);
            this.Description = System.Web.HttpUtility.HtmlDecode(ele.Element("Description").Value);
            this.SortOrder = Convert.ToInt32(ele.Element("Sort").Value);
            this.NumPosts = Convert.ToInt32(ele.Element("CountComments").Value);
            this.NumTopics = Convert.ToInt32(ele.Element("CountDiscussions").Value);
            
        }
    }
}
