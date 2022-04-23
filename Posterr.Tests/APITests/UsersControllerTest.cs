using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Posterr.Controllers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using Xunit;

namespace Posterr.Tests.Controllers
{
    public class UsersControllerTest
    {
        #region [Route("{userId}")]
        [Theory, MemberData(nameof(GetProfileTests))]
        public void GetProfileTest(GetProfileTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();
            var followServiceSubstitute = Substitute.For<IFollowService>();

            userServiceSubstitute.UserExists(Arg.Any<int>()).Returns(test.UserExistExpectedResponse);
            userServiceSubstitute.GetUserProfile(Arg.Any<int>(), Arg.Any<int>()).Returns(test.UserProfileResponse);

            var controller = new UsersController(userServiceSubstitute, followServiceSubstitute);
            IActionResult response = controller.GetUserProfile(test.UserId);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.UserProfileResponse.Data, ((OkObjectResult)response).Value);
            }
        }


        public static readonly TheoryData<GetProfileTestInput> GetProfileTests = new()
        {
            new GetProfileTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = -1,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new GetProfileTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = 0,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new GetProfileTestInput()
            {
                TestName = "Fail, user does not exist",
                ExpectSuccess = false,
                UserId = 10,
                UserExistExpectedResponse = BaseResponse.CreateError("User not found"),
                ExpectedErrorMessage = "User not found"
            },
            new GetProfileTestInput()
            {
                TestName = "Fail at getting user",
                ExpectSuccess = false,
                UserId = 1,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                ExpectedErrorMessage = "User not found",
                UserProfileResponse = BaseResponse<UserProfileModel>.CreateError("User not found")
            },
            new GetProfileTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 1,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                UserProfileResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    UserId = 1,
                    CreatedAt = DateTime.Now.ToString(),
                    Followed = true,
                    Followers = 1,
                    Following = 3,
                    Posts = 0,
                    Username = "test",
                    TopPosts = new List<PostResponseModel>()
                })
            },
        };
        public class GetProfileTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public BaseResponse<UserProfileModel> UserProfileResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public BaseResponse UserExistExpectedResponse { get; internal set; }
        }
        #endregion [Route("{userId}")]

        #region [Route("follow/{userId}")]
        [Theory, MemberData(nameof(FollowTests))]
        public void FollowTest(FollowTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();
            var followServiceSubstitute = Substitute.For<IFollowService>();

            userServiceSubstitute.UserExists(Arg.Any<int>()).Returns(test.UserExistExpectedResponse);
            followServiceSubstitute.FollowUser(Arg.Any<int>(), Arg.Any<int>()).Returns(test.FollowResponse);

            var controller = new UsersController(userServiceSubstitute, followServiceSubstitute);
            IActionResult response = controller.Follow(test.UserId);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkResult>(response);
            }
        }


        public static readonly TheoryData<FollowTestInput> FollowTests = new()
        {
            new FollowTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = -1,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new FollowTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = 0,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new FollowTestInput()
            {
                TestName = "Fail, user does not exist",
                ExpectSuccess = false,
                UserId = 10,
                UserExistExpectedResponse = BaseResponse.CreateError("User not found"),
                ExpectedErrorMessage = "User not found"
            },
            new FollowTestInput()
            {
                TestName = "Fail, userId is the same as the authenticated",
                ExpectSuccess = false,
                UserId = 1,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                ExpectedErrorMessage = "You can't follow yourself"
            },
            new FollowTestInput()
            {
                TestName = "Fail at following",
                ExpectSuccess = false,
                UserId = 2,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                ExpectedErrorMessage = "User is already followed by you",
                FollowResponse = BaseResponse.CreateError("User is already followed by you")
            },
            new FollowTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 3,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                FollowResponse = BaseResponse.CreateSuccess()
            },
        };
        public class FollowTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public BaseResponse FollowResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public BaseResponse UserExistExpectedResponse { get; set; }
        }
        #endregion [Route("follow/{userId}")]

        #region [Route("unfollow/{userId}")]
        [Theory, MemberData(nameof(UnfollowTests))]
        public void UnfollowTest(UnfollowTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();
            var followServiceSubstitute = Substitute.For<IFollowService>();

            userServiceSubstitute.UserExists(Arg.Any<int>()).Returns(test.UserExistExpectedResponse);
            followServiceSubstitute.UnfollowUser(Arg.Any<int>(), Arg.Any<int>()).Returns(test.UnfollowResponse);

            var controller = new UsersController(userServiceSubstitute, followServiceSubstitute);
            IActionResult response = controller.UnfollowUser(test.UserId);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkResult>(response);
            }
        }


        public static readonly TheoryData<UnfollowTestInput> UnfollowTests = new()
        {
            new UnfollowTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = -1,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new UnfollowTestInput()
            {
                TestName = "Fail, invalid user ID",
                ExpectSuccess = false,
                UserId = 0,
                ExpectedErrorMessage = "Invalid User Id, the ID should be between 1 and 2147483647"
            },
            new UnfollowTestInput()
            {
                TestName = "Fail, user does not exist",
                ExpectSuccess = false,
                UserId = 10,
                UserExistExpectedResponse = BaseResponse.CreateError("User not found"),
                ExpectedErrorMessage = "User not found"
            },
            new UnfollowTestInput()
            {
                TestName = "Fail at unfollowing",
                ExpectSuccess = false,
                UserId = 1,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                ExpectedErrorMessage = "You don't follow this user",
                UnfollowResponse = BaseResponse.CreateError("You don't follow this user")
            },
            new UnfollowTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 1,
                UserExistExpectedResponse = BaseResponse.CreateSuccess(),
                UnfollowResponse = BaseResponse.CreateSuccess()
            },
        };
        public class UnfollowTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public BaseResponse UnfollowResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
            public BaseResponse UserExistExpectedResponse { get; set; }
        }
        #endregion [Route("unfollow/{userId}")]
    }
}
