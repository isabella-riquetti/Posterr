namespace Posterr.Services.Model
{
    /// <summary>
    /// Database friendly return format, should return only the essential data
    /// </summary>
    public class PostsModel
    {
        public PostsModel()
        {
        }

        public PostsModel(BasicPostModel model)
        {
            this.PostId = model.PostId;
            this.Username = model.Username;
            this.Content = model.Content;
            this.CreatedAt = model.CreatedAt.ToString();

            if (model.OriginalPostId != null)
            {
                this.OriginalPost = new PostsModel()
                {
                    PostId = (int)model.OriginalPostId,
                    Username = model.OriginalPostUsername,
                    Content = model.OriginalPostContent,
                    CreatedAt = model.OriginalPostCreatedAt.ToString()
                };
            }
        }

        public int PostId { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
        public PostsModel OriginalPost { get; set; }
    }

    /// <summary>
    /// Format the API response to an UI friendy format
    /// The Id, Content, Username and CreatedAt will always be in the same place in the UI
    /// But if IsRepost we should add the "Reposted by {Repost.Username} at {Repost.CreatedAt}
    /// And if IsRequote we should add the "Quoted" post below the Content
    /// </summary>
    public class PostResponseModel
    {
        public PostResponseModel()
        {

        }
        public PostResponseModel(PostsModel postModel)
        {
            this.IsRequote = postModel.OriginalPost != null && postModel.Content != null;
            this.IsRepost = postModel.OriginalPost != null && postModel.Content == null;
            if (this.IsRepost)
            {
                this.PostId = postModel.OriginalPost.PostId;
                this.Content = postModel.OriginalPost.Content;
                this.Username = postModel.OriginalPost.Username;
                this.CreatedAt = postModel.OriginalPost.CreatedAt;
                this.Repost = new RepostedModel()
                {
                    PostId = postModel.PostId,
                    Username = postModel.Username,
                    CreatedAt = postModel.CreatedAt
                };
            }
            else
            {
                this.PostId = postModel.PostId;
                this.Content = postModel.Content;
                this.Username = postModel.Username;
                this.CreatedAt = postModel.CreatedAt;

                if (this.IsRequote)
                {
                    this.Quoted = new QuotedModel()
                    {
                        PostId = postModel.OriginalPost.PostId,
                        Username = postModel.OriginalPost.Username,
                        Content = postModel.OriginalPost.Content,
                        CreatedAt = postModel.OriginalPost.CreatedAt
                    };
                };
            }
        }

        public int PostId { get; set; }
        public string Username { get; set; }
        public string CreatedAt { get; set; }
        public string Content { get; set; }

        public RepostedModel Repost { get; set; }
        public bool IsRepost { get; set; }

        public QuotedModel Quoted { get; set; }
        public bool IsRequote { get; set; }

    }

    public class RepostedModel
    {
        public int PostId { get; set; }
        public string Username { get; set; }
        public string CreatedAt { get; set; }
    }

    public class QuotedModel : RepostedModel
    {
        public string Content { get; set; }
    }
}
