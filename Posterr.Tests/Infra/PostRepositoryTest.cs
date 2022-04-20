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
using System.Linq;
using Posterr.Infra.Repository;

namespace Posterr.Tests.Repository
{
    public class PostRepositoryTest
    {
        #region GetPostById
        [Theory, MemberData(nameof(GetPostByIdTests))]
        public void GetPostByIdTest(GetPostByIdTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new PostRepository(apiContext);
            IQueryable<Post> response = repository.GetPostsById(test.PostId);

            Assert.Equal(test.ExpectedResponseCount, response.Count());
        }

        public static TheoryData<GetPostByIdTestInput> GetPostByIdTests = new TheoryData<GetPostByIdTestInput>()
        {
            new GetPostByIdTestInput()
            {
                TestName = "Success, no posts",
                PostId = 1,
                ExpectedResponseCount = 0
            },
            new GetPostByIdTestInput()
            {
                TestName = "Success, one basic post",
                PostId = 1,
                ExpectedResponseCount = 1,
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
            new GetPostByIdTestInput()
            {
                TestName = "Success, one repost from the own user",
                PostId = 1,
                ExpectedResponseCount = 1,
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
            new GetPostByIdTestInput()
            {
                TestName = "Success, quoted post from other user",
                PostId = 2,
                ExpectedResponseCount = 1,
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
            }
        };
        public class GetPostByIdTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int PostId { get; set; }

            public int ExpectedResponseCount { get; set; }

        }
        #endregion GetPostById
        
        #region GetPostsByUserId
        [Theory, MemberData(nameof(GetPostsByUserIdTests))]
        public void GetPostsByUserIdTest(GetPostsByUserIdTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new PostRepository(apiContext);
            IQueryable<Post> response = repository.GetPostsByUserId(test.UserId, test.Skip);

            Assert.Equal(test.ExpectedResponseCount, response.Count());
        }

        public static TheoryData<GetPostsByUserIdTestInput> GetPostsByUserIdTests = new TheoryData<GetPostsByUserIdTestInput>()
        {
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, no posts",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 0,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 1,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, one repost from the own user",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 2,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 1,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, quote post from the own user",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 2,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 1,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, page 1 of 1",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 4,
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
                    }
                }
            },
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, page 1 of 2",
                UserId = 1,
                Skip = 0,
                ExpectedResponseCount = 5,
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
            new GetPostsByUserIdTestInput()
            {
                TestName = "Success, page 2 of 2",
                UserId = 1,
                Skip = 1,
                ExpectedResponseCount = 2,
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
        public class GetPostsByUserIdTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int UserId { get; set; }
            public int Skip { get; set; }

            public int ExpectedResponseCount { get; set; }

        }
        #endregion GetPostsByUserId

        #region GetFollowedPosts
        [Theory, MemberData(nameof(GetFollowedPostsTests))]
        public void GetFollowedPostsTest(GetFollowedPostsTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new PostRepository(apiContext);
            IQueryable<Post> response = repository.GetFollowedPosts(test.UserId, test.Skip, test.PageLimit);

            Assert.Equal(test.ExpectedResponseCount, response.Count());
        }

