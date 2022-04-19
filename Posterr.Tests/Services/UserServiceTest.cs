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
    public class UserServiceTest
    {
        #region GetUserProfile
        [Theory, MemberData(nameof(GetUserProfileTests))]
        public async void GetUserProfileTest(GetUserProfileTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();
            var postServiceSubstitute = Substitute.For<IPostService>();
            var followServiceSubstitute = Substitute.For<IFollowService>();

            postServiceSubstitute.GetUserPosts(Arg.Any<int>()).Returns(test.GetUserPostsResponse);
            followServiceSubstitute.IsUserFollowedBy(Arg.Any<int>(), Arg.Any<int>()).Returns(test.IsUserFollowedByAuthenticatedUserResponse);

            var service = new UserService(apiContext, postServiceSubstitute, followServiceSubstitute);
            BaseResponse<UserProfileModel> response = await service.GetUserProfile(test.UserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }


        public static TheoryData<GetUserProfileTestInput> GetUserProfileTests = new TheoryData<GetUserProfileTestInput>()
        {
            new GetUserProfileTestInput()
            {
                TestName = "Fail, user not found",
                UserId = 100,
                AuthenticatedUserId = 1,
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateError("User not found"),
            },
            new GetUserProfileTestInput()
            {
                TestName = "Fail to get posts",
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateError("Failed to get posts"),
                UserId = 1,
                AuthenticatedUserId = 1,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateError("Failed to get posts"),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test2",
                        Username = "Test2",
                        CreatedAt = DateTime.Now
                    }
                },
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1
                    }
                }
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, follow someone",
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    UserId = 1,
                    CreatedAt = "April 19, 2022",
                    Username = "Test1",
                    Followers = 0,
                    Following = 1,
                    Posts = 0,
                    TopPosts = new List<PostResponseModel>(),
                    Followed = false
                }),
                UserId = 1,
                AuthenticatedUserId = 1,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test2",
                        Username = "Test2",
                        CreatedAt = DateTime.Now
                    }
                },
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 1,
                        FollowingId = 2
                    }
                }
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, is followed by someone",
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    UserId = 1,
                    CreatedAt = "April 19, 2022",
                    Username = "Test1",
                    Followers = 1,
                    Following = 0,
                    Posts = 0,
                    TopPosts = new List<PostResponseModel>(),
                    Followed = false
                }),
                UserId = 1,
                AuthenticatedUserId = 1,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test2",
                        Username = "Test2",
                        CreatedAt = DateTime.Now
                    }
                },
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
                    }
                }
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, is followed by the authenticated user",
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    UserId = 1,
                    CreatedAt = "April 19, 2022",
                    Username = "Test1",
                    Followers = 1,
                    Following = 0,
                    Posts = 0,
                    TopPosts = new List<PostResponseModel>(),
                    Followed = true
                }),
                UserId = 1,
                AuthenticatedUserId = 2,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()),
                IsUserFollowedByAuthenticatedUserResponse = true,
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test2",
                        Username = "Test2",
                        CreatedAt = DateTime.Now
                    }
                },
                FollowsToAdd = new List<Follow>()
                {
                    new Follow()
                    {
                        Id = 1,
                        FollowerId = 2,
                        FollowingId = 1
                    }
                }
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, has post, repost and quote post and over 5 posts",
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    UserId = 1,
                    CreatedAt = "April 19, 2022",
                    Username = "Test1",
                    Followers = 0,
                    Following = 0,
                    Posts = 6,
                    TopPosts = new List<PostResponseModel>()
                    {
                        new PostResponseModel()
                        {
                            PostId = 6,
                            Username = "Test1",
                            Content = "Hi again",
                            CreatedAt = new DateTime(2022,4,19, 0, 5, 0).ToString(),
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 5,
                            Username = "Test1",
                            Content = "Hi",
                            CreatedAt = new DateTime(2022,4,19, 0, 4, 0).ToString(),
                            IsRepost = false,
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 4,
                            Username = "Test1",
                            Content = "Hello you too!",
                            CreatedAt = new DateTime(2022,4,19, 0, 3, 0).ToString(),
                            IsRepost = false,
                            IsRequote = true,
                            Quoted = new QuotedModel()
                            {
                                PostId = 1,
                                Username = "Test2",
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString()
                            }
                        },
                        new PostResponseModel()
                        {
                            PostId = 1,
                            Username = "Test2",
                            Content = "Hello",
                            CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString(),
                            IsRepost = true,
                            Repost = new RepostedModel()
                            {
                                PostId = 3,
                                CreatedAt = new DateTime(2022,4,19, 0, 2, 0).ToString(),
                                Username = "Test1"
                            },
                            IsRequote = false
                        },
                        new PostResponseModel()
                        {
                            PostId = 2,
                            Username = "Test1",
                            Content = "Hello Everyone",
                            CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString(),
                            IsRepost = false,
                            IsRequote = false
                        }
                    },
                    Followed = false
                }),
                UserId = 1,
                AuthenticatedUserId = 1,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateSuccess(new List<PostResponseModel>()
                    {
                        new PostResponseModel(new PostsModel()
                        {
                            PostId = 6,
                            Username = "Test1",
                            Content = "Hi again",
                            CreatedAt = new DateTime(2022,4,19, 0, 5, 0).ToString(),
                            OriginalPost = null
                        }),
                        new PostResponseModel(new PostsModel()
                        {
                            PostId = 5,
                            Username = "Test1",
                            Content = "Hi",
                            CreatedAt = new DateTime(2022,4,19, 0, 4, 0).ToString(),
                            OriginalPost = null
                        }),
                        new PostResponseModel(new PostsModel()
                        {
                            PostId = 4,
                            Username = "Test1",
                            Content = "Hello you too!",
                            CreatedAt = new DateTime(2022,4,19, 0, 3, 0).ToString(),
                            OriginalPost = new PostsModel()
                            {
                                PostId = 1,
                                Username = "Test2",
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString()
                            }
                        }),
                        new PostResponseModel(new PostsModel()
                        {
                            PostId = 3,
                            Username = "Test1",
                            CreatedAt = new DateTime(2022,4,19, 0, 2, 0).ToString(),
                            OriginalPost = new PostsModel()
                            {
                                PostId = 1,
                                Username = "Test2",
                                Content = "Hello",
                                CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString()
                            }
                        }),
                        new PostResponseModel(new PostsModel()
                        {
                            PostId = 2,
                            Username = "Test1",
                            Content = "Hello Everyone",
                            CreatedAt = new DateTime(2022,4,19, 0, 1, 0).ToString(),
                            OriginalPost = null
                        })
                    }),
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    },
                    new User()
                    {
                        Id = 2,
                        Name = "Test2",
                        Username = "Test2",
                        CreatedAt = DateTime.Now
                    }
                },
                PostsToAdd = new List<Post>()
                {
                    new Post()
                    {
                        Id = 1,
                        UserId = 2,
                    },
                    new Post()
                    {
                        Id = 2,
                        UserId = 1,
                    },
                    new Post()
                    {
                        Id = 3,
                        UserId = 1,
                    },
                    new Post()
                    {
                        Id = 4,
                        UserId = 1,
                    },
                    new Post()
                    {
                        Id = 5,
                        UserId = 1,
                    },
                    new Post()
                    {
                        Id = 6,
                        UserId = 1,
                    },
                    new Post()
                    {
                        Id = 7,
                        UserId = 1,
                    },
                }
            },
        };
        public class GetUserProfileTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            public BaseResponse<UserProfileModel> ExpectedResponse { get; set; }
            
            public int UserId { get; set; }
            public int AuthenticatedUserId { get; set; }

            public BaseResponse<IList<PostResponseModel>> GetUserPostsResponse { get; set; }
            public bool IsUserFollowedByAuthenticatedUserResponse { get; set; }
        }
        #endregion GetUserProfile

        #region IsValidUser
        [Theory, MemberData(nameof(IsValidUserTests))]
        public void IsValidUserTest(IsValidUserTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();
            var postServiceSubstitute = Substitute.For<IPostService>();
            var followServiceSubstitute = Substitute.For<IFollowService>();

            var service = new UserService(apiContext, postServiceSubstitute, followServiceSubstitute);
            BaseResponse response = service.UserExists(test.UserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static TheoryData<IsValidUserTestInput> IsValidUserTests = new TheoryData<IsValidUserTestInput>()
        {
            new IsValidUserTestInput()
            {
                TestName = "Test user does not exist",
                ExpectedResponse = BaseResponse.CreateError("User not found"),
                UserId = 3
            },
            new IsValidUserTestInput()
            {
                TestName = "Test user exist",
                ExpectedResponse = BaseResponse.CreateSuccess(),
                UserId = 1,
                UsersToAdd = new List<User>() {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19)
                    }
                }
            }
        };
        public class IsValidUserTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            public int UserId { get; set; }
            public BaseResponse ExpectedResponse { get; set; }
        }
        #endregion IsValidUser
    }
}
