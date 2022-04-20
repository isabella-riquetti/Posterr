using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Posterr.DB;
using Posterr.Services;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Posterr.DB.Models;

namespace Posterr.Tests.Services
{
    public class PostServiceTest
    {
        #region GetUserFollowingTimeline
        [Theory, MemberData(nameof(GetUserFollowingTimelineTests))]
        public async void GetUserFollowingTimelineTest(GetUserFollowingTimelineTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new PostService(apiContext);
            BaseResponse<IList<PostResponseModel>> response = await service.GetUserFollowingTimeline(test.AuthenticatedUserId, test.Skip, test.CustomPageSize);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }


        public static TheoryData<GetUserFollowingTimelineTestInput> GetUserFollowingTimelineTests = new TheoryData<GetUserFollowingTimelineTestInput>()
        {
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, no posts",
                AuthenticatedUserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, one basic post",
                AuthenticatedUserId = 1,
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, one repost from other user",
                AuthenticatedUserId = 1,
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1
                    }
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, quoted post from other user",
                AuthenticatedUserId = 1,
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1
                    }
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, page 1 of 1",
                AuthenticatedUserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 2,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 10,
                        UserId = 1,
                        Content = "Post 0",
                        CreatedAt = new DateTime(2022,4,19,13,10,0),
                        OriginalPostId = null
                    },
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, page 1 of 2",
                AuthenticatedUserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 7,
                            Content = "Post 7",
                            CreatedAt = new DateTime(2022,4,19,13,7,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 6,
                            Content = "Post 6",
                            CreatedAt = new DateTime(2022,4,19,13,6,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 2,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 2,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 2,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 10,
                        UserId = 1,
                        Content = "Post 0",
                        CreatedAt = new DateTime(2022,4,19,13,10,0),
                        OriginalPostId = null
                    }
                }
            },
            new GetUserFollowingTimelineTestInput()
            {
                TestName = "Success, page 2 of 2",
                AuthenticatedUserId = 1,
                Skip = 1,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 2,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 2,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 2,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 10,
                        UserId = 1,
                        Content = "Post 0",
                        CreatedAt = new DateTime(2022,4,19,13,10,0),
                        OriginalPostId = null
                    }
                }
            }
        };
        public class GetUserFollowingTimelineTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            public int CustomPageSize => 5;

            public int AuthenticatedUserId { get; set; }
            public int Skip { get; set; }

            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetUserFollowingTimeline

        #region GetTimeline
        [Theory, MemberData(nameof(GetTimelineTests))]
        public async void GetTimelineTest(GetTimelineTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new PostService(apiContext);
            BaseResponse<IList<PostResponseModel>> response = await service.GetTimeline(test.Skip, test.CustomPageSize);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static TheoryData<GetTimelineTestInput> GetTimelineTests = new TheoryData<GetTimelineTestInput>()
        {
            new GetTimelineTestInput()
            {
                TestName = "Success, no posts",
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, one basic post",
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, one repost from other user",
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
                        },
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, quoted post from other user",
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
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, page 1 of 1",
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 2,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, page 1 of 2",
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 7,
                            Content = "Post 7",
                            CreatedAt = new DateTime(2022,4,19,13,7,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 6,
                            Content = "Post 6",
                            CreatedAt = new DateTime(2022,4,19,13,6,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 2,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 1,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    }
                }
            },
            new GetTimelineTestInput()
            {
                TestName = "Success, page 2 of 2",
                Skip = 1,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername2",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 2,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 2,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 2,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 1,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    }
                }
            }
        };
        public class GetTimelineTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            public int CustomPageSize => 5;

            public int Skip { get; set; }

            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetTimeline

        #region SearchByText
        [Theory, MemberData(nameof(SearchByTextTests))]
        public async void SearchByTextTest(SearchByTextTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new PostService(apiContext);
            BaseResponse<IList<PostResponseModel>> response = await service.SearchByText(test.SearchText, test.Skip, test.CustomPageSize);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static TheoryData<SearchByTextTestInput> SearchByTextTests = new TheoryData<SearchByTextTestInput>()
        {
            new SearchByTextTestInput()
            {
                TestName = "Success, no posts",
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                }
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, no valid posts",
                Skip = 0,
                SearchText = "Bye",
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                },
                PostsToAdd = new List<Post>()
                {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one basic post",
                Skip = 0,
                SearchText = "Hel",
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one post, do not include repost",
                Skip = 0,
                SearchText = "Posterr",
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1
                    }
                }
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one quotepost, does not include post that was quoted",
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
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1
                    }
                }
            },
            new SearchByTextTestInput()
            {
                TestName = "Success, one quotepost and the quote",
                Skip = 0,
                SearchText = "I'm",
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
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello, I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 2,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1
                    }
                }
            }
        };
        public class SearchByTextTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            public int CustomPageSize => 5;

            public int Skip { get; set; }
            public string SearchText { get; set; }

            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion SearchByText

        #region GetUserPosts
        [Theory, MemberData(nameof(GetUserPostsTests))]
        public async void GetUserPostsTest(GetUserPostsTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new PostService(apiContext);
            BaseResponse<IList<PostResponseModel>> response = await service.GetUserPosts(test.UserId, test.Skip);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }

        public static TheoryData<GetUserPostsTestInput> GetUserPostsTests = new TheoryData<GetUserPostsTestInput>()
        {
            new GetUserPostsTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                }
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
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }

                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, one repost from the own user",
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
                            Username = "TestUsername1",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,13,23,15).ToString(),
                                Username = "TestUsername1"
                            },
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        CreatedAt = new DateTime(2022,4,19,13,23,15),
                        OriginalPostId = 1
                    }
                }
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
                            Username = "TestUsername2",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,13,24,15).ToString(),
                                Username = "TestUsername1"
                            },
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello Posterr",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        CreatedAt = new DateTime(2022,4,19,13,24,15),
                        OriginalPostId = 1
                    }
                }
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, quote post from the own user",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "I'm new here",
                            CreatedAt = new DateTime(2022,4,19,13,25,00).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername1"
                            }
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        Content = "I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,25,00),
                        OriginalPostId = 1
                    }
                }
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
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello, I'm new here",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername2"
                            }
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello, I'm new here",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        Content = "I'm new too!",
                        CreatedAt = new DateTime(2022,4,19,13,27,40),
                        OriginalPostId = 1
                    }
                }
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, page 1 of 1",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 1,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 1,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                }
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, page 1 of 2",
                UserId = 1,
                Skip = 0,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 7,
                            Content = "Post 7",
                            CreatedAt = new DateTime(2022,4,19,13,7,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 6,
                            Content = "Post 6",
                            CreatedAt = new DateTime(2022,4,19,13,6,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Content = "Post 5",
                            CreatedAt = new DateTime(2022,4,19,13,5,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Content = "Post 4",
                            CreatedAt = new DateTime(2022,4,19,13,4,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 3,
                            Content = "Post 3",
                            CreatedAt = new DateTime(2022,4,19,13,3,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 1,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 1,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 1,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 1,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    }
                }
            },
            new GetUserPostsTestInput()
            {
                TestName = "Success, page 2 of 2",
                UserId = 1,
                Skip = 1,
                ExpectedResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Post 2",
                            CreatedAt = new DateTime(2022,4,19,13,2,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Post 1",
                            CreatedAt = new DateTime(2022,4,19,13,1,0).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        },
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Post 1",
                        CreatedAt = new DateTime(2022,4,19,13,1,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                        Content = "Post 2",
                        CreatedAt = new DateTime(2022,4,19,13,2,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 1,
                        Content = "Post 3",
                        CreatedAt = new DateTime(2022,4,19,13,3,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                        Content = "Post 4",
                        CreatedAt = new DateTime(2022,4,19,13,4,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 1,
                        Content = "Post 5",
                        CreatedAt = new DateTime(2022,4,19,13,5,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 1,
                        Content = "Post 6",
                        CreatedAt = new DateTime(2022,4,19,13,6,0),
                        OriginalPostId = null
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 1,
                        Content = "Post 7",
                        CreatedAt = new DateTime(2022,4,19,13,7,0),
                        OriginalPostId = null
                    }
                }
            }
        };
        public class GetUserPostsTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int UserId { get; set; }
            public int Skip { get; set; }

            public BaseResponse<IList<PostResponseModel>> ExpectedResponse { get; set; }

        }
        #endregion GetUserProfile

        #region CreatePost
        [Theory, MemberData(nameof(CreatePostTests))]
        public async void CreatePostTest(CreatePostTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new PostService(apiContext);
            BaseResponse<PostResponseModel> response = await service.CreatePost(test.Request, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse, options => options.WithStrictOrdering());
        }


        public static TheoryData<CreatePostTestInput> CreatePostTests = new TheoryData<CreatePostTestInput>()
        {
            new CreatePostTestInput()
            {
                TestName = "Success, one basic post",
                AuthenticatedUserId = 1,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    Content = "Test Content"
                },
                ExpectedResponse = BaseResponse<PostResponseModel>
                    .CreateSuccess(
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Test Content",
                            CreatedAt = new DateTime(2022,4,19,19,00,00).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = false
                        }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 1,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new CreatePostTestInput()
            {
                TestName = "Success, a repost",
                AuthenticatedUserId = 1,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    OriginalPostId = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>
                    .CreateSuccess(
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                            Username = "TestUsername2",
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 2,
                                CreatedAt = new DateTime(2022,4,19,19,00,00).ToString(),
                                Username = "TestUsername1"
                            },
                            IsRequote = false
                        }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new CreatePostTestInput()
            {
                TestName = "Success, a quotepost",
                AuthenticatedUserId = 1,
                Request = new CreatePostRequestModel(new DateTime(2022,4,19,19,00,00))
                {
                    Content = "Hello, how are you?",
                    OriginalPostId = 1
                },
                ExpectedResponse = BaseResponse<PostResponseModel>
                    .CreateSuccess(
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Content = "Hello, how are you?",
                            CreatedAt = new DateTime(2022,4,19,19,00,00).ToString(),
                            Username = "TestUsername1",
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19,13,19,15).ToString(),
                                Username = "TestUsername2"
                            }
                        }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test Name",
                        Username = "TestUsername1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test Name 2",
                        Username = "TestUsername2",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                },
                PostsToAdd = new List<Post>() {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                        Content = "Hello",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            }
        };
        public class CreatePostTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int AuthenticatedUserId { get; set; }
            public CreatePostRequestModel Request { get; set; }

            public BaseResponse<PostResponseModel> ExpectedResponse { get; set; }

        }
        #endregion CreatePost
    }
}