        public static TheoryData<GetFollowedPostsTestInput> GetFollowedPostsTests = new TheoryData<GetFollowedPostsTestInput>()
        {
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, no posts",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 0
            },
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, one basic post",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 1,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, one repost from the own user",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, one repost from other user",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 1,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, quote post from the own user",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, quoted post from other user",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 1,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, page 1 of 1",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 4,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
                    }
                }
            },
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, page 1 of 2",
                UserId = 2,
                Skip = 0,
                ExpectedResponseCount = 5,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
            new GetFollowedPostsTestInput()
            {
                TestName = "Success, page 2 of 2",
                UserId = 2,
                Skip = 1,
                ExpectedResponseCount = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user2"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
        public class GetFollowedPostsTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int UserId { get; set; }
            public int Skip { get; set; }

            public int ExpectedResponseCount { get; set; }
            public int PageLimit => 5;
        }
        #endregion GetFollowedPosts

        #region GetTimelinePosts
        [Theory, MemberData(nameof(GetTimelinePostsTests))]
        public void GetTimelinePostsTest(GetTimelinePostsTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new PostRepository(apiContext);
            IQueryable<Post> response = repository.GetTimelinePosts(test.Skip, test.PageLimit);

            Assert.Equal(test.ExpectedResponseCount, response.Count());
        }

        public static TheoryData<GetTimelinePostsTestInput> GetTimelinePostsTests = new TheoryData<GetTimelinePostsTestInput>()
        {
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, no posts",
                Skip = 0,
                ExpectedResponseCount = 0
            },
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, one basic post",
                Skip = 0,
                ExpectedResponseCount = 1,
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
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, one reposts",
                Skip = 0,
                ExpectedResponseCount = 2,
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
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, quoted posts",
                Skip = 0,
                ExpectedResponseCount = 2,
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
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, page 1 of 1",
                Skip = 0,
                ExpectedResponseCount = 4,
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
                    }
                }
            },
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, page 1 of 2",
                Skip = 0,
                ExpectedResponseCount = 5,
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
            new GetTimelinePostsTestInput()
            {
                TestName = "Success, page 2 of 2",
                Skip = 1,
                ExpectedResponseCount = 2,
                UsersToAdd = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Username = "user1"
                    },
                    new User()
                    {
                        Id = 2,
                        Username = "user1"
                    }
                },
                FollowsToAdd = new List<Follow>() {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
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
        public class GetTimelinePostsTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int Skip { get; set; }

            public int ExpectedResponseCount { get; set; }
            public int PageLimit => 5;
        }
        #endregion GetTimelinePosts

        #region GetPostsByPartialTextSearch
        [Theory, MemberData(nameof(GetPostsByPartialTextSearchTests))]
        public void GetPostsByPartialTextSearchTest(GetPostsByPartialTextSearchTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var repository = new PostRepository(apiContext);
            IQueryable<Post> response = repository.GetPostsByPartialTextSearch(test.SearchText, test.Skip, test.PageLimit);

            Assert.Equal(test.ExpectedResponseCount, response.Count());
        }

        public static TheoryData<GetPostsByPartialTextSearchTestInput> GetPostsByPartialTextSearchTests = new TheoryData<GetPostsByPartialTextSearchTestInput>()
        {
            new GetPostsByPartialTextSearchTestInput()
            {
                TestName = "Success, no posts",
                Skip = 0,
                SearchText = "No no",
                ExpectedResponseCount = 0
            },
            new GetPostsByPartialTextSearchTestInput()
            {
                TestName = "Success, one basic post",
                Skip = 0,
                ExpectedResponseCount = 1,
                SearchText = "Hello",
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
                        Content = "It's me",
                        CreatedAt = new DateTime(2022,4,19,13,19,15),
                        OriginalPostId = null
                    }
                }
            },
            new GetPostsByPartialTextSearchTestInput()
            {
                TestName = "Success, ignore repost",
                Skip = 0,
                SearchText = "Posterr",
                ExpectedResponseCount = 1,
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
            new GetPostsByPartialTextSearchTestInput()
            {
                TestName = "Success, get posts with quote",
                Skip = 0,
                SearchText = "too",
                ExpectedResponseCount = 1,
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
            new GetPostsByPartialTextSearchTestInput()
            {
                TestName = "Success, get posts with quote and the quoted post",
                Skip = 0,
                SearchText = "I'm",
                ExpectedResponseCount = 2,
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
            }
        };
        public class GetPostsByPartialTextSearchTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int Skip { get; set; }
            public string SearchText { get; set; }

            public int ExpectedResponseCount { get; set; }
            public int PageLimit => 5;

        }
        #endregion GetPostsByPartialTextSearch

        #region CreatePost
        [Theory, MemberData(nameof(CreatePostTests))]
        public void CreatePostTest(CreatePostTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var respository = new PostRepository(apiContext);
            respository.CreatePost(test.AuthenticatedUserId, test.Content, test.CreatedAt, test.OriginalPostId);

            apiContext.Posts.Should().ContainSingle(f => f.UserId == test.AuthenticatedUserId && f.Content == test.Content && f.OriginalPostId == test.OriginalPostId);
        }

        public static TheoryData<CreatePostTestInput> CreatePostTests = new TheoryData<CreatePostTestInput>()
        {
            new CreatePostTestInput()
            {
                TestName = "Create post basic",
                AuthenticatedUserId = 1,
                Content = "Hello",
                CreatedAt = new DateTime(2022,4,19,13,19,15)
            },
            new CreatePostTestInput()
            {
                TestName = "Create repost",
                AuthenticatedUserId = 1,
                CreatedAt = new DateTime(2022,4,19,13,19,15),
                OriginalPostId = 1
            },
            new CreatePostTestInput()
            {
                TestName = "Create post quote",
                AuthenticatedUserId = 1,
                Content = "Hello",
                CreatedAt = new DateTime(2022,4,19,13,19,15),
                OriginalPostId = 1
            }
        };
        public class CreatePostTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int AuthenticatedUserId { get; set; }
            public string Content { get; set; }
            public DateTime CreatedAt { get; set; }
            public int? OriginalPostId { get; set; }
        }
        #endregion CreatePost

    }
}
