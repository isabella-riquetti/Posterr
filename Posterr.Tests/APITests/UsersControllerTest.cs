using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using Posterr.Controllers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using Xunit;

namespace Posterr.Tests
{
    public class UsersControllerTest
    {
        #region [Route("{id}")]
        [Theory, MemberData(nameof(GetProfileTests))]
        public async void GetProfileTest(GetProfileTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();
            
            userServiceSubstitute.GetUserProfile(Arg.Any<int>(), Arg.Any<int>()).Returns(test.UserProfileResponse);

            var controller = new UsersController(userServiceSubstitute, null);
            IActionResult response = await controller.Get(test.UserId);

            if(!test.ExpectSuccess)
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


        public static TheoryData<GetProfileTestInput> GetProfileTests = new TheoryData<GetProfileTestInput>()
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
                TestName = "Fail at getting user",
                ExpectSuccess = false,
                UserId = 1,
                ExpectedErrorMessage = "User not found",
                UserProfileResponse = BaseResponse<UserProfileModel>.CreateFailure("User not found")
            },
            new GetProfileTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 1,
                UserProfileResponse = BaseResponse<UserProfileModel>.CreateSuccess(new UserProfileModel()
                {
                    Id = 1,
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
        }
        #endregion [Route("{id}")]

        #region [Route("follow/{id}")]
        [Theory, MemberData(nameof(FollowTests))]
        public void FollowTest(FollowTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();

            userServiceSubstitute.FollowUser(Arg.Any<int>(), Arg.Any<int>()).Returns(test.FollowResponse);

            var controller = new UsersController(userServiceSubstitute, null);
            IActionResult response = controller.Follow(test.UserId);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.FollowResponse.Data, ((OkObjectResult)response).Value);
            }
        }


        public static TheoryData<FollowTestInput> FollowTests = new TheoryData<FollowTestInput>()
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
                TestName = "Fail at following",
                ExpectSuccess = false,
                UserId = 1,
                ExpectedErrorMessage = "User is already followed by you",
                FollowResponse = BaseResponse<string>.CreateFailure("User is already followed by you")
            },
            new FollowTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 1,
                FollowResponse = BaseResponse<string>.CreateSuccess("You now follow this user")
            },
        };
        public class FollowTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public BaseResponse<string> FollowResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("follow/{id}")]

        #region [Route("unfollow/{id}")]
        [Theory, MemberData(nameof(UnfollowTests))]
        public void UnfollowTest(UnfollowTestInput test)
        {
            var userServiceSubstitute = Substitute.For<IUserService>();

            userServiceSubstitute.UnfollowUser(Arg.Any<int>(), Arg.Any<int>()).Returns(test.UnfollowResponse);

            var controller = new UsersController(userServiceSubstitute, null);
            IActionResult response = controller.Unfollow(test.UserId);

            if (!test.ExpectSuccess)
            {
                Assert.IsType<BadRequestObjectResult>(response);
                Assert.Equal(test.ExpectedErrorMessage, ((BadRequestObjectResult)response).Value);
            }
            else
            {
                Assert.IsType<OkObjectResult>(response);
                Assert.Equal(test.UnfollowResponse.Data, ((OkObjectResult)response).Value);
            }
        }


        public static TheoryData<UnfollowTestInput> UnfollowTests = new TheoryData<UnfollowTestInput>()
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
                TestName = "Fail at unfollowing",
                ExpectSuccess = false,
                UserId = 1,
                ExpectedErrorMessage = "You don't follow this user",
                UnfollowResponse = BaseResponse<string>.CreateFailure("You don't follow this user")
            },
            new UnfollowTestInput()
            {
                TestName = "Success",
                ExpectSuccess = true,
                UserId = 1,
                UnfollowResponse = BaseResponse<string>.CreateSuccess("You unfollowed this user")
            },
        };
        public class UnfollowTestInput
        {
            public string TestName { get; set; }
            public bool ExpectSuccess { get; set; }
            public int UserId { get; set; }
            public BaseResponse<string> UnfollowResponse { get; set; }
            public string ExpectedErrorMessage { get; set; }
        }
        #endregion [Route("unfollow/{id}")]
    }
}
