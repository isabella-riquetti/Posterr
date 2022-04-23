using FluentAssertions;
using NSubstitute;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Services;
using Posterr.Services.Model;
using Posterr.Services.Model.User;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Posterr.Tests.Services
{
    public class UserServiceTest
    {
        #region GetUserProfile
        [Theory, MemberData(nameof(GetUserProfileTests))]
        public void GetUserProfileTest(GetUserProfileTestInput test)
        {
            var userRepositorySubstitute = Substitute.For<IUserRepository>();
            var userTimelineServiceSubstitute = Substitute.For<IUserTimelineService>();
            var followRepositorySubstitute = Substitute.For<IFollowRepository>();
            userRepositorySubstitute.GetUser(Arg.Any<int>()).Returns(test.GetUserResponse);
            userTimelineServiceSubstitute.GetUserPosts(Arg.Any<int>()).Returns(test.GetUserPostsResponse);
            followRepositorySubstitute.IsUserFollowedBy(Arg.Any<int>(), Arg.Any<int>()).Returns(test.IsUserFollowedByAuthenticatedUserResponse);

            var service = new UserService(userRepositorySubstitute, userTimelineServiceSubstitute, followRepositorySubstitute);
            BaseResponse<UserProfileModel> response = service.GetUserProfile(test.UserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static readonly TheoryData<GetUserProfileTestInput> GetUserProfileTests = new()
        {
            new GetUserProfileTestInput()
            {
                TestName = "Fail, user not found",
                UserId = 100,
                AuthenticatedUserId = 1,
                GetUserResponse = new List<User>().AsQueryable(),
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateError("User not found"),
            },
            new GetUserProfileTestInput()
            {
                TestName = "Fail to get posts",
                GetUserResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19),
                        Following = new List<Follow>()
                        {
                            new Follow()
                            {
                                Id = 1,
                                FollowerId = 1
                            }
                        },
                        Followers = new List<Follow>(),
                        Posts = new List<Post>()
                    }
                }.AsQueryable(),
                ExpectedResponse = BaseResponse<UserProfileModel>.CreateError("Failed to get posts"),
                UserId = 1,
                AuthenticatedUserId = 1,
                GetUserPostsResponse = BaseResponse<IList<PostResponseModel>>
                    .CreateError("Failed to get posts")
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, follow someone",
                GetUserResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19),
                        Following = new List<Follow>(),
                        Followers = new List<Follow>()
                        {
                            new Follow()
                        },
                        Posts = new List<Post>()
                    }
                }.AsQueryable(),
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
                    .CreateSuccess(new List<PostResponseModel>())
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, is followed by someone",
                GetUserResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19),
                        Following = new List<Follow>()
                        {
                            new Follow()
                        },
                        Followers = new List<Follow>(),
                        Posts = new List<Post>()
                    }
                }.AsQueryable(),
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
                    .CreateSuccess(new List<PostResponseModel>())
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, is followed by the authenticated user",
                GetUserResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19),
                        Following = new List<Follow>()
                        {
                            new Follow()
                        },
                        Followers = new List<Follow>(),
                        Posts = new List<Post>()
                    }
                }.AsQueryable(),
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
                IsUserFollowedByAuthenticatedUserResponse = true
            },
            new GetUserProfileTestInput()
            {
                TestName = "Success, has post, repost and quote post and over 5 posts",
                GetUserResponse = new List<User>()
                {
                    new User()
                    {
                        Id = 1,
                        Name = "Test1",
                        Username = "Test1",
                        CreatedAt = new DateTime(2022,4,19),
                        Following = new List<Follow>(),
                        Followers = new List<Follow>(),
                        Posts = new List<Post>()
                        {
                            new Post(),
                            new Post(),
                            new Post(),
                            new Post(),
                            new Post(),
                            new Post()
                        }
                    }
                }.AsQueryable(),
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
                    })
            },
        };
        public class GetUserProfileTestInput
        {
            public string TestName { get; set; }
            public BaseResponse<UserProfileModel> ExpectedResponse { get; set; }

            public int UserId { get; set; }
            public int AuthenticatedUserId { get; set; }

            public BaseResponse<IList<PostResponseModel>> GetUserPostsResponse { get; set; }
            public bool IsUserFollowedByAuthenticatedUserResponse { get; set; }

            public IQueryable<User> GetUserResponse { get; set; }
        }
        #endregion GetUserProfile

        #region UserExists
        [Theory, MemberData(nameof(UserExistsTests))]
        public void UserExistsTest(UserExistsTestInput test)
        {
            var userRepositorySubstitute = Substitute.For<IUserRepository>();
            var userTimelineServiceSubstitute = Substitute.For<IUserTimelineService>();
            var followRepositorySubstitute = Substitute.For<IFollowRepository>();
            userRepositorySubstitute.UserExists(Arg.Any<int>()).Returns(test.UserExistsResponse);

            var service = new UserService(userRepositorySubstitute, userTimelineServiceSubstitute, followRepositorySubstitute);
            BaseResponse response = service.UserExists(test.UserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static readonly TheoryData<UserExistsTestInput> UserExistsTests = new()
        {
            new UserExistsTestInput()
            {
                TestName = "Test user does not exist",
                UserExistsResponse = false,
                ExpectedResponse = BaseResponse.CreateError("User not found"),
                UserId = 3
            },
            new UserExistsTestInput()
            {
                TestName = "Test user exist",
                ExpectedResponse = BaseResponse.CreateSuccess(),
                UserId = 1,
                UserExistsResponse = true
            }
        };
        public class UserExistsTestInput
        {
            public string TestName { get; set; }
            public int UserId { get; set; }
            public BaseResponse ExpectedResponse { get; set; }
            public bool UserExistsResponse { get; internal set; }
        }
        #endregion UserExists
    }
}
