namespace Posterr.Services.Model
{
    public class PostModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Content { get; set; }
        public string CreatedAt { get; set; }
        public PostModel OriginalPost { get; set; }
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
        public PostResponseModel(PostModel postModel)
        {
            this.IsRequote = postModel.OriginalPost != null && postModel.Content != null;
            this.IsRepost = postModel.OriginalPost != null && postModel.Content == null;
            if (this.IsRepost)
            {
                this.Id = postModel.OriginalPost.Id;
                this.Content = postModel.OriginalPost.Content;
                this.Username = postModel.OriginalPost.Username;
                this.CreatedAt = postModel.OriginalPost.CreatedAt;
                this.Repost = new RepostedModel()
                {
                    Id = postModel.Id,
                    Username = postModel.Username,
                    CreatedAt = postModel.CreatedAt
                };
            }
            else
            {
                this.Id = postModel.Id;
                this.Content = postModel.Content;
                this.Username = postModel.Username;
                this.CreatedAt = postModel.CreatedAt;

                if (this.IsRequote)
                {
                    this.Quoted = new QuotedModel()
                    {
                        Id = postModel.OriginalPost.Id,
                        Username = postModel.OriginalPost.Username,
                        Content = postModel.OriginalPost.Content,
                        CreatedAt = postModel.OriginalPost.CreatedAt
                    };
                };
            }            
        }
        
        public int Id { get; set; }
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
        public int Id { get; set; }
        public string Username { get; set; }
        public string CreatedAt { get; set; }
    }

    public class QuotedModel : RepostedModel
    {
        public string Content { get; set; }
    }
}
