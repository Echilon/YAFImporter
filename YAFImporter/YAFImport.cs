using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAFImporter {
    class YAFImport :ADONETBase{
        public void Truncate()
        {
            Execute(conn =>
                {
                    using (var trans = conn.BeginTransaction()) {
                        using (var cmd = conn.CreateCommand()) {
                            cmd.Transaction = trans;

                            cmd.CommandText = "UPDATE yaf_Forum SET LastMessageID = NULL, LastTopicID = NULL WHERE ForumID > 1";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "UPDATE yaf_Forum SET LastTopicID = NULL WHERE ForumID > 1";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "UPDATE yaf_Topic SET LastMessageID = NULL WHERE ForumID > 1";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM yaf_Message WHERE TopicID > 1";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM yaf_Topic";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM yaf_ForumAccess WHERE ForumID > 1";
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "DELETE FROM yaf_Forum WHERE ForumID > 1";
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                });
        }

        public void AddForums(IEnumerable<Forum> categories)
        {
            Execute(conn =>
                {
                    using (var trans = conn.BeginTransaction()) {
                        using(var cmdCategories = conn.CreateCommand())
                        using(var cmdDiscussions = conn.CreateCommand()) // yaf_forum
                        using (var cmdDiscussionPermissions = conn.CreateCommand())//yaf_forumaccess
                        using (var cmdComments = conn.CreateCommand()) {
                            cmdCategories.Transaction = trans;
                            cmdDiscussions.Transaction = trans;
                            cmdDiscussionPermissions.Transaction = trans;
                            cmdComments.Transaction = trans;

                            cmdCategories.CommandText = "INSERT INTO yaf_Forum (CategoryID, ParentID, Name, Description, SortOrder, NumTopics, NumPosts, Flags)"
                                                        + " VALUES (@CategoryID, @ParentID, @Name, @Description, @SortOrder, @NumTopics, @NumPosts, @Flags); SELECT SCOPE_IDENTITY();";
                            cmdCategories.Parameters.AddWithValue("@CategoryID", 1);
                            cmdCategories.Parameters.AddWithValue("@ParentID", DBNull.Value);
                            cmdCategories.Parameters.AddWithValue("@Name", string.Empty);
                            cmdCategories.Parameters.AddWithValue("@Description", string.Empty);
                            cmdCategories.Parameters.AddWithValue("@SortOrder", -1);
                            cmdCategories.Parameters.AddWithValue("@NumTopics", -1);
                            cmdCategories.Parameters.AddWithValue("@NumPosts", -1);
                            cmdCategories.Parameters.AddWithValue("@Flags", 0);

                            cmdDiscussionPermissions.CommandText = "INSERT INTO yaf_ForumAccess (GroupID, ForumID, AccessMaskID)"
                                                        + " VALUES (@GroupID, @ForumID, @AccessMaskID);";
                            cmdDiscussionPermissions.Parameters.AddWithValue("@GroupID", -1);
                            cmdDiscussionPermissions.Parameters.AddWithValue("@ForumID", -1);
                            cmdDiscussionPermissions.Parameters.AddWithValue("@AccessMaskID", -1);

                            cmdDiscussions.CommandText = "INSERT INTO yaf_Topic (ForumID, UserID, Posted, Topic, Views, Priority, NumPosts, Flags)"
                                                         + " VALUES (@ForumID, @UserID, @Posted, @Topic, @Views, @Priority, @NumPosts, @Flags);  SELECT SCOPE_IDENTITY();";
                            cmdDiscussions.Parameters.AddWithValue("@ForumID", -1);
                            cmdDiscussions.Parameters.AddWithValue("@UserID", -1);
                            cmdDiscussions.Parameters.AddWithValue("@Posted", DateTime.Now);
                            cmdDiscussions.Parameters.AddWithValue("@Topic", string.Empty);
                            cmdDiscussions.Parameters.AddWithValue("@Views", -1);
                            cmdDiscussions.Parameters.AddWithValue("@Priority", 1);
                            cmdDiscussions.Parameters.AddWithValue("@NumPosts", -1);
                            cmdDiscussions.Parameters.AddWithValue("@Flags", 0);

                            cmdComments.CommandText = "INSERT INTO yaf_Message (TopicID, Position, Indent, UserID, Posted, Message, IP, Flags, IsModeratorChanged)"
                                                         + " VALUES (@TopicID, @Position, @Indent, @UserID, @Posted, @Message, @IP, @Flags, @IsModeratorChanged);  SELECT SCOPE_IDENTITY();";
                            cmdComments.Parameters.AddWithValue("@TopicID", -1);
                            cmdComments.Parameters.AddWithValue("@Position", -1);
                            cmdComments.Parameters.AddWithValue("@Indent", -1);
                            cmdComments.Parameters.AddWithValue("@UserID", -1);
                            cmdComments.Parameters.AddWithValue("@Posted", DateTime.Now);
                            cmdComments.Parameters.AddWithValue("@Message", string.Empty);
                            cmdComments.Parameters.AddWithValue("@IP", string.Empty);
                            cmdComments.Parameters.AddWithValue("@Flags", 22);
                            cmdComments.Parameters.AddWithValue("@IsModeratorChanged", false);
                            cmdComments.Parameters.AddWithValue("@IsApproved", true);

                            // insert forums
                            foreach (var category in categories) {
                                cmdCategories.Parameters["@Name"].Value = category.Name;
                                cmdCategories.Parameters["@Description"].Value = category.Description;
                                cmdCategories.Parameters["@SortOrder"].Value = category.SortOrder;
                                cmdCategories.Parameters["@NumTopics"].Value = category.NumTopics;
                                cmdCategories.Parameters["@NumPosts"].Value = category.NumPosts;
                                category.ForumID = Convert.ToInt32(cmdCategories.ExecuteScalar());

                                cmdDiscussionPermissions.Parameters["@ForumID"].Value= category.ForumID;
                                cmdDiscussionPermissions.Parameters["@GroupID"].Value = 1;
                                cmdDiscussionPermissions.Parameters["@AccessMaskID"].Value = 1; // adnin
                                cmdDiscussionPermissions.ExecuteNonQuery();

                                cmdDiscussionPermissions.Parameters["@GroupID"].Value = 2;
                                cmdDiscussionPermissions.Parameters["@AccessMaskID"].Value = 4; // read only
                                cmdDiscussionPermissions.ExecuteNonQuery();

                                cmdDiscussionPermissions.Parameters["@GroupID"].Value = 3;
                                cmdDiscussionPermissions.Parameters["@AccessMaskID"].Value = 3; // member
                                cmdDiscussionPermissions.ExecuteNonQuery();

                                cmdDiscussions.Parameters["@ForumID"].Value = category.ForumID;
                                // insert topics
                                foreach (var disc in category.Discussions) {
                                    cmdDiscussions.Parameters["@UserID"].Value = disc.InsertUserID;
                                    cmdDiscussions.Parameters["@Posted"].Value = disc.DateInserted;
                                    cmdDiscussions.Parameters["@Topic"].Value = disc.Name;
                                    cmdDiscussions.Parameters["@Views"].Value = disc.CountViews;
                                    cmdDiscussions.Parameters["@Priority"].Value = 1;
                                    cmdDiscussions.Parameters["@NumPosts"].Value = disc.CountComments;
                                    disc.DiscussionID = Convert.ToInt32(cmdDiscussions.ExecuteScalar());

                                    int position = 0;
                                    cmdComments.Parameters["@TopicID"].Value = disc.DiscussionID;
                                    cmdComments.Parameters["@Indent"].Value = 0;
                                    cmdComments.Parameters["@IP"].Value = string.Empty;
                                    var baseComment = new Comment() {
                                        InsertUserID = disc.InsertUserID,
                                        DateCreated = disc.DateInserted,
                                        Body = disc.Body, 
                                        DiscussionID = disc.DiscussionID,
                                    };
                                    disc.Comments.Insert(0,baseComment);

                                    foreach (var msg in disc.Comments.OrderBy(c=>c.DateCreated)) {
                                        cmdComments.Parameters["@Position"].Value = position++;
                                        cmdComments.Parameters["@UserID"].Value = msg.InsertUserID;
                                        cmdComments.Parameters["@Posted"].Value = msg.DateCreated;
                                        cmdComments.Parameters["@Message"].Value = msg.Body;
                                        msg.CommentID = Convert.ToInt32(cmdComments.ExecuteScalar());
                                        
                                    }
                                }
                            }
                        }
                        trans.Commit();
                    }
                });
        }
    }
}
