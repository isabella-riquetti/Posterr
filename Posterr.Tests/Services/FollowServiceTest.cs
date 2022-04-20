using Posterr.DB;
using Posterr.Services;
using Posterr.Services.Model;
using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Posterr.DB.Models;
using NSubstitute;
using Posterr.Infra.Interfaces;
using NSubstitute.Core;

namespace Posterr.Tests.Services
{
    public class FollowServiceTest
    {
        #region FollowUser
        [Theory, MemberData(nameof(FollowUserTests))]
        public void FollowUserTest(FollowUserTestInput test)
        {
            var followRepositorySubstitute = Substitute.For<IFollowRepository>();
            followRepositorySubstitute.IsUserFollowedBy(Arg.Any<int>(), Arg.Any<int>(), out Arg.Any<Follow>()).Returns(test.IsFollowed);
            followRepositorySubstitute.UpdateUnfollowedStatus(Arg.Any<Follow>(), Arg.Any<bool>());
            followRepositorySubstitute.CreateFollow(Arg.Any<int>(), Arg.Any<int>());

            var service = new FollowService(followRepositorySubstitute);
            BaseResponse response = service.FollowUser(test.FollowUserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static TheoryData<FollowUserTestInput> FollowUserTests = new TheoryData<FollowUserTestInput>()
        {
            new FollowUserTestInput()
            {
                TestName = "Fail, user already followed",
                AuthenticatedUserId = 1,
                FollowUserId = 2,
                IsFollowed = true,
                ExpectedResponse = BaseResponse.CreateError("User is already followed by you"),
            },
            new FollowUserTestInput()
            {
                TestName = "Success, create the follow",
                AuthenticatedUserId = 1,
                FollowUserId = 3,
                ExpectedResponse = BaseResponse.CreateSuccess()
            },
            new FollowUserTestInput()
            {
                TestName = "Success, update to follow again",
                AuthenticatedUserId = 1,
                FollowUserId = 3,
                ExpectedResponse = BaseResponse.CreateSuccess()
            }
        };
        public class FollowUserTestInput
        {
            public string TestName { get; set; }
            
            public int AuthenticatedUserId { get; set; }
            public int FollowUserId { get; set; }
            public bool IsFollowed { get; set; }

            public BaseResponse ExpectedResponse { get; set; }
        }
        #endregion FollowUser

        #region UnfollowUser
        [Theory, MemberData(nameof(UnfollowUserTests))]
        public void UnfollowUserTest(UnfollowUserTestInput test)
        {
            var followRepositorySubstitute = Substitute.For<IFollowRepository>();
            followRepositorySubstitute.IsUserFollowedBy(Arg.Any<int>(), Arg.Any<int>(), out Arg.Any<Follow>()).Returns(test.IsFollowed);
            followRepositorySubstitute.UpdateUnfollowedStatus(Arg.Any<Follow>(), Arg.Any<bool>());
            followRepositorySubstitute.CreateFollow(Arg.Any<int>(), Arg.Any<int>());
            
            var service = new FollowService(followRepositorySubstitute);
            BaseResponse response = service.UnfollowUser(test.UnfollowUserId, test.AuthenticatedUserId);

            response.Should().BeEquivalentTo(test.ExpectedResponse);
        }

        public static TheoryData<UnfollowUserTestInput> UnfollowUserTests = new TheoryData<UnfollowUserTestInput>()
        {
            new UnfollowUserTestInput()
            {
                TestName = "Fail, the follow does not exist",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
                ExpectedResponse = BaseResponse.CreateError("You don't follow this user")
            },
            new UnfollowUserTestInput()
            {
                TestName = "Fail, user already unfollowed",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
                IsFollowed = false,
                ExpectedResponse = BaseResponse.CreateError("You don't follow this user")
            },
            new UnfollowUserTestInput()
            {
                TestName = "Success, unfollow the user",
                AuthenticatedUserId = 1,
                UnfollowUserId = 2,
                IsFollowed = true,
                ExpectedResponse = BaseResponse.CreateSuccess()
            },
        };
        public class UnfollowUserTestInput
        {
            public string TestName { get; set; }

            public int AuthenticatedUserId { get; set; }
            public int UnfollowUserId { get; set; }
            public bool IsFollowed { get; set; }

            public BaseResponse ExpectedResponse { get; set; }
        }
        #endregion UnfollowUser
    }
}
