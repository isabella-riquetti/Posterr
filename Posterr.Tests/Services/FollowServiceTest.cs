using Posterr.DB;
using Posterr.Services;
using Posterr.Services.Model;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Posterr.DB.Models;

namespace Posterr.Tests.Services
{
    public class FollowServiceTest
    {
        #region FollowUser
        [Theory, MemberData(nameof(FollowUserTests))]
        public void FollowUserTest(FollowUserTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new FollowService(apiContext);
            BaseResponse response = service.FollowUser(test.FollowUserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);

            if (test.ExpectedResponse.Success)
            {
                apiContext.Follows.Should().ContainSingle(f => f.FollowerId == test.AuthenticatedUserId && f.FollowingId == test.FollowUserId && f.Unfollowed == false);
            }
            else
            {
                apiContext.Follows.Should().NotContain(f => f.FollowerId == test.AuthenticatedUserId && f.FollowingId == test.FollowUserId && f.Unfollowed == true);
            }
        }

        public static TheoryData<FollowUserTestInput> FollowUserTests = new TheoryData<FollowUserTestInput>()
        {
            new FollowUserTestInput()
            {
                TestName = "Fail, user already followed",
                AuthenticatedUserId = 1,
                FollowUserId = 2,
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
                },
                ExpectedResponse = BaseResponse.CreateError("User is already followed by you"),
            },
            new FollowUserTestInput()
            {
                TestName = "Success, create the follow",
                AuthenticatedUserId = 1,
                FollowUserId = 3,
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
                ExpectedResponse = BaseResponse.CreateSuccess()
            },
            new FollowUserTestInput()
            {
                TestName = "Success, update to follow again",
                AuthenticatedUserId = 1,
                FollowUserId = 3,
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
                        FollowingId = 2,
                        Unfollowed = true
                    }
                },
                ExpectedResponse = BaseResponse.CreateSuccess()
            }
        };
        public class FollowUserTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }
            
            public int AuthenticatedUserId { get; set; }
            public int FollowUserId { get; set; }
            
            public BaseResponse ExpectedResponse { get; set; }
        }
        #endregion FollowUser

        #region FollowUser
        [Theory, MemberData(nameof(UnfollowUserTests))]
        public void UnfollowUserTest(UnfollowUserTestInput test)
        {
            ApiContext apiContext = test.CreateNewInMemoryContext();

            var service = new FollowService(apiContext);
            BaseResponse response = service.UnfollowUser(test.UnfollowUserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);

            if (test.ExpectedResponse.Success)
            {
                apiContext.Follows.Should().ContainSingle(f => f.FollowerId == test.AuthenticatedUserId && f.FollowingId == test.UnfollowUserId && f.Unfollowed == true);
            }
            else
            {
                apiContext.Follows.Should().NotContain(f => f.FollowerId == test.AuthenticatedUserId && f.FollowingId == test.UnfollowUserId && f.Unfollowed == false);
            }
        }

        public static TheoryData<UnfollowUserTestInput> UnfollowUserTests = new TheoryData<UnfollowUserTestInput>()
        {
            new UnfollowUserTestInput()
            {
                TestName = "Fail, the follow does not exist",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
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
                ExpectedResponse = BaseResponse.CreateError("You don't follow this user")
            },
            new UnfollowUserTestInput()
            {
                TestName = "Fail, user already unfollowed",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
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
                        FollowingId = 2,
                        Unfollowed = true
                    }
                },
                ExpectedResponse = BaseResponse.CreateError("You don't follow this user")
            },
            new UnfollowUserTestInput()
            {
                TestName = "Success, unfollow the user",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
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
                        FollowingId = 2,
                        Unfollowed = false
                    }
                },
                ExpectedResponse = BaseResponse.CreateSuccess()
            },
        };
        public class UnfollowUserTestInput : DatatbaseTestInput
        {
            public string TestName { get; set; }

            public int AuthenticatedUserId { get; set; }
            public int UnfollowUserId { get; set; }

            public BaseResponse ExpectedResponse { get; set; }
        }
        #endregion FollowUser
    }
}
