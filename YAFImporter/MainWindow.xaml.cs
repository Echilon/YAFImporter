using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace YAFImporter {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, RoutedEventArgs e) {
            var docCategories = XDocument.Load(Constants.pathCategories);
            var categories = docCategories.Descendants("gdn_category").Select(ele => new Forum(ele)).ToList();
            var categoriesDict = categories.ToDictionary(d => d.ForumID, d => d);

            var docDiscussions = XDocument.Load(Constants.pathDiscussions);
            var discussions = docDiscussions.Descendants("gdn_discussion").Select(ele => new Discussion(ele)).ToList();
            foreach (var disc in discussions) {
                Forum parentForum = null;
                if (categoriesDict.TryGetValue(disc.CategoryID, out parentForum))
                    parentForum.Discussions.Add(disc);
            }

            var docComments = XDocument.Load(Constants.pathComments);
            var comments = docComments.Descendants("gdn_comment").Select(ele => new Comment(ele)).ToList();

            var discussionDict = discussions.ToDictionary(d => d.DiscussionID, d => d);
            foreach (var comment in comments) {
                Discussion parentDiscussion = null;
                if (discussionDict.TryGetValue(comment.DiscussionID, out parentDiscussion))
                    parentDiscussion.Comments.Add(comment);
            }

            var imported = new YAFImport();
            imported.AddForums(categories);
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            var db = new YAFImport();
            db.Truncate();
        }
    }
}
