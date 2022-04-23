using FluentAssertions;
using NSubstitute;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Services;
using Posterr.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Posterr.Tests.Services
{
    public class PostServiceTest
    {
        #region GetUserPosts
        [Theory, MemberData(nameof(GetUserPostsTests))]
        public void GetUserPostsTest(GetUserPostsTestInput test)
        {
            var postRepositorySubstitute = Substitute.For<IPostRepository>();
            postRepositorySubstitute.GetPostsByUserId(Arg.Any<int>()).Returns(test.GetPostsByUserIdResponse);

            var service = new PostService(postRepositorySubstitute);
            BaseResponse<IList<PostResponseModel>> response = service.GetUserPosts(test.UserId, test.Skip);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static readonly TheoryData<GetUserPostsTestInput> GetUserPostsTests = new()
        {
            new GetUserPostsTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                GetPostsByUserIdResponse = new List<Post>().AsQueryable()
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,13,24,15).ToString(),
                                Username = "TestUsername2"
                            },
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "I'm new too!",
                            CreatedAt = new DateTime(2022,4,19,13,27,40).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello, I'm new here",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername1"
                            }
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            }
        };
        public class GetUserPostsTestInput
        {
            public string TestName { get; set; }
            public static int CustomPageSize => 5;

            public int UserId { get; set; }
            public int Skip { get; set; }
            public IQueryable<Post> GetPostsByUserIdResponse { get; set; }
            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetUserPosts

        #region GetUserFollowingTimeline
        [Theory, MemberData(nameof(GetUserFollowingTimelineTests))]
        public void GetUserFollowingTimelineTest(GetUserFollowingTimelineTestInput test)
        {
            var postRepositorySubstitute = Substitute.For<IPostRepository>();
            postRepositorySubstitute.GetFollowedPosts(Arg.Any<int>()).Returns(test.GetPostsByUserIdResponse);

            var service = new PostService(postRepositorySubstitute);
            BaseResponse<IList<PostResponseModel>> response = service.GetUserFollowingTimeline(test.UserId, test.Skip);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static readonly TheoryData<GetUserFollowingTimelineTestInput> GetUserFollowingTimelineTests = new()
        {
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                GetPostsByUserIdResponse = new List<Post>().AsQueryable()
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,13,24,15).ToString(),
                                Username = "TestUsername2"
                            },
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "I'm new too!",
                            CreatedAt = new DateTime(2022,4,19,13,27,40).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello, I'm new here",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername1"
                            }
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            }
        };
        public class GetUserFollowingTimelineTestInput
        {
            public string TestName { get; set; }
            public static int CustomPageSize => 5;

            public int UserId { get; set; }
            public int Skip { get; set; }
            public IQueryable<Post> GetPostsByUserIdResponse { get; set; }
            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetUserFollowingTimeline

        #region GetTimeline
        [Theory, MemberData(nameof(GetTimelineTests))]
        public void GetTimelineTest(GetTimelineTestInput test)
        {
            var postRepositorySubstitute = Substitute.For<IPostRepository>();
            postRepositorySubstitute.GetTimelinePosts(Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetPostsByUserIdResponse);

            var service = new PostService(postRepositorySubstitute);
            BaseResponse<IList<PostResponseModel>> response = service.GetTimeline(test.UserId, test.Skip);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static readonly TheoryData<GetTimelineTestInput> GetTimelineTests = new()
        {
            new GetTimelineTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                GetPostsByUserIdResponse = new List<Post>().AsQueryable()
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,13,24,15).ToString(),
                                Username = "TestUsername2"
                            },
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername1"
                        }
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false,
                        },
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "I'm new too!",
                            CreatedAt = new DateTime(2022,4,19,13,27,40).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello, I'm new here",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername1"
                            }
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername1"
                        }
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            }
        };
        public class GetTimelineTestInput
        {
            public string TestName { get; set; }
            public static int CustomPageSize => 5;

            public int UserId { get; set; }
            public int Skip { get; set; }
            public IQueryable<Post> GetPostsByUserIdResponse { get; set; }
            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetTimeline

        #region SearchByText
        [Theory, MemberData(nameof(SearchByTextTests))]
        public void SearchByTextTest(SearchByTextTestInput test)
        {
            var postRepositorySubstitute = Substitute.For<IPostRepository>();
            postRepositorySubstitute.GetPostsByPartialTextSearch(test.SearchText, Arg.Any<int>(), Arg.Any<int>()).Returns(test.GetPostsByUserIdResponse);

            var service = new PostService(postRepositorySubstitute);
            BaseResponse<IList<PostResponseModel>> response = service.SearchByText(test.SearchText, test.Skip);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static readonly TheoryData<SearchByTextTestInput> SearchByTextTests = new()
        {
            new SearchByTextTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                SearchText = "Hi!",
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                GetPostsByUserIdResponse = new List<Post>().AsQueryable()
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 1,
                Skip = 0,
                SearchText = "Hello",
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 1,
                Skip = 0,
                SearchText = "Posterr!",
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername1"
                        }
                    }
                }.AsQueryable()
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 1,
                Skip = 0,
                SearchText = "too",
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "I'm new too!",
                            CreatedAt = new DateTime(2022,4,19,13,27,40).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello, I'm new here",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername1"
                            }
                        }
                    }),
                GetPostsByUserIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            UserId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            OriginalPostId = null,
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            }
        };
        public class SearchByTextTestInput
        {
            public string TestName { get; set; }
            public static int CustomPageSize => 5;

            public int UserId { get; set; }
            public int Skip { get; set; }
            public IQueryable<Post> GetPostsByUserIdResponse { get; set; }
            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }
            public string SearchText { get; internal set; }
        }
        #endregion SearchByText

        #region CreatePost
        [Theory, MemberData(nameof(CreatePostTests))]
        public void CreatePostTest(CreatePostTestInput test)
        {
            var postRepositorySubstitute = Substitute.For<IPostRepository>();
            postRepositorySubstitute.CreatePost(Arg.Any<int>(), Arg.Any<string>(), Arg.Any<DateTime>(), Arg.Any<int?>()).Returns(test.CreatePostResponse);
            postRepositorySubstitute.GetPostsById(test?.Request?.OriginalPostId ?? 0).Returns(test.GetPostsByIdResponseForOriginal);
            postRepositorySubstitute.GetPostsById(test.CreatePostResponse.Id).Returns(test.GetPostsByIdResponse);

            var service = new PostService(postRepositorySubstitute);
            BaseResponse<PostResponseModel> response = service.CreatePost(test.Request, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static readonly TheoryData<CreatePostTestInput> CreatePostTests = new()
        {
            new CreatePostTestInput()
            {
                TestName = "Fail, no original post",
                AuthenticatedUserId = 1,
                Request = new CreatePostRequestModel(new DateTime(2022, 4, 19, 13, 19, 15))
                {
                    Content = "Hello",
                    OriginalPostId = 1
                },
                CreatePostResponse = new Post()
                {
                    Id = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>.CreateSuccess(
                    new PostResponseModel()
                    {
                        PostId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022, 4, 19, 13, 19, 15).ToString(),
                        Username = "TestUsername2",
                        IsRepost = false,
                        IsRequote = false
                    }),
                GetPostsByIdResponseForOriginal = new List<Post>().AsQueryable(),
                GetPostsByIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new CreatePostTestInput()
            {
                TestName = "Success, one basic post",
                AuthenticatedUserId = 1,
                Request = new CreatePostRequestModel(new DateTime(2022, 4, 19, 13, 19, 15))
                {
                    Content = "Hello"
                },
                CreatePostResponse = new Post()
                {
                    Id = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>.CreateSuccess(
                    new PostResponseModel()
                    {
                        PostId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022, 4, 19, 13, 19, 15).ToString(),
                        Username = "TestUsername2",
                        IsRepost = false,
                        IsRequote = false
                    }),
                GetPostsByIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null,
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new CreatePostTestInput()
            {
                TestName = "Fail, repost a repost",
                AuthenticatedUserId = 1,
                CreatePostResponse = new Post()
                {
                    Id = 2
                },
                Request = new CreatePostRequestModel(new DateTime(2022, 4, 19, 13, 19, 15))
                {
                    OriginalPostId = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>.CreateError("Can't repost a repost"),
                GetPostsByIdResponseForOriginal = new List<Post>() {
                    new Post()
                    {
                        Id = 1
                    }
                }.AsQueryable(),
                GetPostsByIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            },
            new CreatePostTestInput()
            {
                TestName = "Success, quoted post from other user",
                AuthenticatedUserId = 1,
                CreatePostResponse = new Post()
                {
                    Id = 2
                },
                Request = new CreatePostRequestModel(new DateTime(2022, 4, 19, 13, 19, 15))
                {
                    OriginalPostId = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>.CreateSuccess(
                    new PostResponseModel()
                    {
                        PostId = 2,
                        Content = "Hiii",
                        CreatedAt = new DateTime(2022, 4, 19, 13, 19, 15).ToString(),
                        Username = "TestUsername2",
                        IsRepost = false,
                        IsRequote = true,
                        Quoted = new QuotedModel()
                        {
                            PostId = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022, 4, 19, 13, 19, 15).ToString(),
                            Username = "TestUsername1"
                        }
                    }),
                GetPostsByIdResponseForOriginal = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        Content = "Hello Posterr"
                    }
                }.AsQueryable(),
                GetPostsByIdResponse = new List<Post>() {
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Hiii",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = 1,
                        OriginalPost = new Post()
                        {
                            Id = 1,
                            Content = "Hello Posterr",
                            CreatedAt = new DateTime(2022,4,19,13,19,15),
                            User = new User()
                            {
                                Username = "TestUsername1"
                            }
                        },
                        User = new User()
                        {
                            Username = "TestUsername2"
                        }
                    }
                }.AsQueryable()
            }
        };

        public class CreatePostTestInput
        {
            public string TestName { get; set; }
            public static int CustomPageSize => 5;

            public int AuthenticatedUserId { get; set; }
            public CreatePostRequestModel Request { get; set; }
            public IQueryable<Post> GetPostsByIdResponse { get; set; }
            public IQueryable<Post> GetPostsByIdResponseForOriginal { get; internal set; }
            public BaseResponse<PostResponseModel> ExpectedResponse { get; set; }
            public Post CreatePostResponse { get; internal set; }
        }
        #endregion GetUserProfile
    }
}
